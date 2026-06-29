# CryptoApp — Backend de microservicios

Migración del monolito `CryptoAppBackEnd` a 5 microservicios independientes, cada uno con su
propia base de datos PostgreSQL, orquestados con Docker Compose y expuestos al frontend a través
de un **API Gateway (YARP)**. Reemplaza por completo a Supabase.

> El monolito original (`../CryptoAppBackEnd`) se conserva intacto como referencia.

## Arquitectura

```
                 ┌─────────────────────────┐
  Frontend ────► │   API Gateway (YARP)     │  :8080  ← única URL para el navegador
  (Vite)         └────────────┬────────────┘
                              │ enruta por path
   ┌──────────────┬───────────┼───────────────┬──────────────┐
   ▼              ▼           ▼                ▼              ▼
 auth-service  market-service portfolio-service trading-service admin-service
   :8081         :8082         :8083            :8084          :8085
   │              │             │                │              │
 auth-db        market-db    portfolio-db     trading-db      admin-db
  :5433          :5434         :5435            :5436          :5437
```

Cada servicio crea su esquema al arrancar (`EnsureCreated`) y no comparte base de datos con
ningún otro (patrón *database-per-service*). Los datos que cruzan fronteras (id de usuario,
nombre/símbolo de moneda) se **desnormalizan**: no hay claves foráneas entre servicios.

## Servicios y endpoints (vía gateway, prefijo `http://localhost:8080`)

| Servicio   | Responsabilidad                    | Endpoints principales |
|------------|------------------------------------|-----------------------|
| **auth**   | Usuarios, login, roles, avatares   | `POST /api/auth/login`, `POST /api/auth/register`, `GET /api/users`, `PUT /api/users/{id}`, `POST /api/users/{id}/role`, `POST /api/users/{id}/avatar` |
| **market** | Precios (proxy + caché de CoinGecko)| `GET /api/market/coins?currency=usd`, `GET /api/market/coins/{id}` |
| **portfolio** | Portafolios y activos           | `GET/POST/PUT/DELETE /api/portfolios`, `.../assets` |
| **trading**| Compras/ventas y PnL realizado     | `GET /api/movements?userId=&role=`, `POST /api/trades` |
| **admin**  | Registro y consulta de errores     | `POST /api/errors`, `GET /api/errors` |

Cada servicio expone además `GET /health` y Swagger UI en su puerto directo (ej. `http://localhost:8081/swagger`).

## Cómo levantar

Requisitos: Docker Desktop.

```bash
cd microservices
docker compose up --build
```

Esto construye las 6 imágenes (.NET 10), levanta las 5 Postgres y deja el gateway en
`http://localhost:8080`. Las migraciones/esquemas y los datos semilla se crean automáticamente.

Para detener y limpiar volúmenes de BD:

```bash
docker compose down -v
```

## Usuarios semilla (auth-service)

| Email                    | Contraseña  | Rol   |
|--------------------------|-------------|-------|
| `admin@crypto.edu`       | `admin123`  | admin |
| `jane.smith@example.com` | `secret123` | user  |
| `john.doe@example.com`   | `secret123` | user  |

> Solo `admin@crypto.edu` puede cambiar roles (validado en el backend).

## Conexión con el frontend

El frontend (`../../CryptoAppProject`) ya apunta al gateway mediante la variable `VITE_API_URL`
(ver su `.env`, por defecto `http://localhost:8080`). Arrancar el frontend:

```bash
cd ../../CryptoAppProject
npm install
npm run dev
```

## Notas de seguridad (academico → producción)

- Contraseñas con SHA-256 sin sal: migrar a **bcrypt/Argon2**.
- El login no emite JWT todavía; añadir **autenticación por token** y validarla en el gateway/servicios.
- CORS está abierto (`AllowAnyOrigin`): restringir a los orígenes del frontend.
- Avatares se guardan en disco del contenedor: usar un volumen persistente o almacenamiento de objetos.

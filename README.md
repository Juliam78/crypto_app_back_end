# CryptoApp — Back-End (.NET + Arquitectura Hexagonal)

Back-end de **CryptoApp**, una aplicación académica de **simulación de trading de
criptomonedas**. Este repositorio contiene exclusivamente el servicio de servidor: una API
REST construida en **C# / .NET 10** siguiendo **arquitectura hexagonal (puertos y
adaptadores)** y persistencia con **Entity Framework Core** sobre **PostgreSQL**.

> El cliente web (React + TypeScript) vive en un repositorio separado:
> **[crypto_app_front_end](https://github.com/Juliam78/crypto_app_front_end)**.

---

## 🎯 Propósito

Exponer, mediante una API REST, toda la lógica de negocio de la aplicación:

- **Autenticación y usuarios** — registro, inicio de sesión (emisión de token de sesión),
  gestión de perfil, roles (`admin` / `employee` / `user`) y avatares.
- **Mercado** — precios de criptomonedas (proxy hacia CoinGecko con caché, para no agotar el
  límite de la API gratuita y poder responder aunque el proveedor falle).
- **Trading** — registro de compras/ventas, cálculo de cantidad y del PnL realizado.
- **Portafolios** — posiciones derivadas de los movimientos del usuario.
- **Administración** — registro y consulta de errores de la aplicación.
- **Academia** — lecciones educativas y señales de compra/venta por moneda; el personal
  (`admin`/`employee`) las crea, edita y publica, y los usuarios consultan las publicadas.
- **Asistente IA ("Cripto")** — mascota conversacional que responde preguntas y sugiere
  comprar/vender, **fundamentada en datos reales** (mercado, portafolio del usuario y señales
  publicadas). Usa un LLM configurable y gratuito (Ollama local u otro endpoint compatible con
  OpenAI), con una **respuesta de respaldo determinista** si el LLM no está disponible.

El objetivo didáctico es demostrar una **separación estricta de capas** donde el dominio es
C# puro, independiente de frameworks de persistencia o web.

---

## 🏛️ Arquitectura hexagonal

Solución multi-proyecto (`.sln`) con **separación física** de capas. La dirección de las
dependencias **siempre apunta hacia el dominio**:

```
CryptoApp.Web  ──►  CryptoApp.Application  ──►  CryptoApp.Domain
       │                                            ▲
       └──────►  CryptoApp.Infrastructure  ─────────┘
```

| Proyecto | Tipo | Responsabilidad | Depende de |
|----------|------|-----------------|------------|
| **CryptoApp.Domain** | Class Library | Entidades de negocio encapsuladas (constructores con validación, propiedades con `private set`) y reglas puras. **Sin EF ni ASP.NET.** | — |
| **CryptoApp.Application** | Class Library | Casos de uso y **puertos** (interfaces de salida: repositorios, servicios externos). Orquesta el dominio inyectando puertos abstractos. | Domain |
| **CryptoApp.Infrastructure** | Class Library | **Adaptadores** concretos: repositorios EF Core, `DbContext`, modelos de persistencia (`*DbModel`), mappers Dominio↔DbModel, migraciones y clientes externos (CoinGecko). | Application, Domain |
| **CryptoApp.Web** | ASP.NET Core Web API | Punto de entrada HTTP: controllers, DTOs, Swagger, CORS y **registro de dependencias** (enlaza cada puerto con su adaptador). | Application, Infrastructure |

### Principios clave

- **Pureza del dominio**: el `.csproj` de `CryptoApp.Domain` no referencia ningún paquete de
  EF Core ni de ASP.NET. La regla la garantizan los límites de proyecto, no la disciplina.
- **Puertos y adaptadores**: la capa de aplicación define interfaces (p. ej.
  `IPersonRepository`); la infraestructura las implementa (`PersonRepository`). La aplicación
  nunca conoce a EF Core.
- **Inversión de dependencias**: en `Program.cs` se enlaza cada puerto con su adaptador
  (`AddScoped<IPersonRepository, PersonRepository>()`).

---

## 🗂️ Estructura del repositorio

```
crypto_app_back_end/
├── CryptoApp.sln
├── src/
│   ├── CryptoApp.Domain/
│   │   ├── Entities/            # Person, CryptoCurrency, Portfolio, PortfolioAsset,
│   │   │                        #   Movement, AppError (errores), Lesson (academia)
│   │   └── Shared/              # Helpers de validación
│   ├── CryptoApp.Application/
│   │   ├── Ports/               # Interfaces de salida (IPersonRepository, IMarketDataProvider,
│   │   │                        #   ITokenService, IPasswordHasher, ILessonRepository,
│   │   │                        #   IAssistantProvider, ...)
│   │   └── UseCases/            # PersonUseCase, MovementUseCase, AuthUseCase, MarketUseCase,
│   │                            #   LessonUseCase, AssistantUseCase, ...
│   ├── CryptoApp.Infrastructure/
│   │   └── Infraestructure/
│   │       ├── Persistence/     # AppDbContext, Models (*DbModel), Configurations, Mappers, Migrations
│   │       ├── Adapters/        # Implementaciones de los puertos (repositorios EF)
│   │       ├── Security/        # PasswordHasher (SHA-256), TokenService (HMAC)
│   │       ├── Market/          # CoinGeckoClient + caché de precios
│   │       └── Assistant/       # OpenAiCompatibleAssistantClient (LLM del asistente)
│   └── CryptoApp.Web/
│       ├── Controllers/         # Auth, Users, Market, Trades, Movements, Errors, Lessons, Assistant, ...
│       ├── Contracts/           # DTOs request/response
│       ├── Program.cs           # DI + pipeline HTTP
│       └── appsettings.json
└── .gitignore
```

---

## 🧰 Tecnologías

- **.NET 10** / C#
- **ASP.NET Core Web API** + **Swagger** (Swashbuckle)
- **Entity Framework Core 10** + **Npgsql** (proveedor PostgreSQL)
- **PostgreSQL** como base de datos relacional
- Migraciones gestionadas por **EF Core CLI** (`dotnet ef`)

---

## 💾 Persistencia

- **Conexión configurable** desde `appsettings.json` (`ConnectionStrings:DefaultConnection`).
- **Modelos de persistencia separados** (`*DbModel`) de las entidades de dominio; los
  repositorios traducen mediante **mappers** (Dominio → DbModel al guardar, DbModel → Dominio
  al leer).
- **Migraciones** versionadas y aplicadas con el CLI de EF Core (no se usa `EnsureCreated`).

---

## ▶️ Cómo ejecutar

### Requisitos
- [.NET SDK 10](https://dotnet.microsoft.com/)
- PostgreSQL en ejecución (local o Docker)
- Herramienta CLI de EF Core: `dotnet tool install --global dotnet-ef`

### Pasos

```bash
# 1. Clonar
git clone https://github.com/Juliam78/crypto_app_back_end.git
cd crypto_app_back_end

# 2. Configurar la cadena de conexión en src/CryptoApp.Web/appsettings.json
#    "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=cryptoapp;Username=postgres;Password=postgres" }

# 3. Aplicar migraciones (crea el esquema en PostgreSQL)
dotnet ef database update -p src/CryptoApp.Infrastructure -s src/CryptoApp.Web

# 4. Ejecutar la API (perfil http -> puerto 5243)
dotnet run --project src/CryptoApp.Web --launch-profile http
```

La API y su documentación Swagger quedan disponibles en `http://localhost:5243`
(la raíz `/` sirve Swagger UI en entorno de desarrollo).

---

## 🔌 API (resumen)

| Recurso | Endpoint | Descripción |
|---------|----------|-------------|
| Auth | `POST /api/auth/login` · `POST /api/auth/register` · `GET /api/auth/me` | Login (emite token), registro, sesión actual |
| Usuarios | `GET /api/users` · `PUT /api/users/{id}` · `POST /api/users/{id}/role` · `POST /api/users/{id}/avatar` | Gestión de usuarios, roles y avatar |
| Mercado | `GET /api/market/coins` · `GET /api/market/coins/{id}` | Listado y detalle de criptomonedas |
| Trading | `POST /api/trades` · `GET /api/movements` | Compras/ventas y movimientos |
| Admin | `GET /api/errors` · `POST /api/errors` | Registro y consulta de errores |
| Academia | `GET /api/lessons` · `POST/PUT/DELETE /api/lessons/{id}` · `POST /api/lessons/{id}/publish` | Lecciones/señales; lectura pública de lo publicado, gestión solo para staff (token) |
| Asistente | `POST /api/assistant/ask` | Pregunta a la mascota IA "Cripto" (respuesta fundamentada + descargo educativo) |

### Convenciones de contrato (DTOs)
- `id` se serializa como **string**.
- Rol: `'A'` ↔ `"admin"`, `'E'` ↔ `"employee"`, resto ↔ `"user"`.
- Tipo de movimiento: `'B'` ↔ `"buy"`, `'S'` ↔ `"sell"`.
- Academia: `kind` `'L'` ↔ `"lesson"`, `'S'` ↔ `"signal"`; `recommendation` `'B'/'S'/'H'` ↔ `"buy"/"sell"/"hold"`.
- Fechas en **ISO 8601 (UTC)**.

---

## 👥 Usuarios semilla

| Email | Password | Rol |
|-------|----------|-----|
| `admin@crypto.edu` | `admin123` | admin |
| `empleado@crypto.edu` | `empleado123` | employee |
| `jane.smith@example.com` | `secret123` | user |
| `john.doe@example.com` | `secret123` | user |

> Solo `admin@crypto.edu` puede cambiar roles (validado en el backend). El personal
> (`admin`/`employee`) puede gestionar la Academia.

---

## 🤖 Asistente IA ("Cripto") — configuración (gratis)

El asistente llama a un endpoint **compatible con la API de OpenAI** (`/chat/completions`),
configurable en la sección `Assistant` de la configuración:

```json
"Assistant": { "BaseUrl": "...", "Model": "...", "ApiKey": "..." }
```

- **Opción A — Ollama local (sin clave, sin costo):** instala [Ollama](https://ollama.com),
  `ollama pull llama3.2`, y deja `BaseUrl: "http://localhost:11434/v1"`, `Model: "llama3.2"`.
- **Opción B — proveedor gratuito con clave (p. ej. Google Gemini):** `BaseUrl`
  `https://generativelanguage.googleapis.com/v1beta/openai`, `Model` `gemini-2.5-flash`,
  y la clave en `ApiKey`.

> ⚠️ **No pongas claves en `appsettings.json`** (se versiona). Colócalas en
> `src/CryptoApp.Web/appsettings.Development.json`, que está en `.gitignore`. Si el LLM no
> responde, el endpoint cae a una **respuesta de respaldo** con datos reales (la app nunca falla).

---

## 🔒 Notas (contexto académico)

Pendientes de *hardening* para producción: contraseñas con sal (bcrypt/Argon2), validación de
token en toda la cadena, CORS restringido y persistencia de avatares fuera del contenedor.

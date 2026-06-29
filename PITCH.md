# Pitch — CryptoApp (7 minutos)

Guion para la presentación final: **3:30 min comerciales + 3:30 min técnicos**.

> **Antes de empezar**: ten levantados el backend (`http://localhost:5243`, Swagger en `/`) y el
> frontend (`http://localhost:5173`), y abiertas las pestañas con Swagger y el código (proyectos
> `CryptoApp.Domain` / `.Application` / `.Infrastructure` / `.Web`).

---

## ¿Qué significa "PnL"?

**PnL** = *Profit and Loss* (**Ganancia y Pérdida**). Es cuánto ganas o pierdes en una operación.

En CryptoApp usamos el **PnL realizado** (`realized_pnl`): la ganancia o pérdida que se
**concreta al VENDER** una criptomoneda. Se calcula así:

```
realized_pnl = (precio_de_venta − costo_promedio_de_compra) × cantidad_vendida
```

- **Positivo** → vendiste más caro de lo que te costó en promedio (ganancia). 🟢
- **Negativo** → vendiste más barato (pérdida). 🔴
- En las **compras** el PnL es siempre **0**: todavía no has "realizado" nada, solo cambiaste
  dinero por cripto.

El **costo promedio de compra** es un *promedio ponderado* de todas tus compras previas de esa
moneda. Ejemplo: compras 1 BTC a $40.000 y luego 1 BTC a $50.000 → tu costo promedio es $45.000.
Si después vendes 1 BTC a $48.000, tu PnL realizado es `(48.000 − 45.000) × 1 = +$3.000`.

> En el código: el cálculo vive en el backend (`MovementUseCase.RegisterTrade`) y también se
> deriva en el cliente (`src/lib/portfolio.ts`) para mostrar el historial.

---

## Parte comercial (3:30) — demo en vivo

| Tiempo | Qué mostrar / decir |
|--------|---------------------|
| 0:00–0:30 | **Gancho**: "CryptoApp es un simulador de trading de cripto: practica comprar y vender con precios reales del mercado, sin arriesgar dinero." |
| 0:30–1:15 | **Login** con `jane.smith@example.com` / `secret123`. Muestra que la sesión persiste (refresca la página y sigues dentro). |
| 1:15–2:15 | **Mercado**: tabla de 20 monedas con precios reales (CoinGecko), que se autoactualizan. Clic en Bitcoin → **detalle** con gráfico y métricas de 24h. |
| 2:15–3:00 | **Operar**: compra una cantidad en USD de BTC → aparece la cantidad estimada y se registra. Ve al **Historial** y muestra el movimiento con su **PnL**. |
| 3:00–3:30 | **Valor de negocio**: "Ideal para educación financiera y onboarding de nuevos inversores; el rol admin gestiona usuarios y monitorea errores." Entra como admin (`admin@crypto.edu` / `admin123`) y enseña el panel. |

---

## Parte técnica (3:30)

| Tiempo | Qué explicar |
|--------|--------------|
| 0:00–0:45 | **Arquitectura general**: SPA React + TypeScript ↔ API REST .NET 10. Dos repositorios separados (front / back). Diagrama de las 4 capas. |
| 0:45–1:45 | **Arquitectura hexagonal (puertos y adaptadores)**: `Domain` (C# puro, sin EF) → `Application` (casos de uso + puertos) → `Infrastructure` (adaptadores EF / CoinGecko) → `Web` (controllers). Enseña que el `.csproj` de `Domain` no referencia EF = **pureza del dominio**. Ejemplo: `IPersonRepository` (puerto) ↔ `PersonRepository` (adaptador), enlazados en `Program.cs`. |
| 1:45–2:30 | **Persistencia**: EF Core + PostgreSQL, **modelos de persistencia separados** (`PersonDbModel`) con **mappers** Dominio↔DbModel, y **migraciones** gestionadas por `dotnet ef`. Muestra Swagger en `:5243`. |
| 2:30–3:10 | **Frontend**: Vite + React Router (rutas + guard de admin), capa de servicios sobre `apiFetch`, validación con Zod, sesión por token Bearer. Cálculo de PnL / costo promedio en el cliente. |
| 3:10–3:30 | **Flujo de datos completo**: un trade viaja React → `POST /api/trades` → `MovementUseCase` (calcula cantidad + **PnL**) → repositorio → PostgreSQL, y regresa al historial. Cierre. |

---

## Usuarios de demo

| Email | Password | Rol |
|-------|----------|-----|
| `admin@crypto.edu` | `admin123` | admin |
| `jane.smith@example.com` | `secret123` | user |
| `john.doe@example.com` | `secret123` | user |

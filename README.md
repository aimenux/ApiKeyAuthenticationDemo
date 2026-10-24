[![.NET](https://github.com/aimenux/ApiKeyAuthenticationDemo/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/ApiKeyAuthenticationDemo/actions/workflows/ci.yml)

# 🔑 ApiKeyAuthenticationDemo

> Eight small ASP.NET Core Web API projects that protect the same movies API with an **API key** in the `X-Api-Key` header, each using a different authentication mechanism.

> [!WARNING]
> ⚠️ **This is a demo.** An API key is a shared secret that is sent with **every request**. Anyone who can read the traffic can read the key and reuse it.
>
> 🔒 **Always use it over HTTPS**, and prefer short-lived tokens (OAuth 2.0 / OpenID Connect) for real systems.

## 📑 Table of contents

- [🎯 What is this?](#-what-is-this)
- [🔒 Why HTTPS is mandatory](#-why-https-is-mandatory)
- [🚀 Quick start](#-quick-start)
- [🧩 Examples at a glance](#-examples-at-a-glance)
- [🛡️ Security posture](#️-security-posture)
- [🧪 Testing](#-testing)
- [🏭 Going to production](#-going-to-production)
- [🗂️ Project structure](#️-project-structure)
- [🛠️ Tools](#️-tools)

## 🎯 What is this?

Each `ExampleNN` project exposes the same two endpoints and protects them with an API key:

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/movies/list` | Returns all movies |
| `GET` | `/api/movies/{movieId}` | Returns one movie (`404` if unknown) |

The key is read from the `X-Api-Key` request header and compared with `Authentication:ApiKey` from configuration. A missing or wrong key returns `401 Unauthorized`.

The examples differ in **how** the key is checked:

- Controllers or minimal APIs
- Middleware, authorization filters, endpoint filters, or a custom authentication scheme
- Global, per route group, per action, or per route

## 🔒 Why HTTPS is mandatory

An API key travels in a header on every request. Over plain HTTP it is visible to anyone on the network path, so it can be captured and replayed.

Each example therefore only listens on HTTPS:

- `WithHttpsCheck()` configures Kestrel to listen on the HTTPS port only. Plain-HTTP clients are refused.
- The port comes from `Https:Port` in configuration, or from the `https` entry in `urls`. If neither is set, startup fails.
- `UseHttpsRedirection()` redirects any HTTP request to HTTPS, and `UseHsts()` is enabled outside Development.

## 🚀 Quick start

### 1️⃣ Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) matching `global.json` (`10.0.x`)
- A trusted development certificate (`dotnet dev-certs https --trust`)

### 2️⃣ Run an example

```bash
dotnet run --project src/Example01
```

Each example uses its own HTTPS port (`5001`–`5008`, see the table below). The port is set in `appsettings.Development.json`:

```json
{
  "Https": {
    "Port": 5001
  },
  "Authentication": {
    "ApiKey": "xyz"
  }
}
```

> 🔌 Examples can run side by side because each one has its own port.

### 3️⃣ Call the API

```bash
# ✅ Valid key over HTTPS
curl -k -H "X-Api-Key: xyz" https://localhost:5001/api/movies/list

# ❌ No key → 401 Unauthorized
curl -k https://localhost:5001/api/movies/list

# ❌ Wrong key → 401 Unauthorized
curl -k -H "X-Api-Key: wrong" https://localhost:5001/api/movies/list

# ❌ Plain HTTP → connection refused (the server only listens on HTTPS)
curl http://localhost:5001/api/movies/list
```

> The development key `xyz` is committed for convenience. Never reuse it outside your machine.

## 🧩 Examples at a glance

| # | 🧱 Style | 🔑 Mechanism | 📍 Scope | ❓ Status Code | 🔌 Port |
|---|----------|--------------|----------|------------------|---------|
| 01 | Controllers | Middleware | Global | **401** | `5001` |
| 02 | Minimal API | Middleware | Global | **401** | `5002` |
| 03 | Controllers | Authorization filter | Global | 404 | `5003` |
| 04 | Minimal API | Endpoint filter | Per route group | 404 | `5004` |
| 05 | Controllers | Authorization filter | Per action (attribute) | 404 | `5005` |
| 06 | Minimal API | Endpoint filter | Per route | 404 | `5006` |
| 07 | Controllers | Custom authentication scheme | Per route group | 404 | `5007` |
| 08 | Minimal API | Custom authentication scheme | Per route | 404 | `5008` |

### 🔍 Reading the table

- **Middleware (01, 02)** runs for every request, so unknown routes are also protected and return `401`.
- **Filters and schemes (03–08)** run only for matched endpoints, so unknown routes return `404` as usual.
- **Protection by default:** route-group and global filters (03, 04) protect routes added later to that scope. Per-route attributes and filters (05, 06, 08) must be repeated on each new route, so a new route is public until it is protected.

### 🧭 Which one should I read first?

1. **Example01** for the simplest version: a middleware that compares the header with configuration.
2. **Example03** to see the same check as an authorization filter.
3. **Example07** for a custom authentication scheme, which is the most "framework-like" option.

## 🛡️ Security posture

| ✅ Measure | Where | Why it matters |
|-----------|-------|----------------|
| 🔐 **HTTPS-only listener** | `WithHttpsCheck()` in each `Program.cs` | Kestrel never opens an HTTP socket, so plain-HTTP clients are refused |
| 🔢 **Configurable HTTPS port** | `Https:Port` in `appsettings.Development.json` | No port hardcoded in code; the HTTPS port is set in configuration |
| ⏳ **HSTS** | `app.UseHsts()` | Browsers that have visited over HTTPS will upgrade future requests |
| 🚫 **Fail-closed startup** | `ValidateOnStart()` on `ApiKeyOptions` | The app refuses to start when `Authentication:ApiKey` is missing or blank, so an empty key can never accept requests |
| 🧾 **Typed configuration** | `ApiKeyOptions` bound to `Authentication` | Consumers read a validated `IOptions<ApiKeyOptions>` instead of raw strings |
| ⏱️ **Constant-time comparison** | `CryptographicOperations.FixedTimeEquals` in each filter, middleware, and handler | The key comparison does not leak how many leading characters match |
| 🪪 **`WWW-Authenticate: ApiKey`** | Every `401` response | Clients can discover the scheme from the challenge header |

### ⚠️ Known limitations

These are intentionally left out of the demo so the examples stay short:

- **Shared key.** A single key is used for every client, so it cannot be revoked per client.
- **Plaintext key in source control.** `appsettings.Development.json` contains the development key.

## 🧪 Testing

```bash
dotnet test
```

Each `ExampleNN.Tests` project covers the same behaviour:

| 🧪 Test | What it checks |
|--------|----------------|
| `WebApiTests` | Valid key returns `200`; wrong key returns `401`; missing or empty header returns `401`; missing or blank configured key stops startup |
| `ApiKeyMiddlewareTests` / `ApiKeySecurityFilterTests` / `ApiKeyAuthenticationHandlerTests` | Unit tests for the mechanism of each example |
| `MovieRepositoryTests` | The returned movies cannot be cast to a mutable list or changed |
| `MoviesControllerTests` / `MoviesEndpointsTests` | The HTTP result of each endpoint, using a mocked repository |

Integration tests run the real pipeline in memory with `WebApplicationFactory`. They use the HTTPS base address `https://localhost:5000` and do not bind any port.

## 🏭 Going to production

API keys are a simple shared secret. For a real API, consider:

| Option | Use it when... |
|--------|----------------|
| 🎟️ **Bearer tokens (JWT)** issued by an identity provider | You need stateless APIs and centralized login |
| 🪪 **OAuth 2.0 / OpenID Connect** (Microsoft Entra ID, Auth0, Keycloak, ...) | You need SSO, scopes, or third-party clients |
| 🔑 **Per-client API keys** stored hashed and revocable | You need simple server-to-server access with revocation |
| 🧱 **Mutual TLS** | You control both client and server and need strong client identity |

If you must keep API keys, at least:

1. 🔐 Serve them only over HTTPS, terminated by a reverse proxy or the server itself.
2. 🗄️ Keep keys out of source control (environment variables or a secret store), and store only hashes on the server.
3. 🔄 Rotate keys regularly and issue one key per client.
4. 🚦 Add rate limiting and log failures without logging the key.

## 🗂️ Project structure

```text
ApiKeyAuthenticationDemo/
├── 📁 src/
│   ├── 📁 Example01/          # Controllers + middleware
│   ├── 📁 Example02/          # Minimal API + middleware
│   ├── 📁 Example03/          # Controllers + global authorization filter
│   ├── 📁 Example04/          # Minimal API + route group endpoint filter
│   ├── 📁 Example05/          # Controllers + per-action authorization filter
│   ├── 📁 Example06/          # Minimal API + per-route endpoint filter
│   ├── 📁 Example07/          # Controllers + custom authentication scheme
│   └── 📁 Example08/          # Minimal API + custom authentication scheme
├── 📁 test/                   # One xUnit project per example
├── 📄 Directory.Packages.props # Central package versions
└── 📄 global.json             # Pinned .NET SDK
```

## 🛠️ Tools

| Tool | Purpose |
|------|---------|
| 🟣 **.NET 10 / ASP.NET Core** | Web API, Kestrel, authentication and authorization |
| 📘 **Swagger / Swashbuckle** | Interactive API docs (Development only) |
| 🧪 **xUnit** | Test framework |
| ✨ **AwesomeAssertions** | Readable assertions |
| 🎭 **NSubstitute** | Mocking repositories |
| 🔁 **GitHub Actions** | CI build and test on every push and pull request to `main` |

---

<p align="center">
  <sub>Made for learning 📚 · Not for production 🚫 · Always use HTTPS 🔒</sub>
</p>

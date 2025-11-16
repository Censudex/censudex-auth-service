# AuthService

Servicio de autenticación y autorización para la arquitectura de microservicios Censudex. Proporciona funcionalidades de registro, inicio de sesión, gestión de tokens JWT y blacklist de tokens revocados.

## Arquitectura y Patrón de Diseño

### Arquitectura: Microservicios con API Gateway

El AuthService implementa una arquitectura de capas (Layered Architecture) dentro de un ecosistema de microservicios, funcionando como servicio de autenticación centralizado.
```mermaid
flowchart TD
    A[API Gateway
    Puerto 5237]
    
    subgraph AuthService
        B1[Controllers] --> B2[Services]
        B2 --> B3[Repository/DbContext]
    end
    
    A -->|HTTP REST| B1
    B3 --> C[(PostgreSQL
    auth_service)]
```

### Patrones de Diseño Implementados:

1. **Repository Pattern:** Abstracción del acceso a datos con Entity Framework
2. **Dependency Injection:** Inyección de dependencias nativa de ASP.NET Core
3. **JWT Bearer Authentication:** Autenticación basada en tokens
4. **Blacklist Pattern:** Revocación de tokens mediante base de datos
5. **Service Layer Pattern:** Lógica de negocio separada de controladores

## Tecnologías Utilizadas

- **Framework:** ASP.NET Core 9.0
- **Base de Datos:** PostgreSQL 12+
- **ORM:** Entity Framework Core con Npgsql
- **Autenticación:** JWT Bearer (Microsoft.IdentityModel.Tokens)
- **Variables de Entorno:** DotNetEnv
- **Control de Versiones:** Git con Conventional Commits

## Modelo de Datos

### Entidad TokenBlacklist (PostgreSQL Table)
```sql
CREATE TABLE TokenBlacklist (
  Id UUID PRIMARY KEY,
  Token VARCHAR(500) NOT NULL,
  UserId VARCHAR(100) NOT NULL,
  BlacklistedAt TIMESTAMP NOT NULL,
  ExpiresAt TIMESTAMP NOT NULL
);
```

### Configuración JWT (JwtSettings)
```csharp
{
  "Secret": "string (min 32 caracteres)",
  "Issuer": "CensudexAPIGateway",
  "Audience": "CensudexClients",
  "ExpirationMinutes": 60
}
```

## Endpoints de la API

### Endpoints Disponibles

| Método | Endpoint | Descripción | Autenticación | Rol Requerido |
|--------|----------|-------------|---------------|---------------|
| `POST` | `/api/auth/login` | Iniciar sesión y obtener JWT | No | - |
| `POST` | `/api/auth/logout` | Cerrar sesión (blacklist token) | Sí | Cualquiera |
| `POST` | `/api/auth/validate` | Validar token JWT | Sí | Cualquiera |

### DTOs (Data Transfer Objects)

**LoginRequest:**
```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**LoginResponse:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "uuid",
  "username": "string",
  "email": "user@example.com",
  "role": "ADMIN | CLIENT",
}
```

## Instalación y Configuración

### Prerequisitos

- **.NET 9 SDK:** [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **IDE:** Visual Studio Code, Visual Studio 2022 o Rider

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Censudex/censudex-auth-service.git
cd censudex-auth-service
cd AuthService
```

### 2. Configurar Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto (mismo nivel que `AuthService.csproj`):
```env
# JWT Configuration
JWT_SECRET=my-super-secret-key-at-least-32-characters-long-for-jwt-validation!
JWT_ISSUER=CensudexAPIGateway
JWT_AUDIENCE=CensudexClients
JWT_EXPIRATION_MINUTES=60

# PostgreSQL (para blacklist de tokens)
DATABASE_HOST=localhost
DATABASE_PORT=5433
DATABASE_NAME=auth_service
DATABASE_USER=postgres
DATABASE_PASSWORD=postgres
```

### 3. Instalar Dependencias
```bash
dotnet restore
```

### 4. Levantar Base de Datos PostgreSQL

Crea la base de datos:
```bash
# Crea la base de datos auth_service con el archivo docker-compose.yml
docker-compose up -d
```

### 5. Aplicar Migraciones
```bash
# Crear migración inicial (si no existe)
dotnet ef migrations add InitialCreate

# Aplicar migraciones
dotnet ef database update
```

### 6. Ejecutar el Proyecto
```bash
dotnet run
```

El servicio estará disponible en:
- **HTTP:** http://localhost:5237
- **Swagger UI:** http://localhost:5237/swagger

## Uso de la API

### Ejemplos con cURL

**1. Iniciar Sesión:**
```bash
curl -X POST http://localhost:5237/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "johndoe",
  "email": "john@example.com",
  "role": "CLIENT",
  "expiresAt": "2025-11-08T13:00:00Z"
}
```

**2. Cerrar Sesión (Blacklist Token):**
```bash
curl -X POST http://localhost:5237/api/auth/logout \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**3. Validar Token:**
```bash
curl -X POST http://localhost:5237/api/auth/validate \
  -H "Content-Type: application/json" \
  -d '{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }'

```

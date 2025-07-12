# AuthMicroservice

## Descripción del Proyecto

AuthMicroservice es un microservicio desarrollado en .NET 9 que implementa autenticación y autorización utilizando JWT (JSON Web Tokens). Este proyecto está diseñado para ser escalable y fácil de integrar con otros sistemas. Incluye endpoints para gestionar usuarios, roles, y tokens de autenticación.

## Requisitos

- .NET 9 SDK
- SQL Server
- SQL Server Management Studio

## Instalación

1. Clona el repositorio:
git clone https://github.com/rjonathan87/AuthMicroservice.git
cd AuthMicroservice
2. Configura la base de datos:

   - Abre el archivo `AuthMicroservice.Api/Database/database.txt`.
   - Ejecuta el script en SQL Server Management Studio para crear la base de datos y las tablas necesarias.

3. Restaura los paquetes NuGet:
dotnet restore
4. Compila el proyecto:
dotnet build
5. Ejecuta el proyecto:
dotnet run --project src/AuthMicroservice.Api/AuthMicroservice.Api.csproj

## Endpoints Funcionales

### Autenticación

#### POST `/api/auth/login`

**Datos de entrada:**{
  "username": "string",
  "password": "string"
}
**Respuesta:**
{
  "token": "string",
  "refreshToken": "string"
}
#### POST `/api/auth/refresh`

**Datos de entrada:**{
  "refreshToken": "string"
}
**Respuesta:**
{
  "token": "string",
  "refreshToken": "string"
}
#### POST `/api/auth/register`

**Datos de entrada:**{
  "username": "string",
  "email": "string",
  "password": "string"
}
**Respuesta:**
{
  "userId": "string",
  "username": "string",
  "email": "string",
  "token": "string",
  "refreshToken": "string",
  "expiresAt": "datetime"
}
#### POST `/api/auth/revoke-token`

**Datos de entrada:**{
  "token": "string"
}
**Respuesta:**
{
  "success": true
}
### Usuarios

#### GET `/api/users`

**Respuesta:**[
  {
    "userId": "string",
    "username": "string",
    "email": "string",
    "isLocked": false
  }
]
#### POST `/api/users`

**Datos de entrada:**{
  "username": "string",
  "password": "string",
  "email": "string"
}
**Respuesta:**
{
  "userId": "string",
  "username": "string",
  "email": "string"
}
### Roles

#### GET `/api/roles`

**Respuesta:**[
  {
    "roleId": "string",
    "roleName": "string"
  }
]
#### POST `/api/roles`

**Datos de entrada:**{
  "roleName": "string"
}
**Respuesta:**
{
  "roleId": "string",
  "roleName": "string"
}
## Notas Adicionales

- Asegúrate de que el servidor SQL esté configurado correctamente y que el script de la base de datos se haya ejecutado sin errores.
- Los endpoints están protegidos con JWT, por lo que necesitarás un token válido para acceder a la mayoría de ellos.

## Contribuciones

Si deseas contribuir al proyecto, por favor abre un issue o envía un pull request.

## Licencia

Este proyecto está bajo la licencia MIT.
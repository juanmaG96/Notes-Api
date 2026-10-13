# Notes App - API (Backend)

API RESTful para el sistema de gestión de notas, metas y planificación temporal. Construida bajo los principios de Spec-Driven Development (SDD) con un enfoque estricto en el aislamiento de datos (Zero Data Leakage) y seguridad OWASP.

## Stack Tecnológico
* **Framework:** .NET 10 (C# 14)
* **ORM:** Entity Framework Core 10
* **Base de Datos:** PostgreSQL Serverless (Neon DB)
* **Seguridad:** Autenticación JWT (HttpOnly Cookies), BCryptPasswordHasher

## Requisitos Previos
* .NET 10 SDK
* PostgreSQL (Local o Neon DB)

## Configuración Local
1. Restaura las dependencias del proyecto:
   ```bash
   dotnet restore
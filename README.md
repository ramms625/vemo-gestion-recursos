# Vemo - Gestión de Recursos

## Resumen
Vemo es un sistema diseñado para la gestión eficiente de recursos y usuarios. Este proyecto incluye funcionalidades como la creación, edición y administración de usuarios, sesiones y recursos compartidos. Está construido sobre una arquitectura robusta que utiliza tecnologías modernas para garantizar escalabilidad y mantenibilidad.

## Tecnologías Utilizadas
- **C# y .NET Core**: Framework principal para el desarrollo del backend.
- **Entity Framework Core**: ORM utilizado para la interacción con la base de datos.
- **AutoMapper**: Herramienta para mapear objetos entre DTOs y entidades.
- **ASP.NET Identity**: Gestión de usuarios y autenticación.
- **SQL Server**: Base de datos relacional para almacenar la información.
- **Visual Studio Code**: IDE recomendado para el desarrollo y mantenimiento del proyecto.

## Recomendaciones para el Entendimiento del Proyecto
Para comprender y trabajar con este proyecto, se recomienda tener nociones básicas de:
1. **C# y .NET Core**:
   - Familiaridad con la programación orientada a objetos.
   - Conocimiento de conceptos como controladores, servicios y dependencias.
2. **Entity Framework Core**:
   - Comprender cómo funcionan las migraciones y el mapeo de entidades a tablas.
3. **AutoMapper**:
   - Saber cómo configurar perfiles para mapear objetos entre DTOs y entidades.
4. **ASP.NET Identity**:
   - Conocer los conceptos básicos de autenticación y autorización.
5. **SQL Server**:
   - Habilidad para gestionar bases de datos y realizar consultas básicas.

## Estructura del Proyecto
El proyecto está organizado de la siguiente manera:
- **`Vemo.Gestion.Recursos.Data`**: Contiene las entidades, DTOs y el contexto de la base de datos (`ApplicationDbContext`).
- **`Vemo.Gestion.Recursos.Helpers`**: Incluye configuraciones como perfiles de AutoMapper.
- **`Vemo.Gestion.Recursos`**: Contiene la lógica principal del sistema.

## Requisitos Previos
Antes de ejecutar el proyecto, asegúrate de tener instalados los siguientes elementos:
1. **.NET SDK**: Descarga e instala la versión compatible desde [dotnet.microsoft.com](https://dotnet.microsoft.com/).
2. **SQL Server**: Configura una instancia local o remota de SQL Server.
3. **Visual Studio Code**: Descarga e instala desde [code.visualstudio.com](https://code.visualstudio.com/).


## Cómo Empezar
1. Clona el repositorio en tu máquina local.
2. Configura la cadena de conexión en el archivo de configuración (`appsettings.json`).
3. Ejecuta las migraciones de Entity Framework para crear la base de datos.
4. Inicia el proyecto desde Visual Studio Code o tu IDE preferido.

¡Gracias por contribuir a este proyecto!
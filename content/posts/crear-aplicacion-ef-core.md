---
title: Crear Aplicación EF Core
author: Miguel Veloso
date: 2017-03-02
description: Desarrollo de una aplicación de consola para entender aspectos de configuración de EF Core.
thumbnail: posts/images-ef-core/post.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp" ]
series: [ "Entity Framework Core" ]
---


# Crear una aplicación con Entity Framework Core 1.1

En este artículo desarrollo una aplicación de consola muy sencilla usando Code First con EF Core 1.1, con el fin de entender algunos aspectos básicos del trabajo con EF Core.

Los aspectos principales que exploro son:

0. Uso de la interfaz de comandos para EF Core ([.NET Core EF CLI](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet))
0. Creación de la migración inicial trabajando con "Code First".
0. Creación de la base de datos inicial.
0. Configuración del modelo usando "Fluent API" en una clase de configuración por cada clase del modelo de dominio.
0. Carga de datos iniciales al crear la base de datos.

Finalmente se espera poder migrar una aplicación mediana (~100 clases del modelo de dominio), haciendo lo posible por mantener la experiencia de desarrollo lo más parecida posible a la de EF 6.

El repositorio con la solución completa está aquí: https://github.com/mvelosop/EFCoreApp

## Contexto

### Herramientas

* [Visual Studio 2015 Community Edition](https://www.visualstudio.com/post-download-vs/#)
* [SQL Server 2016 Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-editions-developers)
* [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### .NET Core 1.1
* [Visual Studio 2015 Tools (Preview 2)](https://go.microsoft.com/fwlink/?LinkId=827546)  
* [.NET Core 1.1 SDK - Installer x64](https://go.microsoft.com/fwlink/?LinkID=835014)  
* [.NET Core 1.1 runtime - Installer x64](https://go.microsoft.com/fwlink/?LinkID=835009)  
  (Este es necesario, además del SDK, para soportar .NET Core 1.1)

## Paso a paso

### 1) Crear la solución EFCoreApp

1. Crear una solución "blank" llamada EFCoreApp
2. Crear el solution folder "src"
3. Crear la carpeta "src" dentro de la carpeta de la solución

### 2) Crear proyecto src/EFCore.App 

1. Crear el proyecto como una "Console Application (.NET Core)"
2. Actualizar project.json a lo siguiente:

    {{< getSourceFile "EFCoreApp/src/EFCore.App/project.json" >}}

3. Salvar el archivo desde VS para actualizar todos los paquetes o, si prefiere usar la interfaz de comandos de desarrollo ([Shift]+[Alt]+[,]), ejecute **```dotnet restore```**

#### Detalles de project.json

* Paquetes de Entity Framework Core

   * **Microsoft.EntityFrameworkCore**: Paquete base
   * **Microsoft.EntityFrameworkCore.Design**: Componentes para EF Core CLI, sólo para desarrollo, por eso ```"type": "build"```
   * **Microsoft.EntityFrameworkCore.Tools.DotNet**: EF Core CLI
   * **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider

* Paquetes para manejar archivos de configuración

   * **Microsoft.Extensions.Configuration**: Paquete base
   * **Microsoft.Extensions.Configuration.Binder**: Para manejar configuraciones "strongly typed".
   * **Microsoft.Extensions.Configuration.Json**: Manejo de archivos json

* Otros

   * **System.ComponentModel.Annotations**: Annotations para los modelos
  
### 3) Agregar archivos del proyecto

#### Model/Currency.cs

La clase del modelo, Divisas en este caso.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Model/Currency.cs" >}}

#### Base/EntityTypeConfiguration.cs

Estas clases permiten manejar una clase de configuración por cada clase del modelo, para mantener el DbContext lo más sencillo posible, de forma similar a como se puede hacer con EF 6, según lo sugerido en https://github.com/aspnet/EntityFramework/issues/2805

{{< getSourceFile "EFCoreApp/src/EFCore.App/Base/EntityTypeConfiguration.cs" >}}

#### Data/CurrencyConfiguration.cs

Clase de configuración del modelo en EF. Así se mantienen fuera del modelo los detalles que corresponden a la capa de base de datos.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Data/CurrencyConfiguration.cs" >}}

#### Config/ConfigClasses.cs

Clases de configuración de la aplicación, permiten manejar la configuraciones que se carguen del archivo 
**appsettings.json** de una forma "strongly typed".

{{< getSourceFile "EFCoreApp/src/EFCore.App/Config/ConfigClasses.cs" >}}

#### Data/CommonDbContext.cs

El DbContext para la aplicación, define la vista de la base de datos a la que tiene acceso la aplicación.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Data/CommonDbContext.cs" >}}

#### Program.cs

El programa principal de la aplicación. Aquí están los métodos que crean/actualizan la base de datos y realizar la carga de datos iniciales.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Program.cs" >}}

### 4) Generar la migración inicial

Ahora es necesario generar la migración inicial que utilizará EF para crear la base de datos cuando se ejecute la aplicación por primera vez.

1. Abrir la interfaz de comandos de desarrollo

   * Hacer click sobre el nodo del proyecto EFCore.App en el explorador de la solución.
   * Pulsar [Shift]+[Alt]+[,] o Botón derecho > Open Command Line > Developer Command Prompt
   * Ejecutar **```dotnet ef```**
   * Si todo marchó bien, debe observar la una pantalla similar a la siguiente:
   
	{{<img-popup src="/posts/images-ef-core/cmd_2017-02-27_23-41-30.png" width="100%">}}

2. Crear la migración inicial

   * Ejecutar **```dotnet ef migrations add InitialCreateMigration```** en la interfaz de comandos.
   * Se puede utilizar cualquier nombre para la clase de la migración, pero recomiendo utilizar el sufijo "Migration" para evitar conflictos de nombres con otras clases de la aplicación.
   * También podemos utlizar **```dotnet ef [comando] --help```** para consultar la ayuda de cualquier comando de la interfaz.

3. Verificar que se hayan creado los archivos del la migración inicial, en la carpeta Migrations, similar a los siguientes:
 
#### Migrations/CommonDbContextModelSnapshot.cs

Este archivo es la configuración de la última versión del modelo, se utiliza al ejecutar el método DbContext.Database.EnsureCreated().

Observe que en esta clase está consolidada toda la definición de los objetos de base de datos, usando Fluent API, incluyendo los atributos utilizados en las propiedades del modelo de dominio.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Migrations/CommonDbContextModelSnapshot.cs" >}}

#### Migrations/20170227231210_InitialCreateMigration

Este archivo es el encargado de generar la migración desde la versión anterior de CommonDbContextModelSnapshot.cs, se utiliza al ejecutar el método DbContext.Database.Migrate().

Los números iniciales del nombre indican el año-mes-día-hora-minuto-segundo (yyyyMMddHHmmss) de generación de la migración.

{{< getSourceFile "EFCoreApp/src/EFCore.App/Migrations/20170227231210_InitialCreateMigration.cs" >}}

### 5) Crear archivo de configuración

#### appsettings.json

{{< getSourceFile "EFCoreApp/src/EFCore.App/appsettings.json" >}}

Verificar que project.json incluya la opción para copiar este archivo a la carpeta de salida:

```json
    "buildOptions": {
        "copyToOutput": {
            "include": [ "appsettings.json" ]
        },
        "emitEntryPoint": true
    },
```

### 6) Ejecutar la aplicación

Al ejecutar la aplicación con [Ctrl]+[F5] se debe obtener una salida similar a esta:

{{<img-popup src="/posts/images-ef-core/cmd_2017-02-28_00-47-31.png">}}

## Conclusiones

0. Identificamos los paquetes requeridos para trabajar con EF Core, diferenciando los utilizados sólo para desarrollo de los necesarios para ejecución.

0. Encontramos una forma de trabajar con clases de configuración separadas por cada clase del dominio, necesario para cualquier aplicación en la vida real.

0. Vimos cómo usar archivos de configuración dentro de la aplicación.

0. Utilizamos la interfaz de comandos para EF Core para crear la migración incial y vimos como usar la ayuda disponible.

0. Vimos como ahora EF Core mantiene el "snapshot" de la base de datos en una clase que se puede entender (a diferencia de EF 6).

Espero que sea de ayuda para alguien.

**Miguel.**

---
###### Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet


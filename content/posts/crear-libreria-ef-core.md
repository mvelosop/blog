---
title: Crear Librería EF Core
draft: true
author: Miguel Veloso
date: 2017-03-10
description: Desarrollo de una librería EF Core para entender la separación de componentes con EF Core.
thumbnail: posts/images/books-1281581_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreLib
---

# Crear una librería con Entity Framework Core 1.1

En este artículo vamos a desarrollar una versión de la misma [aplicación de consola desarrollada anteriormente (migrada a VS 2017)](posts/migrar-a-visual-studio-2017) pero separando todo el manejo de la capa de datos en la librería EFCore.Lib, para entender cómo se deben separar los paquetes y los componentes.

Los aspectos principales que exploramos son:

0. Uso de la interfaz de comandos para EF Core ([.NET Core EF CLI](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)) cuando trabajamos con librerías.
0. Creación de la migración inicial trabajando con "Code First" cuando trabajamos con librerías.

El repositorio con la solución completa está aquí:  
{{< repoUrl >}}

## Contexto

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## Proceso

Vamos a crear 

Como la interfaz de comandos para EF Core ([.NET Core EF CLI](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)) necesita la plataforma .NET Core, vamos a manejar en la solución una aplicación de consola .NET Core, que utilizaremos para generar las migraciones y ejecutar la creación y actualización de la base de datos cuando queramos hacerlo manualmente.

Para esto hay que instalar el paquete Microsoft.EntityFrameworkCore.Tools en EFCore.App

## Conclusiones

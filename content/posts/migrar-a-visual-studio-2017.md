---
title: Migrar Aplicación a VS 2017
draft: false
author: Miguel Veloso
date: 2017-03-14
description: Migración de una aplicación de consola a Visual Studio 2017 para entender el proceso.
thumbnail: posts/images/wild-geese-249134_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp", "Migración Visual Studio" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreApp
repoRelease: "2.0"
---

# Migrar una aplicación de consola a Visual Studio 2017

En este artículo vamos a migrar la aplicación que desarrollamos en el [artículo anterior](/posts/crear-aplicacion-ef-core), aprovechando el reciente lanzamiento oficial de Visual Studio 2017, para entender el proceso de migración, en especial lo referente al cambio del archivo de configuración **project.json** a **"NombreDelProjecto".csproj**.

Para este ejemplo sencillo la migración transcurre sin ningún inconveniente y al final tenemos nuestro proyecto migrado a VS 2017 y aplicación trabajando con EF Core 1.1.1.

El repositorio con la solución completa está aquí:  
{{< repoUrl >}}

## Contexto

Lo primero es instalar:

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## El Proceso

Lo primero que vemos al abrir el proyecto en VS 2017 es lo siguiente:

{{<img-popup src="/posts/images/devenv_2017-03-13_17-49-26.png">}}

Y a continuación el siguiente informe de conversión:

{{<img-popup src="/posts/images/chrome_2017-03-13_17-50-25.png">}}

Al terminar la conversión se crea una carpeta Backup con los archivos originales de la solución (.sln) y del proyecto (project.json y el .xproj):

{{<img-popup src="/posts/images/explorer_2017-03-14_10-03-33.png">}}

También podemos verificar que ahora ya no existe el archivo ```project.json``` y en su lugar está el archivo ```EFCore.App.csproj``` con el siguiente contenido:

{{< getSourceFile "src\EFCore.App\EFCore.App.csproj" >}}

En este archivo hay dos cosas notables:

1. Se actualizaron automáticamente todos los paquetes a la versión más reciente (1.1.1).
2. EL paquete **Microsoft.EntityFrameworkCore.Tools.DotNet** se "actualizó" hacia abajo a 1.0.0.  
(Al cambiar a la versión original falla porque no encuentra el archivo project.json).

En cuanto a este último, efectivamente al ejecutar ```dotnet ef``` obtenemos esto:

{{<img-popup src="/posts/images/cmd_2017-03-14_10-43-14.png">}}

Sólo para verificar el funcionamiento del CLI de EF, eliminamos el contenido de la carpeta Migrations, para volver a generar la migración, y ejecutamos ```dotnet ef migrations add InitialCreateMigration``` y se generan los siguientes archivos:

**Migrations/CommonDbContextModelSnapshot.cs**

{{< getSourceFile "src/EFCore.App/Migrations/CommonDbContextModelSnapshot.cs" >}}

Como era de esperar, este archivo es igual al de la versión anterior con la única excepción de la línea que indica:

    .HasAnnotation("ProductVersion", "1.1.1")

De la misma forma, el nuevo archivo de la migración sólo se diferencia en el nombre, por la nueva fecha de creación:

**Migrations/20170314105415_InitialCreateMigration.cs**

{{< getSourceFile "src/EFCore.App/Migrations/20170314105415_InitialCreateMigration.cs" >}}

Para verificar que todo sigue funcionando correctamente, eliminamos la base de datos EFCore.App y ejecutamos el programa con [Ctrl]+[F5] y obtenemos el resultado esperado:

{{<img-popup src="/posts/images/cmd_2017-03-14_11-07-49.png">}}

Así que, de aquí en adelante, a menos que no se pueda para algún tema en particular, seguiremos trabajando con Visual Studio 2017.

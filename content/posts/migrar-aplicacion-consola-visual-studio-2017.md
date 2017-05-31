---
title: Migrar una aplicación de consola a Visual Studio 2017
draft: false
author: Miguel Veloso
date: 2017-03-14
description: Migración de una aplicación de consola a Visual Studio 2017 para explorar el proceso.
thumbnail: posts/images/wild-geese-249134_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework Core", "CSharp", "Visual Studio 2017 Migration" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreApp
repoRelease: "2.0"
---

En este artículo vamos a migrar la aplicación que desarrollamos en el [artículo anterior](/posts/crear-aplicacion-entity-framework-core), aprovechando el reciente lanzamiento oficial de Visual Studio 2017, para explorar el proceso de migración, en especial lo referente al cambio del archivo de configuración **project.json** a **&lt;NombreDelProjecto&gt;.csproj**.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Puntos Importantes</span>

> 0. Tranformaciones del archivo project.json a &lt;Nombre-del-proyecto&gt;.csproj
> 0. Se crea un respaldo de todos los archivos migrados
> 0. Ventajas del nuevo formato simplificado del archivo &lt;Nombre-del-proyecto&gt;.csproj
> 0. Incompatibilidad de EF Core CLI 1.1 en VS 2017, hay que cambiar a 1.0 pero produce los mismos resultados.

{{< repoUrl >}}

## Contexto

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## Paso a paso

### 1 - Abrir la solución con VS 2017

En cuanto se abre la solución desarrollada con una versión anterior de Visual Studio se observa lo siguiente:

{{<image src="/posts/images/devenv_2017-03-13_17-49-26.png">}}

Y a continuación el siguiente informe de conversión:

{{<image src="/posts/images/chrome_2017-03-13_17-50-25.png">}}

En este momento ya se realizó la conversión y la solución debería compilar sin problemas.

### 2 - Explorar los cambios realizados

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Al terminar la conversión se crea una carpeta Backup con los archivos originales de la solución (.sln) y del proyecto (project.json y el .xproj):

{{<image src="/posts/images/explorer_2017-03-14_10-03-33.png">}}

También podemos verificar que ahora ya no existe el archivo ```project.json``` y en su lugar está el archivo ```EFCore.App.csproj``` con el siguiente contenido:

{{< getSourceFile "src\EFCore.App\EFCore.App.csproj" >}}

En este archivo hay tres cosas notables:

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> En el nuevo formato .csproj no es necesario incluir la lista de todos los archivos de la aplicación, lo mismo que ocurría en el project.json.

1. Se actualizaron automáticamente todos los paquetes a la versión más reciente (1.1.1).
2. EL paquete **Microsoft.EntityFrameworkCore.Tools.DotNet** se "actualizó" hacia abajo a 1.0.0.  
3. No se incluyen las interminables listas de archivos del formato anterior, ¡qué bueno! definitivamente **no voy a extrañar los conflictos durante los merge**.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> En la migración se cambia la versión de EF Core CLI a 1.0, porque la versión 1.1 no maneja el archivo .csproj

Al ejecutar ```dotnet ef``` verificamos que efectivamente se trata de la versión 1.0.0:

{{<image src="/posts/images/cmd_2017-03-14_10-43-14.png">}}

### 3 - Verificar funcionamiento de EF Core

Sólo para verificar el funcionamiento del CLI de EF, eliminamos el contenido de la carpeta Migrations, para volver a generar la migración, y ejecutamos ```dotnet ef migrations add InitialCreateMigration``` y se generan los siguientes archivos:

**Migrations/CommonDbContextModelSnapshot.cs**

{{< getSourceFile "src/EFCore.App/Migrations/CommonDbContextModelSnapshot.cs" >}}

Como era de esperar, este archivo es igual al de la versión anterior con la única excepción de la línea que indica:

    .HasAnnotation("ProductVersion", "1.1.1")

De la misma forma, el nuevo archivo de la migración sólo se diferencia en el nombre, por la nueva fecha de creación:

**Migrations/20170314105415_InitialCreateMigration.cs**

{{< getSourceFile "src/EFCore.App/Migrations/20170314105415_InitialCreateMigration.cs" >}}

Para verificar que todo sigue funcionando correctamente, eliminamos la base de datos EFCore.App y ejecutamos el programa con [Ctrl]+[F5] y obtenemos el resultado esperado:

{{<image src="/posts/images/cmd_2017-03-14_11-07-49.png">}}

Así que, de aquí en adelante, a menos que no se pueda para algún tema en particular, seguiremos trabajando con Visual Studio 2017.

---
{{< goodbye >}}

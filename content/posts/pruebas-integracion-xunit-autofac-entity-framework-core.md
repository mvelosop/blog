---
title: Pruebas de integración con xUnit, Autofac y Entity Framework Core
draft: true
author: Miguel Veloso
date: 2017-06-15
description: Reseña tamaño twitter
thumbnail: /posts/images/gears-1236578_1280.jpg
categorías: [ "Desarrollo" ]
tags: [  ]
series: [  ]
repoName: EFCoreLib
repoRelease: "1.0"
---

Breve descripción de lo que se hace en el artículo.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Puntos Importantes</span>

> 0. Inyección de dependencias con [Autofac](https://autofac.org/)
> 0. Pruebas de integración con [xUnit](https://xunit.github.io/) y [FluentAssertions](http://fluentassertions.com/)

{{< repoUrl >}}

## Contexto

En este momento me estoy dando cuenta 

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## A - Mover proyecto de pruebas en la solución

Preparando el artículo me di cuenta que las pruebas que vamos a hacer corresponden al proyecto de prueba y no directamente a las librerías, aunque, obviamente al probar la aplicación también estamos probándolas, pero me parece más adecuado renombrar la carpeta **tests** a **samples.tests**, para reflejar más claramente lo que son.

### A-1 - Renombrar carpetas en Visual Studio

Desde el explorador de la solución:

1. Cambiar nombre de la carpeta **tests** por **sample.test**
2. Cerar la solución, salvando el archivo .sln cuando Visual Studio pregunte si quiere salvar los cambios

### A-2 - Renombrar carpetas del sistema de archivos

Desde el explorador de archivos:

1. Cambiar nombre de la carpeta **tests** por **sample.test**
2. Editar el archivo Domion.Net.sln y cambiar la línea donde aparece la ruta vieja del proyecto por la ruta nueva:

```txt
Project("{FAE ... EFBC}") = "DFlow.Budget.Lib.Tests", "tests\DFlow.Budget.Lib.Tests\DFlow.Budget.Lib.Tests.csproj", ...
```

```txt
Project("{FAE ... FBC}") = "DFlow.Budget.Lib.Tests", "sample.tests\DFlow.Budget.Lib.Tests\DFlow.Budget.Lib.Tests.csproj", ...
```

### A-3 - Cambiar las referencias de los projectos

En nuestro caso, todavía no hay ninguna referencia hacia los proyectos de prueba, pero si las hubiese, sería necesario cambiarlas, editando los archivos .csproj de los proyectos correspondientes.

### A-4 - En caso de emergencia

En caso de que no logre realizar este proceso con éxito, probablemente lo mejor es borrar el proyecto por completo y crearlo de nuevo.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>

> Recuerde que al crear el proyecto sobre una carpeta de solución en Visual Studio, debe seleccionar a mano la carpeta en el sistema de archivos.

## B - 

### 1 - Paso #1

#### 1.1 - Sub-paso #1.1

1. Tarea #1
2. Tarea #2
3. Tarea #3

Para incluir un archivo fuente:

{{<getSourceFile "src\EFCore.Lib\Model\Currency.cs">}}

Para incluir una imágen:

{{<image src="/posts/images/cmd_2017-03-18_21-23-38.png">}}

---

{{< goodbye >}}

---

#### Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

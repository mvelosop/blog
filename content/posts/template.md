---
title: Plantilla (título)
draft: true
author: Miguel Veloso
date: 2017-01-01
description: Reseña tamaño twitter
thumbnail: 
tags: [  ]
repoName: EFCoreLib
repoRelease: "1.0"
toc: true
---

Breve descripción de lo que se hace en el artículo.

> {{< IMPORTANT "Puntos Importantes" >}}

> 0. Lista de los puntos más importantes del artículo

{{< repoUrl >}}

## Contexto

Texto descriptivo del contexto.

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## Paso a paso

Como en este artículo estamos separando los componentes de la aplicación, vamos a crear los archivos de programas y luego incluiremos los paquetes necesarios para poder compilar y ejecutar la solución.

En este artículo vamos a desarrollar la solución desde el principio, para apreciar los archivos .proj en su expresión más simple, por no haber pasado por la migración de un proyecto de VS 2015.

### 1 - Paso #1

#### 1.1 - Sub-paso #1.1

1. Tarea #1
2. Tarea #2
3. Tarea #3

Para incluir un archivo fuente:

{{<renderSourceFile "src\EFCore.Lib\Model\Currency.cs">}}

Para incluir una imágen:

{{<image src="/posts/images/cmd_2017-03-18_21-23-38.png">}}

## Resumen

Resumen del artículo.

---

{{< goodbye >}}

---

### Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

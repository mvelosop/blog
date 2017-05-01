---
title: Pruebas de integración con Entity Framework y SpecFlow
draft: true
author: Miguel Veloso
date: 2017-03-21
description: Uso de Specflow para probar una librería de EF Core
thumbnail: posts/images/gears-1236578_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework Core", "CSharp", "SpecFlow" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreLib
keywords: ["entity-framework", "specflow"]
---

En este artículo exploramos el uso de [SpecFlow](http://specflow.org/), una herramienta enfocada en desarrollo BDD (Behavior Driven Development), para facilitar las pruebas de integración de una librería muy sencilla.

> ### <i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Resultados principales

> 0. Lista de los resultados principales del artículo

{{< repoUrl >}}

## Contexto

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

* [SpecFlow for Visual Studio 2017](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowforVisualStudio2017)

## Paso a paso

---

Instalar SpecFlow



---

Como en este artículo estamos separando los componentes de la aplicación, vamos a crear los archivos de programas y luego incluiremos los paquetes necesarios para poder compilar y ejecutar la solución.

En este artículo vamos a desarrollar la solución desde el principio, para apreciar los archivos .proj en su expresión más simple, por no haber pasado por la migración de un proyecto de VS 2015.

### 1) Paso #1

1. Sub-paso #1
2. Sub-paso #2
3. Sub-paso #3

Para incluir un archivo fuente:

{{<getSourceFile "src\EFCore.Lib\Model\Currency.cs">}}

Para incluir una imágen:

{{<image src="/posts/images/cmd_2017-03-18_21-23-38.png">}}

---
{{< goodbye >}}

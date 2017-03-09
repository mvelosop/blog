---
title: Migrar Aplicación a VS 2017
draft: true
author: Miguel Veloso
date: 2017-03-08
description: Migración de una aplicación de consola a Visual Studio 2017 para entender el proceso.
thumbnail: posts/images/wild-geese-249134_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp", "Migración Visual Studio" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreApp
---

# Migrar una aplicación de consola a Visual Studio 2017

En este artículo voy a migrar la aplicación que desarrollamos en el [artículo anterior](/posts/crear-aplicacion-ef-core), provechando la oportunidad de que justo ayer fue el lanzamiento oficial de Visual Studio 2017, para enteder el proceso de migración, en especial lo referente al cambio del archivo de configuración **project.json** a **"NombreDelProjecto".csproj**.

{{< getSourceFile "src/EFCore.App/project.json" >}}

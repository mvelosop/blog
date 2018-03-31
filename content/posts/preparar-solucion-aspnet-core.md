---
title: Preparar una solución para ASP.NET Core
draft: false
author: Miguel Veloso
date: 2017-05-31
description: Organizar las carpetas y proyectos de una solución ASP.NET Core
thumbnail: posts/images/site-592458_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Architecture", "Project Structure" ]
series: [ "Domion" ]
repoName: Domion.Net
repoRelease: "1.0"
toc: true
---

En este artículo vamos a preparar la solución, con sus carpetas y proyectos, para comenzar con el desarrollo de [Domion - Un sistema para desarrollar aplicaciones en .NET Core](/domion).

Este es el primer artículo de la [serie](/domion), donde se prepara la estructura de carpetas y proyectos de la solución.

> {{< IMPORTANT "Puntos Importantes" >}}

> 0. Organización de las carpetas y proyectos de la solución.

> 0. Aplicación de consola para ejecutar las migraciones de EF.

> 0. Scripts de la solución para facilitar tareas repetitivas.

Al terminar el artículo deberíamos tener una idea general de la arquitectura y conocer algunos detalles de la organización de los proyectos en la solución.

En el artículo [siguiente](/posts/patron-repositorio-entity-framework-core/) trabajaremos con:

> 0. Implementación del [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html)
> 0. Facilidades para configurar modelos en [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index)
> 0. Aplicación del proceso [MDA - Model Driven Architecture](https://en.wikipedia.org/wiki/Model-driven_architecture)
> 0. Migraciones con [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index) "Code First"

{{< repoUrl >}}

## Contexto

La organización del proyecto puede ayudar a facilitar el desarrollo, al agrupar los proyectos relacionados según sus funciones.

En el caso de desarrollo de librerías es especialmente útil tener una carpeta de ejemplos, donde se pueda verificar el funcionamiento en situaciones similares a la realidad, entendiendo que por razones pedagógicas, los ejemplos deben ser fundamentalmente sencillos.

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones)

* [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)

* [.NET Core 1.1.2 - x64 Installer](https://download.microsoft.com/download/D/0/2/D028801E-0802-43C8-9F9F-C7DB0A39B344/dotnet-win-x64.1.1.2.exe)
* [.NET Core SDK 1.0.4 - x64 Installer](https://download.microsoft.com/download/B/9/F/B9F1AF57-C14A-4670-9973-CDF47209B5BF/dotnet-dev-win-x64.1.0.4.exe)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones)

## Paso a paso

La estructura está inspirada en este gist de David Fowler: https://gist.github.com/davidfowl/ed7564297c61fe9ab814, más otros comentarios que he ido recogiendo por ahí, en especial de un [podcast en DotNetRocks con Scott Allen](http://www.dotnetrocks.com/?show=1405).

### 1 - Crear solución en blanco

{{<image src="/posts/images/devenv_2017-05-31_09-07-32.png">}}

Como se trata de una solución en blanco no es importante el framework seleccionado, en este caso ".NET Framework 4.6.2"

Es importante marcar la opción para crear el repositorio Git

### 2 - Crear proyecto con fuentes principales de la solución

#### 2.1 - Crear el solution folder "src" en Visual Studio

> {{< IMPORTANT "Importante" >}}

> Las carpetas de la solución de Visual Studio son contenedores lógicos que sólo existen en Visual Estudio.

{{<image src="/posts/images/2017-05-31_09-17-55.png">}}

Esta es una carpeta lógica y realmente no existe en el sistema operativo. Si agregamos un ítem en esta carpeta, realmente se guardará en la carpeta raíz de la solución.

#### 2.2 - Crear la carpeta "src" en el sistema de archivos 

Para crear la carpeta se puede hacer desde el explorador de windows, con **[botón derecho > Open Folder in File Explorer]**, pero si está instalada la extensión [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017) (altamente recomendada), bastará con pulsar **[Shift]+[Alt]+[,]** para abrir una ventana de comandos.

> {{< IMPORTANT "Importante" >}}

> Es necesario realizar los dos pasos anteriores, es decir, crear **la carpeta "src"** en el sistema de archivos y **el solution folder "src"** en Visual Studio. 

> Realmente son dos objetos distintos que no están relacionados, pero al tener el mismo nombre "parecen" lo mismo.

### 3 - Crear resto de carpetas de la solución

Según lo indicado en el paso anterior, crear las siguientes carpetas, tanto en la solución con el sistema de archivos:

1. **"samples"** - Aplicaciones de ejemplo para probar el sistema
2. **"scripts"** - Scripts para crear migraciones
3. **"tests"** - Proyectos de pruebas del sistema

Al final el explorador de la solución se debe ver así:

{{<image src="/posts/images/devenv_2017-05-31_11-42-47.png">}}

Y el explorador de Windows así:

{{<image src="/posts/images/explorer_2017-05-31_11-49-48.png">}}

### 4 - Crear los proyectos principales de la solución

En este caso los proyectos principales son las librerías base de [Domion](/domion) y las aplicaciones serán ejemplos que estarán en la carpeta "samples".

Las librerías tienen dos proyectos principales:

1. **Domion.Core** - Donde estarán los componentes relacionados con la capa del modelo de las aplicaciones y
2. **Domion.Lib** - Donde están los componentes relacionados con las capas de datos y servicios de las aplicaciones.

#### 4.1 - Crear proyecto "src\Domion.Core"

> {{< IMPORTANT "Importante" >}}

> Es importante agregar la carpeta "src" (o la que corresponda) en la ruta de creación del proyecto.

Agregar un nuevo proyecto tipo **Class Library (.NET Core)** sobre la carpeta "src"

{{<image src="/posts/images/devenv_2017-05-31_12-02-38.png">}}

Es necesario agregar **"\src"** a mano en la ruta, en caso contrario el proyecto se guardará en la raíz de la solución.

También puede seleccionar la carpeta con el botón "Browse...".

#### 4.2 - Crear proyecto "src\Domion.Lib"

Repetir el paso anterior para el proyecto **"Domion.Lib"** de tipo **Class Library (.NET Core)** dentro de la carpeta **"src"**

En ambos casos podemos eliminar las clases iniciales "Class1.cs" de los proyectos.

### 5 - Crear proyectos de ejemplo

Como se mencionó en el artículo inicial de [Domion](/domion), la aplicación de ejemplo será un "sistema" de flujo de caja personal.

Esta aplicación nos interesa trabajar fundamentalmente la estructura del proyecto, más que la cantidad de funciones manejadas, por eso inicialmente sólo manejaremos dos "micro módulos":

1. **Presupuesto** - Básicamente un clasificador de gastos.
2. **Transacciones** - Para registrar los bancos y las transacciones.

Adicionalmente, cada módulo tendrá dos capas:

1. **Core** - Para la capa del modelo, con las clases del dominio, y las interfaces de servicios.
2. **Lib** - Para las capas de datos y servicios básicos de la aplicación. En la capa de datos usaremos Entity Framework y para los servicios básicos utilizaremos el [Patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html).

Además, La aplicación se llamará "DFlow", por lo tanto, siguiendo el mismo procedimiento anterior, vamos a crear los siguientes proyectos:

1. **samples\DFlow.Budget.Core** - Class Library (.NET Core)
1. **samples\DFlow.Budget.Lib** - Class Library (.NET Core)
1. **samples\DFlow.Transactions.Core** - Class Library (.NET Core)
1. **samples\DFlow.Transactions.Lib** - Class Library (.NET Core)

> {{< IMPORTANT "Importante" >}}

> Para poder crear las migraciones de Entity Framework (EF) necesitamos crear una aplicación de consola, donde luego instalaremos el "tooling" para EF.

Finalmente, también vamos a necesitar una aplicación de consola para crear las migraciones, según una de las opciones para ello, indicadas en la página de la [interfaz de comandos .NET EF Core (.NET Core EF CLI)](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#net-standard-limitation), por una limitación de .NET Standard que no permite (al 31/05/2017) ejecutar ```dotnet``` en un proyecto "Class Library".

Así que vamos a crear el siguiente proyecto para poder generar las migraciones de base de datos:

**samples\DFlow.CLI** - Console App (.NET Core)

### 6 - Crear proyectos de pruebas

Uno de los objetivos de esta serie es trabajar desde el principio con pruebas automatizadas, que permitan verificar rápidamente el funcionamiento de la aplicación, para eventualmente configurar un proceso de [Continuous Delivery](https://en.wikipedia.org/wiki/Continuous_delivery).

Como inicialmente vamos a trabajar con código generado con la [metodología MDA](/domion#mda-model-driven-architecture), no vamos a comenzar estrictamente con [TDD](/domion#tdd-test-driven-development), sino con pruebas de integración, por lo por ahora sólo crearemos los siguientes proyectos:

1. **tests\DFlow.Budget.Lib.Tests** - xUnit Test Project (.NET Core)
1. **tests\DFlow.Transactions.Lib.Tests** - xUnit Test Project (.NET Core)

### 7 - Crear scripts para generar migraciones

> {{< IMPORTANT "Importante" >}}

> Los scripts facilitan la ejecución de comandos comunes durante del desarrollo como la creación de las migraciones.

Para incluir los scripts en la carpeta se debe hacer lo siguiente:

#### 7.1 - Crear los scripts en la carpeta "scripts" desde el explorador de Windows

Es importante crear estos archivos desde el explorador de Windows, para ubicarlos donde queremos, no se puede hacer lo mismo que con los proyectos.

#### 7.2 - Script para agregar migraciones

{{<getSourceFile "scripts\add-migration.cmd">}}

#### 7.3 - Script para eliminar la última migración

{{<getSourceFile "scripts\remove-migration.cmd">}}

Cuando se copien estos scripts a otros proyectos, es necesario indicar la dirección del proyecto .CLI en la variable **cliProjectDir**, como una ruta relativa desde la carpeta scripts, en ambos scripts:

```cmd
set cliProjectDir="..\samples\DFlow.CLI"
```

#### 7.2 - Incluir los scripts en la solución

1. Ejecutar **[Botón Derecho > Add > Existing Item...]** sobre la carpeta scripts en el **explorador de la solución** y buscar los scripts en el diálogo de selección, o
1. Arrastrar los archivos **desde el explorador de Windows** hasta la carpeta "scripts" en el explorador de la solución.

### 8 - Verificar estructura del proyecto

Al terminar este procedimiento el explorador de la solución se debe ver así:

{{<image src="/posts/images/devenv_2017-05-31_15-15-11.png">}}

Y el explorador de Windows se debe ver así:

{{<image src="/posts/images/explorer_2017-05-31_15-17-39.png">}}

> {{< IMPORTANT "Importante" >}}

> Para eliminar un proyecto de la solución sin causar problemas, se debe eliminar primero desde el explorador de Visual Studio.

Si quedó algún proyecto en la raíz de la solución, se debe:

1. Eliminar el proyecto de la solución en el explorador de la solución (Visual Studio).
1. Eliminar la carpeta en el explorador de Windows.
1. Crear el proyecto nuevamente

Con esto ya tenemos el proyecto preparado para comenzar con el desarrollo.

---

Para comenzar el desarrollo podemos ir de una vez al artículo siguiente: [Patrón de repositorio con Entity Framework Core](/posts/patron-repositorio-entity-framework-core/)

---

{{< goodbye >}}

---
### Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

---
title: Patrón de repositorio con Entity Framework Core
draft: false
author: Miguel Veloso
date: 2017-06-11
description: Implementar el patrón de repositorio para el módulo inicial de las pruebas de Domion
thumbnail: /posts/images/stairs-195924_1280.jpg
categorías: [ "Desarrollo" ]
tags: [  ]
series: [ "Domion" ]
repoName: Domion.Net
repoRelease: "2.0"
---

Este es el segundo artículo de la serie [Domion - Un sistema para desarrollar aplicaciones en .NET Core](/domion). En el [artículo anterior](/posts/preparar-solucion-aspnet-core/) creamos la solución para definir la estructura general, con los proyectos principales, aunque sin programas.

En esta oportunidad vamos a implementar el [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html) con [Entity Framework Core (EF Core)](https://docs.microsoft.com/en-us/ef/core/index) en las librerías, para aplicarlo en el backend del primer módulo de la aplicación.

El patrón de repositorio es uno de los elementos principales de la arquitectura que estaremos utilizando en esta serie y por eso comenzamos por ahí.

Este artículo es un poco largo porque hace falta implementar una parte importante de la infraestructura básica. También se incluye en la aplicación de consola lo mínimo necesario para verificar que todo esté funcionando bien, sin meternos todavía en pruebas de integración, las cuales trataremos en el próximo artículo.

Los puntos más importantes que cubriremos son:

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Puntos Importantes</span>

> 0. Implementación del [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html)
> 0. Facilidades para configurar modelos en [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index)
> 0. Aplicación del proceso [MDA - Model Driven Architecture](https://en.wikipedia.org/wiki/Model-driven_architecture)
> 0. Migraciones con [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index) "Code First"

Al terminar el artículo deberíamos tener una buena visión general de la arquitectura y conocer algunos detalles de los elementos principales.

En el artículo siguiente trabajaremos con:

> 0. Pruebas de integración con [xUnit](https://xunit.github.io/) y [FluentAssertions](http://fluentassertions.com/)
> 0. Inyección de dependencias con [Autofac](https://autofac.org/)

{{< repoUrl >}}

## Contexto

Para desarrollar aplicaciones de tamaño significativo es importante tener una forma de dividir el trabajo, de forma que varios equipos puedan trabajar en paralelo.

El primer paso para esto es la separación por capas, típicamente Modelo, Datos y Servicios y en segundo lugar la separación en áreas funcionales o módulos.

En este artículo vamos a explorar principalmente el primero de estos pasos, en especial con la implementación del patrón de repositorio y también vamos a explorar algo de la separación por áreas funcionales, aunque nuestro primer módulo sólo va a tener por ahora una clase en el modelo de dominio.

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)

* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

* [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

* [.NET Core 1.1.2 - x64 Installer](https://download.microsoft.com/download/D/0/2/D028801E-0802-43C8-9F9F-C7DB0A39B344/dotnet-win-x64.1.1.2.exe)
* [.NET Core SDK 1.0.4 - x64 Installer](https://download.microsoft.com/download/B/9/F/B9F1AF57-C14A-4670-9973-CDF47209B5BF/dotnet-dev-win-x64.1.0.4.exe)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones)

### Recomendación general

Durante el trabajo diario de desarrollo me ha resultado muy conveniente usar una base de datos local con [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) y el [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms), en vez de las herramientas integradas en Visual Studio.

Siento que es mucho más fácil cambiar el contexto de trabajo a otra aplicación con un simple [Ctrl]+[Alt] que tener que buscar el servidor de SQL Server o LocalDb con el explorador de servidores en Visual Studio usando el ratón.

## A - Paso a paso - Patrón de repositorio

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> El [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html) nos ofrece una abstracción del DbContext de EF Core, donde podemos agregar procesamiento y validaciones adicionales. También puede facilitar la realización de pruebas sin tener que involucrar al DbContext.

Esta implementación del patrón de repositorio se apoya en la funcionalidad del [DbContext](https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext) de EF Core, que mantiene en memoria una colección de las entidades que han sido modificadas (en el ChangeTracker), antes de enviar los cambios a la base de datos para salvarlos.

### A-1 - Repositorio genérico

Esta implementación del [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html) se realiza con dos interfaces y un repositorio genérico, que luego se complementan con una interfaz y una clase específica identificada como "Manager" y que llamaremos en forma genérica como EntityManager.

Todo el acceso al DbContext se realiza a través del EntityManager específico, para que se pueda adaptar a fácilmente a las necesidades particulares de cada situación, ocultando o implementando los métodos necesarios.

Las interfaces están en el proyecto **Domion.Core** y el repositorio genérico en **Domion.Lib**.

#### A-1.1 - IQueryManager - Interfaz genérica para queries

Esta interfaz define lo mínimo que debe implementar un EntityManager, ya que es lo que permite realizar consultas generales.

También es posible, aunque menos frecuente, implementar sólo consultas específicas y en ese caso bastaría con que el EntityManager no implemente la interfaz genérica.

Esta interfaz genérica nos permite implementar [Extension Methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods) de uso común, aplicables a todos los "Managers", sin necesidad de implementarlos para cada EntityManager.

{{<getSourceFile "src\Domion.Core\Services\IQueryManager.cs">}}

#### A-1.2 - IEntityManager - Interfaz genérica para DbContext.Find()

La implementación de esta interfaz nos permite acceder al método **Find** de los DbContext, si decidimos exponerlo a través del EntityManager.

{{<getSourceFile "src\Domion.Core\Services\IEntityManager.cs">}}

#### A-1.3 - BaseRepository - Repositorio genérico

Esta es la implementación base del repositorio genérico, está declarada como una clase abstracta, así que cada EntityManager específico debe heredar de ésta y entonces decidir que métodos cambiar u ocultar o, incluso, eliminando alguna de las interfaces de la declaración.

{{<getSourceFile "src\Domion.Lib\Data\BaseRepository.cs">}}

#### A-1.4 - IQueryManagerExtensions - Extensiones para el IQueryManager

Estos extension methods agregan funcionalidad de uso común con los IQueryable, directamente al IQueryManager, sin necesidad de implementarlos en cada EntityManager.

{{<getSourceFile "src\Domion.Lib\Extensions\IQueryManagerExtensions.cs">}}

### A-2 - Extensiones para configuración de los modelos en EF Core

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Las extensiones que se muestran a continuación facilitan la configuración de modelos grandes en EF Core.

Actualmente (EF Core 1.1.2), para configurar los modelos usando el Fluent API, es necesario hacerlo en un override del método OnModelCreation del DbContext, pero esto resulta poco práctico para aplicaciones de cualquier tamaño significativo, ya que el DbContext se puede extender más allá de lo razonable.

Por eso, siguiendo una de las sugerencias en https://github.com/aspnet/EntityFramework/issues/2805, incluimos las siguientes clases:

#### A-2.1 - EntityTypeConfiguration - Configuración genérica por clase de dominio

El método Map de esta clase abstract recibe un EntityTypeBuilder<TEntity> con el que se puede refactorizar allí toda la configuración de una clase del modelo.

En el punto [B-3.1](#b-3-1-budgetclassconfiguration-configuración-del-modelo-para-ef-core) podemos ver cómo se utiliza esta clase para manejar la clase de configuración.

{{<getSourceFile "src\Domion.Lib\Data\EntityTypeConfiguration.cs">}}

#### A-2.2 - ModelBuilderExtensions - Extensión de ModelBuilder 

Este extension method permite invocar la configuración de una clase desde el DbContext.

{{<getSourceFile "src\Domion.Lib\Data\ModelBuilderExtensions.cs">}}

Con esto la configuración de cada clase del modelo queda reducida a una línea en el DbContext, por ejemplo:

```cs
    modelBuilder.AddConfiguration(new BudgetClassConfiguration());
```

Estas clases se encuentran en el proyecto **Domion.Lib**

### A-3 - Incluir referencias y compilar

Incluir las siguientes dependencias en **Domion.Lib**:

1. **Domion.Core** (Referencia)
2. **Microsoft.EntityFrameworkCore - 1.1.2** (Nuget)

Estos son los componentes básicos de la infraestructura y en este momento se debería poder compilar la solución sin errores.

## B - Paso a paso - MDA - Componentes básicos de la aplicación

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> El enfoque de diseño [MDA - Model Driven Architecture](https://en.wikipedia.org/wiki/Model-driven_architecture) permite generar cantidades importantes de código a partir de modelos de alto nivel y esto contribuye tanto con la productividad del equipo de desarrollo como con la calidad y facilidad de mantenimiento de los productos.

Como mencionamos en el artículo inicial, [la aplicación de ejemplo será un "sistema" de flujo de caja personal](/domion/#alcance-de-la-aplicación) y el "módulo" inicial va a tener, por ahora, una sola entidad: **BudgetClass**.

Vamos a aclarar que el tamaño de esta aplicación modelo no justifica, por sí mismo, el uso de la estructura de la solución que estamos desarrollando.

Sin embargo, como lo que buscamos es lograr una estructura flexible, que facilite el desarrollo de aplicaciones grandes y la división del trabajo entre varios equipos, entonces nos enfocamos en una aplicación muy sencilla, para poder dedicar el esfuerzo cognitivo en la estructura y no en el contenido.

### B-1 - Modelos de la aplicación

También mecionamos en ese artículo que íbamos a aplicar el enfoque [MDA - Model Driven Architecture](https://en.wikipedia.org/wiki/Model-driven_architecture) usando [Enterprise Architect](http://www.sparxsystems.com/products/ea/). 

A continuación, mostramos los modelos desarrollados con este enfoque y, como se puede observar, no son los modelos típicos de UML, con la excepción del modelo de dominio, sino que forman parte del [DSL - Domain Specific Language](https://en.wikipedia.org/wiki/Domain-specific_language) diseñado específicamente para facilitar el desarrollo de aplicaciones con Domion.

#### B-1.1 - Modelo de Dominio

Este es el "modelo de dominio" de la aplicación por ahora.

Estamos de acuerdo que "Modelo de Dominio" le queda grande a esto, pero ya lo iremos ampliando durante el desarrollo de la serie.

{{<image src="/posts/images/EA_2017-06-09_12-04-33.png">}}

#### B-1.2 - Modelo de "Datos"

Este no es modelo de base de datos que podríamos esperar, sino más bien un modelo de la capa de datos, que indica que la clase **BudgetDbContext**, con estereotipo _dbcontext_, debe tener una propiedad tipo DbSet<BudgetClass>.

{{<image src="/posts/images/EA_2017-06-09_12-40-09.png">}}

Para esta serie vamos a trabajar con la opción "Code First" de Entity Framework, que se encarga de crear la base de datos a partir de las clases del modelo y las configuraciones, a través de las migraciones, por lo que no tenemos que preocuparnos directamente de la base de datos.

Veremos las migraciones más adelante en este mismo artículo.

#### B-1.3 - Modelo de "Servicios"

Este modelo indica que la clase **BudgetClassManager**, con estereotipo _entity-manager_, depende de las clases **BudgetClass** con estereotipo _entity-model_ y de **BudgetDbContext** con estereotipo _dbcontext_.

Esta es la forma de especificar en Domion que el **BudgetClassManager** utiliza el **BudgetDbContext** para gestionar el acceso a los objetos **BudgetClass**.

{{<image src="/posts/images/EA_2017-06-09_12-42-48.png">}}

#### B-1.4 - Generación de código

A partir de esos modelos, usando las plantillas de transformación y generación de Domion, generamos los componentes que se muestran a continuación.

Este proceso no se detalla en el artículo, sólo se muestra el resultado final.

### B-2 - Componentes en DFlow.Budget.Core

#### B-2.1 - BudgetClass - Clasificación de conceptos del presupuesto

Esta es la clase "principal" (la única por ahora) del modelo de dominio. 

Los atributos **[Required]** y **[MaxLength(100)]**, así como el comentario **// Key data ---**, son el resultado de especificaciones particulares que se hacen en el modelo en Enterprise Architect. El comentario **Key data** nos indica que los valores de esa propiedad se deben manejar como valores únicos en la base de datos.

Aunque los atributos indicados realmente pertenecen a la capa de datos y no a la capa del modelo de dominio, donde estamos, me parece que es útil tenerlos aquí como referencia al implementar las pantallas.

{{<getSourceFile "samples\DFlow.Budget.Core\Model\BudgetClass.cs">}}

#### B-2.2 - TransactionType - Tipo de transacción

Esto es simplemente un enum convencional.

{{<getSourceFile "samples\DFlow.Budget.Core\Model\TransactionType.cs">}}

#### B-2.3 - IBudgetClassManager - Interfaz del EntityManager para BudgetClass

Esta es la interfaz específica para el BudgetClassManager, está declarada en el proyecto .Core para facilitar su uso desde las clases de dominio si hace falta.

En caso de ser necesario utilizar los managers desde la capa del modelo de dominio, se utilizará el patrón [Service Locator](https://martinfowler.com/articles/injection.html#UsingAServiceLocator) con la clase **System.Web.Mvc.DependencyResolver**, para resolver las implementaciones de los IEntityManager necesarios, a través de un contenedor de inyección de dependencias, configurado convenientemente.

Esta interfaz se puede modificar tanto como sea necesario, por ejemplo, se podría eliminar la referencia a IQueryManager<BudgetClass> y ocultar en el BudgetClassManager los métodos del repositorio base, para entonces implementar métodos de consulta específicos.

{{<getSourceFile "samples\DFlow.Budget.Core\Services\IBudgetClassManager.cs">}}

### B-3 - Componentes en DFlow.Budget.Lib

#### B-3.1 - BudgetClassConfiguration - Configuración del modelo para EF Core

Esta es la clase de configuración del modelo de datos para BudgetClass. Aquí se pueden apreciar claramente los elementos relacionados con la base de datos.

En esta clase no están los elementos relativos al tamaño de los campos o si son requeridos o no, porque ya están incluidos como atributos en la clase del modelo, como se mostró anteriormente.

Vamos a destacar un elemento importante de la configuración, como uso de un **schema de base de datos** asociado a cada DbContext, como un modo de separar las áreas funcionales en la base de datos. Esto, además, nos facilitará el desarrollo de aplicaciones grandes, a la hora de distribuir el trabajo entre varios equipos y compartir el acceso a una base de datos desde varios DbContext.

{{<getSourceFile "samples\DFlow.Budget.Lib\Data\BudgetClassConfiguration.cs">}}

#### B-3.2 - BudgetDbContext - DbContext para el módulo

El DbContext es el corazón de [Entity Framework](https://docs.microsoft.com/en-us/ef/).

El [DbContext](https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext) es una implementación combinada de los patrones [Unit of Work](https://martinfowler.com/eaaCatalog/unitOfWork.html) y [Repository](https://martinfowler.com/eaaCatalog/repository.html).

También lo podemos ver como una ventana a la base de datos (con una interfaz de objetos) que nos facilita el acceso a los objetos necesarios.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Es un error común manejar en un único DbContext todo el modelo de dominio de una aplicación, lo ideal es dividir el modelo en un DbContext por módulo o por área funcional.
>
> En un artículo próximo veremos cómo manejar múltiples DbContext compartiendo la misma base de datos.

Además, el DbContext también facilita la implementación de un [Bounded Context](https://martinfowler.com/bliki/BoundedContext.html), que es uno de los elementos pricipales del [Domain Driven Design (DDD)](https://domainlanguage.com/ddd/).

{{<getSourceFile "samples\DFlow.Budget.Lib\Data\BudgetDbContext.cs">}}

#### B-3.3 - BudgetClassManager - EntityManager para las clasificaciones

Esta es la implementación del EntityManager para BudgetClass. Es la responsable de gestionar el acceso al DbContext, en especial en cuanto a las validaciones relacionadas con incluir o eliminar objetos en el repositorio, por ejemplo, evitar elementos duplicados, o eliminación de objetos referenciados por otros.

Independientemente de que esas validaciones estén reforzadas a nivel de la base de datos, por ejemplo, usando Foreign Keys o índices únicos, esta clase permite detectar estos casos antes de que se levante una excepción por una validación de la base de datos y mostrar un mensaje controlado al usuario.

{{<getSourceFile "samples\DFlow.Budget.Lib\Services\BudgetClassManager.cs">}}

### B-4 - Incluir dependencias y compilar

Incluir las siguientes dependencias en **DFlow.Budget.Core**:

1. **Domion.Core** (Referencia)

Incluir las siguientes dependencias en **DFlow.Budget.Core**:

1. **Domion.Lib** (Referencia)
2. **Microsoft.EntityFrameworkCore.SqlServer - 1.1.2** (Nuget)
2. **NLog - 5.0.0-beta7** (Nuget, con soporte para .NET Core)

Estos son los componentes básicos de la aplicación y en este momento se debería poder compilar la solución sin errores.

## C - Paso a paso - Migraciones

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Las migraciones generadas por Entity Framework cuando se trabaja con la modalidad "Code First", permiten generar y actualizar la base de datos de forma automática y sin necesidad de dedicarle mucho tiempo.

En esta fase vamos a crear la migración inicial, con la que se creará la base de datos al ejecutar la aplicación.

### C-1 - Crear proyecto de configuración

Vamos a crear un proyecto para manejar los temas de configuración del módulo, aunque por ahora sólo vamos a incluir lo referente a la configuración de la base de datos.

#### C-1.1 - Crear el proyecto "samples\DFlow.Budget.Setup"

1. Crear el proyecto tipo **Class Library (.NET Core)** en la carpeta "samples"

#### C-1.2 - Agregar clase "samples\DFlow.Budget.Setup\BudgetDbSetupHelper.cs"

{{<getSourceFile "samples\DFlow.Budget.Setup\BudgetDbSetupHelper.cs">}}

#### C-1.3 - Instalar dependencias

1. **DFlow.Budget.Lib** (Referencia)
2. **Microsoft.EntityFrameworkCore - 1.1.2** (Nuget)

### C-2 - Instalar "Tooling" de Entity Framework en DFlow.CLI

#### C-2.1 - Instalar paquete de tooling

El paquete de tooling es el encargado de crear las migraciones y aplicar actualizaciones en las bases de datos en el ambiente de desarrollo.

Para efectos de este artículo trabajaremos con la versión CLI (Command Line Interface) de las herramientas (**Microsoft.EntityFrameworkCore.Tools.DotNet**), para realizar las operaciones desde la línea de comandos. También se podría instalar la versión de PowerShell (**Microsoft.EntityFrameworkCore.Tools**) que se ejecuta desde la consola del Package Manager, es sólo un asunto de preferencias personales.

Para habilitar el tooling es necesario instalar el paquete **Microsoft.EntityFrameworkCore.Tools.DotNet** en **DFlow.CLI**, pero este es un tipo de paquete **"DotNetCliTool"**, que no se puede instalar como un NuGet cualquiera.

Entonces, siguiendo lo indicado en la página de la [interfaz de comandos .NET EF Core (.NET Core EF CLI)](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#workaround-1---use-an-app-as-the-startup-project), hay que editar el archivo .csproj del proyecto (Solution Explorer, sobre DFlow.CLI: **[Botón derecho > Edit DFlow.CLI.csproj]**) y agregar las líneas siguientes:

```xml
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.1" />
  </ItemGroup>
```

Al salvar el archivo se debe instalar el paquete automáticamente. En caso contrario utilice el comando ```dotnet restore``` desde la interfaz de comandos en el proyecto **DFlow.CLI**.

#### C-2.2 - Instalar dependencias

1. **DFlow.Budget.Lib** (Referencia)
1. **Microsoft.EntityFrameworkCore.Core - 1.1.2**. (NuGet)
1. **Microsoft.EntityFrameworkCore.Design - 1.1.2**. (NuGet)
1. **Microsoft.EntityFrameworkCore.SqlServer - 1.1.2**. (NuGet)

#### C-2.3 - Verificar instalación del tooling

Para verificar si el tooling quedó instalado correctamente, basta con abrir una ventana de comandos sobre el proyecto DFlow.CLI y ejecutar el comando ```dotnet ef```. Si todo está bien se debe ver la siguiente pantalla.

{{<image src="/posts/images/cmd_2017-06-09_19-40-25.png">}}

### C-3 - Crear migración inicial

#### C-3.1 - Ejecutar script de migración

1. Abrir una ventana de comandos en la carpeta "scripts" de la solución (con [Alt]+[Shift]+[,] sobre el archivo del script, si están instaladas las [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017))
2. Ejecutar el script **add-migration**
3. Indicar los siguientes parámetros:
   * **DFlow.Budget.Lib**
   * **BudgetDbContext**
   * **Create**

Si todo está bien, se debe obtener una pantalla como esta:

{{<image src="/posts/images/cmd_2017-06-09_21-45-36.png">}}

Y se debe obtener un archivo como este en la carpeta **Migrations** de **DFlow.Budget.Lib**.

{{<getSourceFile "samples\DFlow.Budget.Lib\Migrations\20170609203746_CreateMigration_BudgetDbContext.cs">}}

## D - Paso a Paso - Ejecutar la aplicación

Originalmente había pensado incluir el proyecto de pruebas de integración e inyección de dependencias con [Autofac](https://autofac.org/), pero como el artículo ya está demasiado largo, sólo vamos a hacer una corrida rápida desde la aplicación de consola en DFlow.CLI.

### D-1 - Preparar DFlow.CLI para ejecutar la aplicación

#### D-1.1 - Modificar Program.cs

{{<getSourceFile "samples\DFlow.CLI\Program.cs">}}

#### D-1.2 - Activar DFlow.CLI como Startup project

* Sobre el proyecto **DFlow.Cli**: **[Botón derecho > Set as StartUp Project]**

#### D-1.3 - Ejecutar la aplicación

Al ejecutar la aplicación por primera vez debe obtener una pantalla como esta:

{{<image src="/posts/images/dotnet_2017-06-11_13-03-36.png">}}

Y esta otra al ejecutarla por segunda vez:

{{<image src="/posts/images/dotnet_2017-06-11_12-59-30.png">}}

Y de esta forma verificamos que la aplicación está funcionando y terminamos el artículo, ¡finalmente!

## Resumen

En este artículo exploramos una implementación del patrón de repositorio y la utilizamos para poner a funcionar el backend del primer módulo de la aplicación de presupuesto personal.

También tuvimos una visión general de los resultados de usar el enfoque MDA - Model Driven Architecture y como, gracias al enfoque Code First de Entity Framework, pasamos a tener una base de datos completamente funcional, con muy poco esfuerzo y casi sin tener que pensar en ello.

---

{{< goodbye >}}

---

#### Enlaces relacionados

**DbContext**  
https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext

**Domain Driven Design**  
https://domainlanguage.com/ddd/

**DSL - Domain Specific Language**  
https://en.wikipedia.org/wiki/Domain-specific_language

**Enterprise Architect**  
http://www.sparxsystems.com/products/ea/

**Entity Framework Core**  
https://docs.microsoft.com/en-us/ef/core/index

**Extension Methods**  
https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

**MDA - Model Driven Architecture**  
https://en.wikipedia.org/wiki/Model-driven_architecture

**Patrón Bounded Context**  
https://martinfowler.com/bliki/BoundedContext.html

**Patrón de repositorio**  
https://martinfowler.com/eaaCatalog/repository.html

**Patrón Service Locator**  
https://martinfowler.com/articles/injection.html#UsingAServiceLocator

**Patrón Unit of Work**  
https://martinfowler.com/eaaCatalog/unitOfWork.html

---
title: Pruebas de integración con xUnit y Entity Framework Core
draft: false
author: Miguel Veloso
date: 2017-06-20
description: Desarrollar las primeras pruebas de integración con xUnit
thumbnail: /posts/images/gears-1236578_1280.jpg
tags: [ "Entity Framework Core", "xUnit", "Integration Tests", "EF Code First", "Domion" ]
repoName: Domion.Net
repoRelease: "3.0"
toc: true
---

Este es el tercer artículo de la serie [Domion - Un sistema para desarrollar aplicaciones en .NET Core](/domion). En el [artículo anterior](/posts/patron-repositorio-entity-framework-core/) desarrollamos los componentes iniciales de la aplicación modelo, haciendo énfasis en el [patrón de repositorio](https://martinfowler.com/eaaCatalog/repository.html) con [Entity Framework Core (EF Core)](https://docs.microsoft.com/en-us/ef/core/index), que implementamos y denominamos, de forma general, como **EntityManagers**.

En esta ocasión nos vamos en enfocar en desarrollar unas pruebas básicas de integración para los EntityManagers, no sólo con el objetivo de verificar su funcionamiento, sino también para que sirvan como los bloques básicos para construir las pruebas de aceptación con [SpecFlow](http://specflow.org/), siguiendo el enfoque [BDD](https://en.wikipedia.org/wiki/Behavior-driven_development), que veremos más adelante en la serie.

También aprovecharemos luego este aprendizaje para desarrollar nuevas plantillas para generar este tipo de componentes y seguir ampliando el alcance de [Domion](/domion).

> {{< IMPORTANT "Puntos Importantes" >}}

> 0. Pruebas de integración con [xUnit](https://xunit.github.io/) y [FluentAssertions](http://fluentassertions.com/).

> 0. Mover o renombrar proyectos de una solución en VS 2017.

> 0. Cambiar el TargetFramework en proyectos .NET Core.

> 0. Entender la forma correcta de hacer pruebas de integración sobre un DbContext.

> 0. Aplicar el patrón de pruebas Arrange / Act / Assert.

> 0. Refactorización usando delegados.

> 0. Estandarizar pruebas típicas como paso previo a generarlas usando [MDA](https://en.wikipedia.org/wiki/Model-driven_architecture).

Al terminar el artículo tendremos una buena estructura para organizar las pruebas de integración y podremos apreciar las ventajas de trabajar con el enfoque "Code First" en [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/index).

Para el artículo siguiente tenemos planeado trabajar con:

> 0. Inyección de dependencias usando [AutoFac](https://autofac.org/).

{{< repoUrl >}}

> {{< IMPORTANT "Importante" >}}

> Si quiere realizar el tutorial paso a paso, le recomiendo que comience con los [fuentes del release 2.0 (.zip)] (https://github.com/mvelosop/Domion.Net/archive/2.0.zip)

> El tiempo estimado para realizar este tutorial es de aproximadamente una hora.

## Contexto

Después de varios años desarrollando productos (que requieren mantenimiento) y haber pagado el precio de haber sido un poco laxo con el tema de las pruebas, he llegado a la conclusión de que las pruebas automatizadas son una de las mejores inversiones que podemos realizar para mejorar la productividad de la empresa, cuando hay que dar mantenimiento a los programas y no se factura por hora.

Esto puede parecer contradictorio, ya que las pruebas requieren tiempo adicional para desarrollarlas y luego para mantenerlas, pero, en mi humilde opinión, sólo hace falta tener un enfoque pragmático en las pruebas.

En mi segunda iteración profesional en desarrollo de software, usando .NET, lo que me ha dado mejor resultado valor/costo, ha sido trabajar casi todas las historias de usuario partiendo desde las pruebas de aceptación usando [BDD](https://en.wikipedia.org/wiki/Behavior-driven_development) con [SpecFlow](http://specflow.org/), pero hacerlo desde la capa de negocio, incluyendo la base de datos, sin pasar por la interfaz de usuario. 

Además, en cuanto a las pruebas unitarias, hacerlas sólo en los casos complejos, como algunas máquinas de estado, por ejemplo.

Y la verdad es que, aunque me había funcionado bien, me daba cierta vergüenza decirlo, hasta que un día escuché a [Scott Allen](http://odetocode.com/about/scott-allen) en [.Net Rocks](http://www.dotnetrocks.com/?show=1405) y luego encontré [esta respuesta en Stack Overflow](https://stackoverflow.com/questions/153234/how-deep-are-your-unit-tests#answer-153565) del mismísimo [Kent Beck](https://en.wikipedia.org/wiki/Kent_Beck), considerado el padre de [TDD](https://en.wikipedia.org/wiki/Test-driven_development). 

Todavía no vamos a hablar sobre [BDD](https://en.wikipedia.org/wiki/Behavior-driven_development), sino que vamos a comenzar con las pruebas de integración, como ya lo hemos mencionado.

Sin embargo, estamos apuntando a facilitar el desarrollo de las pruebas o, mejor dicho, las especificaciones con [SpecFlow](http://specflow.org/).

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [Productivity Power Tools 2017](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)

* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

* [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

* [.NET Core 1.1.2 - x64 Installer](https://download.microsoft.com/download/D/0/2/D028801E-0802-43C8-9F9F-C7DB0A39B344/dotnet-win-x64.1.1.2.exe)
* [.NET Core SDK 1.0.4 - x64 Installer](https://download.microsoft.com/download/B/9/F/B9F1AF57-C14A-4670-9973-CDF47209B5BF/dotnet-dev-win-x64.1.0.4.exe)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones)

### Paquetes NuGet utilizados

* FluentAssertions - 4.19.2
* Microsoft.EntityFrameworkCore - 1.1.2
* Microsoft.EntityFrameworkCore.Design - 1.1.2
* Microsoft.EntityFrameworkCore.SqlServer - 1.1.2
* Microsoft.NET.Test.Sdk - 15.0.0
* NLog - 5.0.0-beta07
* System.ComponentModel.Annotations - 4.3.0
* xunit - 2.2.0
* xunit.runner.visualstudio - 2.2.0


## A - Mover proyecto de pruebas en la solución

Preparando el artículo me di cuenta que las pruebas que vamos a hacer corresponden al proyecto de prueba y no directamente a las librerías, aunque, obviamente al probar la aplicación también estamos probándolas, pero me parece más adecuado renombrar la carpeta **tests** a **samples.tests**, para reflejar más claramente lo que son.

> {{< IMPORTANT "Importante" >}}

> Estrictamente hablando, probablemente sería más fácil eliminar y crear los proyectos de nuevo, porque están vacíos, pero es útil saber cómo hacer esta operación, para cuando sea realmente necesario.

### A-1 - Renombrar carpetas en Visual Studio

Desde el explorador de la solución:

1. Cambiar nombre de la carpeta **tests** por **samples.test**
2. Cerar la solución, salvando el archivo .sln cuando Visual Studio pregunte si quiere salvar los cambios

### A-2 - Renombrar carpetas del sistema de archivos

Desde el explorador de archivos:

1. Cambiar nombre de la carpeta **tests** por **samples.test**
2. Editar el archivo Domion.Net.sln y cambiar las línea donde aparecen las rutas originales de los proyectos de pruebas por las rutas nuevas:

Rutas originales (`"tests\...`) 
```cs
Project("{FAE ... FBC}") = "DFlow.Budget.Lib.Tests", "tests\DFlow.Budget.Lib.Tests\DFlow.Budget.Lib.Tests.csproj", ...

Project("{9A1 ... 556}") = "DFlow.Transactions.Lib.Tests", "tests\DFlow.Transactions.Lib.Tests\DFlow.Transactions.Lib.Tests.csproj", ...
```

Rutas nuevas (`"samples.tests\...`)
```cs
Project("{FAE ... FBC}") = "DFlow.Budget.Lib.Tests", "sample.tests\DFlow.Budget.Lib.Tests\DFlow.Budget.Lib.Tests.csproj", ...

Project("{9A1 ... 556}") = "DFlow.Transactions.Lib.Tests", "samples.tests\DFlow.Transactions.Lib.Tests\DFlow.Transactions.Lib.Tests.csproj", ...
```

### A-3 - Abrir la solución y recompilar

Al abrir la solución se deberían ver los proyectos en el explorador de la solución y recompilar sin errores.

{{<image src="/posts/images/devenv_2017-06-16_12-06-49.png">}}

### A-4 - Cambiar las referencias de los proyectos

En nuestro caso, todavía no hay ninguna referencia hacia los proyectos de prueba, pero si las hubiese, sería necesario cambiarlas en los archivos .csproj de los proyectos correspondientes.

### A-5 - En caso de emergencia

En caso de que no logre realizar este proceso con éxito, probablemente lo mejor es borrar el proyecto por completo y crearlo de nuevo.

> {{< IMPORTANT "Importante" >}}

> Recuerde que al crear el proyecto sobre una carpeta de solución en Visual Studio, debe seleccionar a mano la carpeta en el sistema de archivos.

## B - Preparación del ambiente

> {{< IMPORTANT "Importante" >}}

> Al escribir el artículo (15/06/2017), por alguna razón que no logré identificar y corregir, falló el explorador de pruebas de Visual Studio 2017 y, al no encontrar las pruebas, no podía ejecutarlas.

> Sin embargo pude encontrar una vuelta, cambiando el **TargetFramework** de los proyectos a **.NET Framework 4.6.2**.

> En esta sección entonces vamos a cambiar el TargetFramework de los proyectos, además de instalar los paquetes necesarios, para no tener que enfrentarnos con el problema mencionado.

### B-1 - Cambiar el TargetFramework (plataforma)

#### ¿Qué significa cambiar el target framework?

Significa que después de hacerlo, aunque vamos a seguir desarrollando en .NET Core, sólo vamos a poder correr los programas en Windows.

Si el target framework fuera .NET Core, podríamos correrlo en cualquier plataforma soportada, por ejemplo, Linux.

De todas formas, eventualmente resolverán este problema y podremos volver a .NET Core.

> {{< IMPORTANT "Importante" >}}

> Cuando trabajamos con .NET Core podemos incluso utilizar varios TargetFrameworks, por ejemplo **.NET Core** y **.NET Framework 4.6.2**, y así generar los ejecutables para ambas plataformas.

> Lamentablemente el explorador de pruebas tampoco funcionaba al trabajar con las dos plataformas simultáneamente.

#### B-1.1 - Descargar todos los proyectos de la solución

En teoría se debería poder hacer sin necesidad de descargar los proyectos, pero me ha resultado más rápido hacerlo así.

1. Seleccionar todos los proyectos en el explorador de la solución
2. **[Botón derecho > Unload Project]**

{{<image src="/posts/images/2017-06-15_13-51-33.png">}}

#### B-1.2 - Modificar DFlow.Budget.Core.csproj

El archivo .csproj se modifica con **[Botón derecho > Edit {nombre del proyecto}.csproj]** sobre el proyecto en el explorador de la solución.

Para cambiar la plataforma hay que cambiar el tag del TargetFramework de .NET Core 1.1 (**`netcoreapp1.1`**)

```xml
<PropertyGroup>
	<TargetFramework>netcoreapp1.1</TargetFramework>
</PropertyGroup>
```

a .NET Framework 4.6.2 (**`net462`**)

```xml
<PropertyGroup>
	<TargetFramework>net462</TargetFramework>
</PropertyGroup>
```

Además de cambiar la plataforma, en el proyecto **DFlow.Budget.Core**, es necesario incluir un referencia a externa a **System.ComponentModel.Annotations** cuando el target framework sea "net462", así que el archivo **DFlow.Budget.Core.csproj** debe quedar así:

{{<renderSourceFile "samples\DFlow.Budget.Core\DFlow.Budget.Core.csproj">}}

Note que la inclusión de la referencia está condicionada al TargetFramework **net462**

No es necesario hacer esto para **netcoreapp1.1**, porque el paquete **System.ComponentModel.Annotations** ya está incluído en .NET Core.

#### B-1.3 - Editar archivos .csproj

Para el resto de los proyectos sólo es necesario hacer el cambio de la plataforma, editando el archivo .csproj para cambiar **netcoreapp1.1** por **net462**.

### B-1.4 - Recargar todos los proyectos de la solución

1. Seleccionar todos los proyectos en el explorador de la solución
2. **[Botón derecho > Reload Project]**

{{<image src="/posts/images/2017-06-16_12-47-07.png">}}

En este momento debería poder compilar la solución sin errores.

### B-2 - Crear proyecto src\Domion.FluentAssertions

Como estamos preparando el ambiente, vamos a crear de una vez el proyecto indicado, porque lo vamos a necesitar en un momento.

El proyecto se debe crear como **Class Library (.NET Core)**

> {{< IMPORTANT "Importante" >}}

> Recuerde que debe seleccionar manualmente la carpeta **"src"** al crear el proyecto.

Para este proyecto también tenemos que hacer el cambio de plataforma e incluir el paquete **System.ComponentModel.Annotations**, así que vamos a modificar el archivo **Domion.FluentAssertions.csproj** a esto:

{{<getSourceFile "src\Domion.FluentAssertions\Domion.FluentAssertions.csproj">}}

Con esto también instalaremos de una vez el paquete **FluentAssertions** que vamos a necesitar.

### B-3 - Configurar proyecto de pruebas
 
#### B-3.1 - Crear archivo de configuración de xUnit

Crear el archivo **xunit.runner.json** en la raíz del proyecto de pruebas: 

{{<getSourceFile "samples.tests\DFlow.Budget.Lib.Tests\xunit.runner.json">}}

Este archivo hace que el explorador de pruebas muestre el sólo nombre de los métodos de prueba (en vez de mostrar también el nombre completo de la clase):

Ajustar las propiedades del archivo (**[Alt]+[Enter]** o **[Botón derecho > Properties]** sobre el explorador de la solución) para que siempre se copie a la carpeta de salida (ejecutables).

{{<image src="/posts/images/devenv_2017-06-15_18-26-36.png">}}

## C - Pruebas de integración básicas

En esta sección del artículo vamos desarrollar una versión inicial básica de las pruebas de integración.

Incluso, vamos a comenzar con una versión que ni siquiera ha pasado por una refactorización, para explicar el proceso de llegar a una estructura mucho más cómoda de usar.

### C-1 - Trabajar con DbContext

> {{< IMPORTANT "Importante" >}}

> Es importante tener en cuenta la [forma adecuada de trabajar con un DbContext](https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx#Anchor_1), ya que no seguir las recomendaciones nos puede traer problemas difíciles de diagnosticar y resolver.

Un DbContext es, entre otras cosas, un cache, con sus ventajas e inconvenientes, así que es necesario estar consciente de eso.

Entonces, como para efectos de esta serie estamos enfocados en el desarrollo de aplicaciones web, donde la vida de cada DbContext está limitada a un request, tenemos que simular ese ciclo de vida en las pruebas, para que éstas se parezcan más a las condiciones reales de producción.

Esto quiere decir que un DbContext (que implementa IDisposable) se debe utilizar dentro de una estructura "using" (```using (var dbContext = new DbContext()) { }```) para "delimitar" los pasos que normalmente se realizarían durante un request y estar seguros que el DbContext se descarta al terminar el using.

#### C-1.1 - BudgetClassData - Datos de prueba

Para facilitar el manejo de los datos de prueba, vamos a usar una clase que representa los datos ingresados por el usuario.

Además, como veremos en un artículo posterior, esto es especialmente útil cuando tenemos que hacer referencia a otros objetos

{{<renderSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Helpers\BudgetClassData.cs">}}

#### C-1.2 - Estructura de las pruebas con un DbContext

> {{< IMPORTANT "Importante" >}}

> Para efectos de las pruebas de todo tipo, tanto unitarias como de integración y aceptación o comportamiento (BDD), se usa el patrón Arrange / Act / Assert, en el cuál:

>* **Arrange**: Crea el contexto para realizar la prueba.
>* **Act**: Ejecuta lo que se quiere probar.
>* **Assert**: Verifica que se hayan obtenido los resultados esperados.

Al combinar esto con lo indicado en el punto anterior, resulta que una prueba típica tiene la siguiente estructura, donde lo que más destaca es un ```using () { }``` para cada fase (Arrange, Act, Assert) como se muestra a continuación:

```cs
[Fact]
public void TryInsert_InsertsRecord_WhenValidData()
{
	IEnumerable<ValidationResult> errors = null;

	// Arrange ---------------------------

	var data = BudgetClassData("Insert-Success-Valid - Inserted", TransactionType.Income);

	// Ensure entitiy does not exist
	using (var dbContext = dbSetupHelper.GetDbContext())
	{
		var manager = new BudgetClassManager(dbContext);

		var entity = manager.SingleOrDefault(bc => bc.Name == data.Name);

		if (entity != null)
		{
			errors = manager.TryDelete(entity);

			errors.Should().BeEmpty();

			manager.SaveChanges();
		}
	}

	// Act -------------------------------

	// Insert entity
	using (var dbContext = dbSetupHelper.GetDbContext())
	{
		var manager = new BudgetClassManager(dbContext);

		BudgetClass entity = new BudgetClass { Name = data.Name, TransactionType = data.TransactionType };

		errors = manager.TryInsert(entity);

		manager.SaveChanges();
	}

	// Assert ----------------------------

	errors.Should().BeEmpty();

	// Verify entity exists
	using (var dbContext = dbSetupHelper.GetDbContext())
	{
		var manager = new BudgetClassManager(dbContext);

		var entity = manager.SingleOrDefault(bc => bc.Name == data.Name);

		entity.Should().NotBeNull();
	}
}

```

Según lo que hemos comentado, debería ser bastante clara la necesidad del ```using () {}``` en la fase **Act**, pero ¿Por qué en el **Arrange** y el **Assert**?

Porque de esa forma, al usar instancias diferentes del DbContext, nos aseguramos de evitar resultados incorrectos (tanto falsos positivos como negativos) por objetos que pueden quedar en el ChangeTracker del DbContext, como resultado de las operaciones anteriores.

### C-2 - Refactorización

Con una inspección rápida del código anterior es evidente que hay varias oportunidades de refactorización.

Lo primero que vamos a trabajar son las fases de Arrange y Assert, en éstas encontramos cuatro casos fundamentales.

Para que las pruebas sean repetibles es necesario:

1. Asegurar que algunas entidades existan en la base de datos y/o
1. Asegurar que algunas entidades no existan en la base de datos

Y para terminar, después de ejecutar que función que se está probando, en la mayoría de los casos, es necesario:

1. Verificar si existen algunas entidades en la base de datos y/o
1. Verificar si no existen algunas entidades en la base de datos

No vamos a ver ahora los detalles de implementación de esos métodos, porque son bastante obvios, pero lo importante es que vamos a usar una clase "Helper" que resuelva esos detalles y ésta va a necesitar una instancia del EntityManager para hacerlo.

#### C-2.1 - Manager Helper

El punto importante es que, después de implementar lo que sería el **BudgetClassManagerHelper** se reducen significativamente las secciones:

**Arrange**
```cs
// Ensure entitiy does not exist
using (var dbContext = dbSetupHelper.GetDbContext())
{
	var manager = new BudgetClassManager(dbContext);
	var helper = new BudgetClassManagerHelper(manager);

	helper.EnsureEntitiesDoNotExist(data);
}
```

**Assert**
```cs
// Verify entity exists
using (var dbContext = dbSetupHelper.GetDbContext())
{
	var manager = new BudgetClassManager(dbContext);
	var helper = new BudgetClassManagerHelper(manager);

	helper.AssertEntitiesExist(data);
}
```

Sin embargo, todavía hay algo por mejorar ahí, aunque a lo mejor no es tan evidente cómo hacerlo.

La solución se basa en el uso de delegados, específicamente un [Action](https://msdn.microsoft.com/en-us/library/018hxwa8(v=vs.110).aspx), porque en ambos casos el contexto del **using** es el mismo, sólo cambia lo que se ejecuta dentro.

#### C-2.2 - Refactorizando el contexto y la verificación

> {{< IMPORTANT "Importante" >}}

> Aquí vamos a ver una estrategia interesante de refactorización usando delegados.

Entonces sólo tenemos que implementar un método que resuelva el contexto y reciba como parámetro lo que se va a ejecutar.

Además, como sabemos que se va a trabajar con el helper, se lo podemos pasar como parámetro al Action para que sea todavía más fácil usarlo:

```cs
private void UsingManagerHelper(Action<BudgetClassManagerHelper> action)
{
	using (var dbContext = dbSetupHelper.GetDbContext())
	{
		var manager = new BudgetClassManager(dbContext);
		var helper = new BudgetClassManagerHelper(manager);

		action.Invoke(helper);
	}
}
```

Después, con este nuevo método, ahora sí se reduce significativamente el código y el nombre de los métodos es completamente explicativo:

**Arrange**
```cs
// Ensure entitiy does not exist
UsingManagerHelper(helper =>
{
	helper.EnsureEntitiesDoNotExist(data);
});
```

**Assert**
```cs
// Verify entity exists
UsingManagerHelper(helper =>
{
	helper.AssertEntitiesExist(data);
});
```

#### C-2.3 - Refactorizando la sección de prueba (Act)

De forma similar, también podemos refactorizar la sección de prueba para obtener esto:

```cs
private void UsingManager(Action<BudgetClassManager> action)
{
	using (BudgetDbContext dbContext = DbSetupHelper.GetDbContext())
	{
		var manager = new BudgetClassManager(dbContext);

		action.Invoke(manager);
	}
}
```

Ahora, aplicando todas las refactorizaciones, la prueba queda bastante simplificada respecto a la versión inicial:

```cs
[Fact]
public void TryInsert_InsertsRecord_WhenValidData()
{
	IEnumerable<ValidationResult> errors = null;

	// Arrange ---------------------------

	var data = BudgetClassData("Insert-Success-Valid - Inserted", TransactionType.Income);

	UsingManagerHelper(helper =>
	{
		helper.EnsureEntitiesDoNotExist(data);
	});

	// Act -------------------------------

	UsingManager(manager =>
	{
		BudgetClass entity = new BudgetClass { Name = data.Name, TransactionType = data.TransactionType };

		errors = manager.TryInsert(entity).ToList();

		manager.SaveChanges();
	});

	// Assert ----------------------------

	errors.Should().BeEmpty();

	UsingManagerHelper(helper =>
	{
		helper.AssertEntitiesExist(data);
	});
}
```

### C-3 - Refactorización y versión final

Para no extender demasiado el artículo, a continuación presentamos las versiones finales de las clases resultantes.

Las clases que se muestran a continuación se pueden copiar e incluir directamente en el proyecto.

#### C-3.1 - BudgetClassDataMapper - Convertidor entre datos y entidades

Entre esta clase y **BudgetClassData**, se implementa el patrón [Mapper](https://martinfowler.com/eaaCatalog/mapper.html) para establecer la comunicación entre las pruebas y las librerías (.Core y .Lib) del módulo, haciendo la conversión **BudgetClass** <--> **BudgetClassData**.

Este patrón, va a resultar especialmente útil en los escenarios más complejos que veremos más adelante en la serie.

{{<getSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Helpers\BudgetClassDataMapper.cs">}}

#### C-3.2 - BudgetClassManagerHelper - Asistente del EntityManager

En esta clase se implementan los cuatro puntos mencionados en [C-2 - Refactorización](#c-2-refactorización).

Adicionalmente a lo que se indicó en ese punto, se puede ver que en los métodos <strong>Assert...</strong> se están creando nuevos DbContext y EntityManager.

Esto se hace para poder llamarlos desde los <strong>Ensure...</strong> y estar seguros que no estamos trabajando con el mismo DbContext donde se realizó la operación.

{{<renderSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Helpers\BudgetClassManagerHelper.cs">}}

#### C-3.3 - FluentAssertionsExtensions - Extensión para facilitar la verificación de errores

En esta clase se implementa una extensión sobre la librería **FluentAssertions**, para facilitar la verificación de los mensajes de error en las validaciones.

Aquí se compara si en la colección de mensajes recibidos, hay alguno que comience con la parte constante del mensaje esperado.

La parte constante del mensaje va desde el comienzo del string hasta la posición del primer parámetro de sustitución (el primer "{").

Esta clase la vamos a incluir en el proyecto **src\Domion.FluentAssertions** que creamos [anteriormente](#b-2-crear-projecto-src-domion-fluentassertions).

{{<getSourceFile "src\Domion.FluentAssertions\Extensions\FluentAssertionsExtensions.cs">}}

#### C-3.4 - BudgetClassManager_IntegrationTests - Pruebas de integración

Finalmente llegamos a las pruebas de integración, donde implementamos las siguientes pruebas básicas:

1. Se pueden insertar entidades.
2. Se pueden modificar entidades.
3. Se pueden eliminar entidades.
4. No se puede insertar una entidad con nombre duplicado.
5. No se puede modificar el nombre de una entidad si ya existe otra con el nuevo nombre, porque se duplicaría.</br></br>

{{<getSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Tests\BudgetClassManager_IntegrationTests.cs">}}

> {{< IMPORTANT "Importante" >}}

> Uno de los aspectos más interesantes de estas pruebas, es que logramos un nivel de estandarización, con el que podemos generar con [MDA](https://en.wikipedia.org/wiki/Model-driven_architecture) los bloques básicos de pruebas, que luego nos ayuden a desarrollar más rápidamente especificaciones ejecutables con [SpecFlow](http://specflow.org/) para usar el enfoque [BDD](https://en.wikipedia.org/wiki/Behavior-driven_development) como pruebas de aceptación.

### C-4 - Ejecución de las pruebas

Después de compilar la solución, se debe ver todas las pruebas en el explorador de pruebas (**[Menú Principal > TEST > Windows > Test Explorer]**):

{{<image src="/posts/images/devenv_2017-06-16_16-27-11.png">}}

Y se deben ejecutar todas (**Run All**) correctamente:

{{<image src="/posts/images/devenv_2017-06-15_19-49-29.png">}}

Y si consultamos la base de datos que se creó usando el string de conexión de la clase de pruebas **BudgetClassManager_IntegrationTests**, deberíamos ver esto:

{{<image src="/posts/images/Ssms_2017-06-16_16-35-52.png">}}

> {{< IMPORTANT "Importante" >}}

> Uno de los aspectos más interesantes de esta forma de trabajo es que no nos hemos tenido que ocupar de la base de datos, aparte del string de conexión a utilizar.

> Al correr las pruebas se va a crear automáticamente la base de datos y siempre va a tener los mismos datos y al final de las pruebas siempre va a quedar en el mismo estado (si todo va bien), así que cuando tengamos algún problema será mucho más fácil hacer el diagnóstico.

### C-5 - Repetición de las pruebas

Las pruebas se pueden ejecutar tantas veces como se quiera y siempre darán los mismos resultados, con la única diferencia de los Id de algunos registros que cambiarán con cada ejecución, por la eliminación de algunos registros en las secciones de **Arrange** de las pruebas.

En caso necesario, se puede eliminar por completo la base de datos como se indica a continuación, porque se creará de nuevo automáticamente al ejecutar las pruebas:

{{<image src="/posts/images/2017-06-16_16-42-31.png">}}

{{<image src="/posts/images/Ssms_2017-06-16_16-44-53.png">}}

## Resumen

En este artículo trabajamos le desarrollo de pruebas básicas de integración trabajando con xUnit y Entity Framework Core y construimos las bases para otros escenarios más complejos e interesantes que exploraremos más adelante.

También aprendimos algunos detalles importantes sobre el trabajo y las pruebas con los [DbContext](https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext).

---

{{< goodbye >}}

---

### Enlaces relacionados

**Action**  
https://msdn.microsoft.com/en-us/library/018hxwa8(v=vs.110).aspx

**AutoFac**  
https://autofac.org/

**BDD**  
https://en.wikipedia.org/wiki/Behavior-driven_development

**DbContext Life cycle**  
https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx#Anchor_1

**DbContext**  
https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext

**Entity Framework Core**  
https://docs.microsoft.com/en-us/ef/core/index

**FluentAssertions**  
http://fluentassertions.com/

**Kent Beck**  
https://en.wikipedia.org/wiki/Kent_Beck

**Kent Beck, sobre TDD en Stack Overflow**  
https://stackoverflow.com/questions/153234/how-deep-are-your-unit-tests#answer-153565

**MDA**  
https://en.wikipedia.org/wiki/Model-driven_architecture

**Patrón de repositorio**  
https://martinfowler.com/eaaCatalog/repository.html

**Patrón Mapper**  
https://martinfowler.com/eaaCatalog/mapper.html

**Scott Allen en .Net Rocks**  
http://www.dotnetrocks.com/?show=1405

**Scott Allen**  
http://odetocode.com/about/scott-allen

**SpecFlow**  
http://specflow.org/

**TDD**  
https://en.wikipedia.org/wiki/Test-driven_development

**xUnit**  
https://xunit.github.io/

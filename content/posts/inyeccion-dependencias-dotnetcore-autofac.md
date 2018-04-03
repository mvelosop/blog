---
title: Inyección de dependencias en .NET Core con AutoFac
draft: true
author: Miguel Veloso
date: 2017-06-24
description: Reseña tamaño twitter
thumbnail: 
categorías: [ "Desarrollo" ]
tags: [  ]
series: [ "Domion" ]
repoName: Domion.Net
#repoRelease: "1.0"
toc: true
---

Breve descripción de lo que se hace en el artículo.

> {{< IMPORTANT "Puntos Importantes" >}}

> 0. Lista de los puntos más importantes del artículo

{{< repoUrl >}}

> {{< IMPORTANT "Para realizar el tutorial paso a paso" >}}

> Si quiere realizar el tutorial paso a paso, le recomiendo que comience con los [fuentes del release 3.0 (.zip)] (https://github.com/mvelosop/Domion.Net/archive/3.0.zip)

> El tiempo estimado para realizar este tutorial es de aproximadamente media hora.

## Contexto

Texto descriptivo del contexto.

### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## A - Aplicando inyección de dependencias

En primer lugar vamos a aplicar la inyección de dependencias a la clase de pruebas de integración, pero primero un pequeño repaso.

### ¿Qué es la inyección de dependencias?

Desde un punto de vista conceptual, la inyección de dependencias es muy sencillo, luego hay algunos detalles que lo complican un poco pero, en cualquier caso, es bastante fácil de utilizar.

[James Shore](http://www.jamesshore.com/Consulting/Credentials.html) tiene una [excelente explicación sobre la inyección de dependencias](http://www.jamesshore.com/Blog/Dependency-Injection-Demystified.html), así que, tomando los elementos clave que él menciona, trataré de resumirlo.

En primer lugar, ¿Qué es una dependencia?

Cuando un objeto necesita una instancia de otro, se establece un dependencia y, entonces, el primero depende del segundo.

Por ejemplo, cuando en un método necesitamos una variable y para crearla usamos ```var variable = new ClaseNecesaria()```, establecemos una dependencia, porque ahora nuestra clase depende de esa otra clase "externa".

Además, cuando usamos ```var variable = new ClaseNecesaria()``` creamos la dependencia específica con esa clase y si necesitáramos trabajar con un sub-tipo de **ClaseNecesaria**, tendríamos que modificar el código.

Una solución para evitar esto es no crear la instancia dentro de la clase, sino pedirla en el constructor, para que sea responsabilidad del usuario de la clase, crear esa instancia necesaria.

Esto se ve claramente en este fragmento del ejemplo que estamos desarrollando:

```cs
var manager = new BudgetClassManager(dbContext);

var helper = new BudgetClassManagerHelper(manager, ...
```

Entonces, citando a [James Shore](http://www.jamesshore.com/Consulting/Credentials.html):

> {{< IMPORTANT "Inyección de Dependencias - Definición" >}}

> **Inyección de dependencias es darle a un objeto las instancias de las variables que necesita.**

La inyección de dependencias se fundamenta en la existencia de un "**Contenedor de Dependencias**" donde registramos las clases que vamos a necesitar y al que pedimos las instancias que necesitamos.

Cuando trabajamos con un contenedor de depencias el código anterior queda simplificado a algo como esto:

```cs
var helper = container.Resolve<BudgetClassManagerHelper>();
```

En este caso el contenedor sabe que para crear una instancia de **BudgetClassManager**, necesita una instancia de **BudgetClassManager** y esta necesita una instancia de **BudgetDbContext**.

Entonces el contenedor resuelve toda esa cadena de dependencias automáticamente y en el orden adecuado para nosotros.

El otro concepto importante es el **ambito** (scope) de los contenedores, que determina el ciclo de vida de las instancias que le pedimos. Esto lo veremos más adelente en el artículo.

Para más información también puede consultar [este artículo de Martin Fowler](https://martinfowler.com/articles/injection.html).

### Beneficios e inconvenientes

Lo que me gusta:

1. Simplifica la creación de objetos.
2. Ayuda a hacer evidente las dependencias entre clases.
3. Facilita las pruebas unitarias.

Lo que no me gusta:

1. Puede ser mucho más complejo depurar un programa.
2. Los errores de configuración se detectan en tiempo de ejecución.


### A-1 - Preparar contenedor

Como ya mencionamos, para empezar es necesario registrar las clases en el contenedor y esto puede ser representar mucho trabajo si hay que registrar cada clase de forma individual.

Vamos desarrollar un configurador basado en las convenciones que utilizamos, pensando en refactorizar luego las partes reutilizables a la librería general.

> {{< IMPORTANT "Corregir" >}}

#### A-1.1 - Preparar proyecto Budget.Setup

1. Agrear paquetes NuGet
   * **Autofac - 4.6.0**


**Renombrar método GetDbContext a CreateDbContext**


#### A-1.2 - BudgetContainerSetup - Configurador del contenedor para el módulo

En esta clase vamos a manejar todo lo relacionado con la configuración del contenedor de dependencias para el módulo de presupuesto (**Dflow.Budget**).

Básicamente lo que hacemos es usar las convenciones de nombres para registrar las clases que existen en todos los archivos .dll del módulo, es decir, los que comienzan con "**DFlow.Budget**".

> {{< IMPORTANT "Ámbitos en inyección de dependencias" >}}

Hay un punto que vamos destacar brevemente, que se refiere al **ámbito** (scope) de la inyección de dependencias.

El ámbito define hasta cuándo estará disponible un objeto creado por el contenedor, desde la instanciación original hasta su disposición.

Los tres casos más frecuentes son:

* **Singleton**: Una sola instancia durante toda la vida de la aplicación.
* **Transient**: Una nueva instancia cada vez que se solicite (Equivalente a new {Objeto}()).
* **LifetimeScope**: Una sola instancia dentro del ámbito donde se solicita (por ejemplo, en un request).

En la clase a continuación vemos que para registrar todas las clases utilizamos el método ```InstancePerLifetimeScope()```, para poder simular en las pruebas el trabajo dentro de un request, por lo que mencionamos en el [artículo anterior](/posts/pruebas-integracion-xunit-entity-framework-core/#c-1-trabajar-con-dbcontext).

Vamos a incluir esta clase en el proyecto **DFlow.Budget.Setup** que maneja todos aspectos de preparación para ejecutar la aplicación.

{{<getFixedSourceFile "samples\DFlow.Budget.Setup\BudgetContainerSetup.cs" "4.0-init">}}

### A-2 - Ajustar asistente del EntityManager

Originalmente usábamos el ```BudgetClassManager``` por un lado y por el otro el ```BudgetDbSetupHelper```, para crear el DbContext que necesita el primero.

También necesitábamos un ```BudgetClassDataMapper``` y lo instanciábamos en el constructor, para tenerlo siempre disponible.

Así que este ha sido nuestro constructor hasta ahora:

```cs
public BudgetClassManagerHelper(
	BudgetClassManager classBudgetClassManager,
	BudgetDbSetupHelper budgetDbSetupHelper)
{
	BudgetClassManager = classBudgetClassManager;
	BudgetDbSetupHelper = budgetDbSetupHelper;

	BudgetClassMapper = new BudgetClassDataMapper();
}
```

Podríamos utilizar este mismo constructor con inyección de dependencias, pero vamos a aprovechar para mejorar algunas cosas.

En primer lugar, vamos a eliminar el ```BudgetDbSetupHelper``` porque sólo lo necesitamos para obtener el DbContext que necesita el EntityManager.

Ahora el contenedor se va a encargar de instanciar el ```BudgetDbContext``` cuando solicitemos un ```BudgetClassManager```.

En este fragmento del registro es donde indicamos como se crea un BudgetDbContext:

```cs
public void RegisterTypes(ContainerBuilder builder)
{
	// This defers instance registration until it is actually needed
	builder.Register<BudgetDbContext>((c) => DbSetupHelper.CreateDbContext())
		.InstancePerLifetimeScope();

	RegisterCommonModuleTypes(builder, _modulePrefix);
}
```

El otro inconveniente con el constructor original es que obliga al contenedor a crear instancias de todas las dependencias, pero en la realidad no siempre se usan todas cuando se instancia la clase. Para evitar eso y de ahora en adelante, vamos a trabajar con la clase [Lazy\<T>](https://msdn.microsoft.com/en-us/library/dd642331(v=vs.110).aspx).

Con [Lazy\<T>](https://msdn.microsoft.com/en-us/library/dd642331(v=vs.110).aspx), la instancia requerida se crea sólo en el momento en que realmente se va a usar.

Entonces, de ahora en adelante, vamos a implementar el patrón de esta forma para inyectar todas las dependencias:


```cs
private readonly Lazy<BudgetClassManager> LazyBudgetClassManager;

public BudgetClassManagerHelper(
	Lazy<BudgetClassManager> lazyBudgetClassManager)
{
	LazyBudgetClassManager = lazyBudgetClassManager;
}

private BudgetClassManager BudgetClassManager => LazyBudgetClassManager.Value;
```

Para terminar, cuando incluimos en el constuctor el parámetro ```ILifetimeScope scope```, Autofac inyecta el ámbito actual en la clase, donde podemos usarlo para instanciar las clases o crear otros sub-ámbitos.

#### A-2.1 - BudgetClassManagerHelper

El asistente ajustado finalmente queda así:

{{<renderSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Helpers\BudgetClassManagerHelper.cs">}}

### A-3 - Ajustar clase de pruebas de integración

Ahora que ya tenemos estamos manejando la inyección de dependencias en el EntityManager, tenemos que ajustar las pruebas.

Lo primero es la configuración del contenedor de dependencias. Para eso agregamos este método que será invocado desde el constructor estático, para que sólo se ejecute una vez.

```cs
private static IContainer SetupContainer(BudgetDbSetupHelper dbHelper)
{
	var containerSetup = new BudgetContainerSetup(dbHelper);

	var builder = new ContainerBuilder();

	containerSetup.ResgisterTypes(builder);

	IContainer container = builder.Build();

	return container;
}
```

Luego ajustaremos también los métodos UsingManager y UsingManagerHelper.

> {{< IMPORTANT "Creación de instancias con el contenedor" >}}

> En estos fragmentos podemos ver que ahora usamos ```scope.Resolve<BudgetClassManager>()``` en vez de ```new BudgetClassManager()``` para crear las nuevas instancias.

En ambos casos vamos a incluir también el ILifetimeScope como parámetro del action para tener más flexibilidad en éste:


```cs
private void UsingManager(Action<ILifetimeScope, BudgetClassManager> action)
{
	using (ILifetimeScope scope = Container.BeginLifetimeScope())
	{
		var manager = scope.Resolve<BudgetClassManager>();

		action.Invoke(scope, manager);
	}
}

private void UsingManagerHelper(Action<ILifetimeScope, BudgetClassManagerHelper> action)
{
	using (ILifetimeScope scope = Container.BeginLifetimeScope())
	{
		var helper = scope.Resolve<BudgetClassManagerHelper>();

		action.Invoke(scope, helper);
	}
}
```

#### A-3.1 - BudgetClassManager_IntegrationTests

Después de hacer las modificaciones principales anteriores y ajustar la clase de pruebas para que compile, llegamos a esta versión de la clase:

{{<renderSourceFile "samples.tests\DFlow.Budget.Lib.Tests\Tests\BudgetClassManager_IntegrationTests.cs">}}

### A-4 - Compilar

> {{< IMPORTANT "Compilar" >}}


## B - Refactorizando en librerías

En este punto ya hemos identificado una buena oportunidad de refactorización hacia una librería, con la clase ```BudgetContainerSetup```, así que vamos a crear un nuevo proyecto en las librería, para refactorizar las clases relaciones con preparación de los módulos.

### B-1 - Librería de preparación de módulos

#### B-1.1 - Crear proyecto Domion.Setup

1. Crear proyecto **src\Domion.Setup** como **Class Library (.NET Core)**.

2. Cambiar plataforma a .NET Framework 4.6.2 (ver [artículo anterior](/posts/pruebas-integracion-xunit-entity-framework-core/#b-1-cambiar-el-targetframework-plataforma))

3. Instalar paquetes NuGet
   * **Autofac - 4.6.0**

#### B-1.2 - Crear BaseContainerSetup

{{<renderSourceFile "src\Domion.Setup\BaseContainerSetup.cs">}}

#### B-1.4 - Agregar referencia a Domion.Setup en DFlow.Budget.Setup

> {{< IMPORTANT "Importante" >}}


#### B-1.5 - Ajustar BudgetContainerSetup

{{<renderSourceFile "samples\DFlow.Budget.Setup\BudgetContainerSetup.cs">}}

## C - Pruebas

Con esto terminamos los ajustes y sólo nos falta verificar si todo sigue funcionando correctamente.

Así que al ejecutar todas las pruebas debemos obtener algo como esto:

{{<image src="/posts/images/devenv_2017-06-29_14-59-42.png">}}



---

{{< goodbye >}}

---

### Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

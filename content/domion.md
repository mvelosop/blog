---
title: Domion - Un sistema para desarrollar aplicaciones en .NET Core
draft: false
author: Miguel Veloso
date: 2017-05-30
description: Un conjunto de componentes para facilitar el desarrollo de aplicaciones web y móviles
thumbnail: images/puzzle-1713170_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Architecture" ]
series: [ "Domion" ]
---

Con este artículo voy a comenzar una serie para repasar y aprender conceptos fundamentales del desarrollo de aplicaciones web y móviles en .NET. 

La idea es que sirva como recurso de aprendizaje y referencia, para aprender cosas nuevas y para profundizar en temas más conocidos.

También pretendo que este proceso sirva para desarrollar una librería y otros componentes, que se faciliten el desarrollo de ese tipo de aplicaciones.

La arquitectura que voy a utilizar en esta serie es producto de lo que me ha funcionado bien a lo largo de varios años de desarrollo de aplicaciones web en .NET MVC y puede no ser la más adecuada para otros tipos de aplicaciones. De hecho, seguramente habrá mejores soluciones y espero que podamos encontrarlas con los comentarios de los interesados.

En resumen, no garantizo que esta sea la solución adecuada para ningún problema en particular y es responsabilidad del lector determinar si le puede resultar útil.

Todo el código que publique estará bajo licencia MIT, así que lo único que se requiere es mencionar la fuente. En todo caso, agradecería que me lo comunicaran, aunque, desde luego, esto es opcional.

## Artículos de la serie en orden de publicación

1. [Preparar una solución para ASP.NET Core](/posts/preparar-solucion-aspnet-core/) - [Release 1.0 (GitHub)](https://github.com/mvelosop/Domion.Net/releases/tag/1.0)

2. [Patrón de repositorio con Entity Framework Core](/posts/patron-repositorio-entity-framework-core/) - [Release 2.0 (GitHub)](https://github.com/mvelosop/Domion.Net/releases/tag/2.0)

3. [Pruebas de integración con xUnit y Entity Framework Core](/posts/pruebas-integracion-xunit-entity-framework-core) - [Release 3.0 (GitHub)](https://github.com/mvelosop/Domion.Net/releases/tag/3.0)









## ¿Por qué Domion?

Domi es una palaba griega (en caracteres latinos) que significa estructura y la entiendo como una forma de hacer referencia a la arquitectura de las aplicaciones, así que la utilicé como raíz y simplemente le agregué "on" porque me pareció que sonaba mejor.

Creo que el tema de arquitectura es uno de los factores fundamentales para desarrollar aplicaciones sólidas y, además, como espero poder demostrar a lo largo de esta serie, también puede contribuir para agilizar el proceso de desarrollo.

## Alcance de la aplicación

En principio vamos a desarrollar una aplicación simple para gestionar el flujo de caja personal, para probar el desarrollo web con MVC Core, REST API y aplicaciones móviles, pero la iremos ajustando a medida que sea necesario para explorar nuevos caminos.

## Arquitectura de la aplicación

La aplicación está planteada inicialmente como un monolito modular, de varias capas.

Aunque no sería estrictamente necesario hacerla modular por lo pequeña, lo hago para probar la composición de módulos de forma dinámica.

Eventualmente podría evolucionar hacia una arquitectura de micro-servicios, aunque no lo tengo incluido en el plan inicial.

## Enfoques y Procesos

En este proyecto utilizaré principalmente los siguientes enfoques y procesos de desarrollo:

### DDD - Domain Driven Design

[Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) es un enfoque de desarrollo que se basa en el uso de los objetos del dominio para establecer un lenguaje ubicuo del negocio en todos los productos de la aplicación.

En particular vamos a trabajar el concepto de [Bounded Contexts](https://martinfowler.com/bliki/BoundedContext.html) asociado a los [DbContext](https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext) de [Entity Framework (Core)](https://docs.microsoft.com/en-us/ef/core/).

### MDA - Model Driven Architecture

[Model Driven Architecture](https://en.wikipedia.org/wiki/Model-driven_architecture) es un enfoque de diseño en el que se parte de un [PIM (Platform Independent Model)](https://en.wikipedia.org/wiki/Platform-independent_model) para generar un [PSM (Platform Specific Model)](https://en.wikipedia.org/wiki/Platform-specific_model) para generar código.

En este caso voy a trabajar los modelos en [Enterprise Architect](http://www.sparxsystems.com/products/ea/) para realizar las transformaciones y generación de código con plantillas desarrolladas específicamente para esto. 

### TDD - Test Driven Development

[Test Driven Development](https://en.wikipedia.org/wiki/Test-driven_development) es un proceso de desarrollo en el que, de forma muy resumida, se desarrolla primero el código de las pruebas y luego el código de la solución.

Puede ver una [presentación de TDD](https://github.com/mvelosop/TDD.Kata1/blob/master/docs/TDDPrimer.pdf) que realicé el 23/02/2017 en un [Meetup de TenerifeDev](https://www.meetup.com/es/TenerifeDev/events/236814516/).

### BDD - Behavior Driven Development

[Behavior Driven Development](https://en.wikipedia.org/wiki/Behavior-driven_development) es un proceso de desarrollo derivado de TDD, donde las especificaciones y los resultados esperados se elaboran en un formato que facilita la comunicación con los usuarios.

En este caso estaremos trabajando con [SpecFlow](http://specflow.org/), la herramienta por excelencia de BDD para .NET.

---

Comencemos entonces de una vez con el primer artículo: [Preparar una solución para ASP.NET Core](/posts/preparar-solucion-aspnet-core/)

---

{{< goodbye >}}

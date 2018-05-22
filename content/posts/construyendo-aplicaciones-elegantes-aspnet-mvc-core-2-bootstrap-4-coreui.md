---
englishVersion: /posts/building-elegant-applications-aspnet-mvc-core-2-bootstrap-4-coreui/
title: Construyendo aplicaciones elegantes con ASP.NET MVC Core 2 y Bootstrap 4 usando CoreUI
draft: false
author: Miguel Veloso
date: 2017-11-04
description: Adaptar una plantilla de BS 4 a una aplicación MVC, para mejorar la experiencia del usario
thumbnail: posts/images/interior-2245657_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "User Experience", "Client Side Development", "Bootstrap 4", "CoreUI" ]
series: [  ]
repoName: AspNetCore2CoreUI
repoRelease: "1.0"
toc: true
---


<p style="background-color: #ffe680; padding: 1rem;">
<span style="color: red; font-weight: bold">ACTUALIZACIÓN</span>: hay una versión <a href="/posts/construyendo-aplicaciones-elegantes-aspnet-core-mvc-2.1-coreui-2-bootstrap-4/" style="font-weight: bold">ACTUALIZADA Y REVISADA de este artículo</a>. Que cubre <a href="https://blogs.msdn.microsoft.com/dotnet/2018/05/07/announcing-net-core-2-1-rc-1/">ASP.NET Core 2.1 (rc1)</a> y <a href="https://coreui.io/">CoreUI 2.0.0</a>.
</p>

En este artículo vamos a explicar como adaptar la plantilla [CoreUI](http://coreui.io/), basada en [Bootstrap 4](http://getbootstrap.com/) para usarla como base en aplicaciones ASP.NET MVC Core 2.

> {{< IMPORTANT "Puntos Importantes" >}}

> 0. Conceptos importantes sobre el manejo de paquetes "client-side" con **npm**

> 0. Entender el rol de [Gulp](https://gulpjs.com/) en el desarrollo "client-side"

> 0. Manejar la relación entre las vistas principales, las vistas Layout y las vistas parciales.

{{< repoUrl >}}

## Contexto

Usualmente trabajo más orientado hacia el back-end, pero siempre he tenido que hacer algo de interfaz de usuario, y como tengo poca habilidad para el diseño, decidí investigar sobre plantillas modernas basadas en [Bootstrap 4](https://getbootstrap.com/docs/4.0/getting-started/introduction/) para adoptar alguna.

Hay una oferta grande en este campo, basta con buscar "admin dashboard templates" en [Google](https://www.google.es/search?q=admin+dashboard+templates) para encontrar muchas opciones, entre gratis y económicas (US$ 10-30). Con prácticamente cualquiera se puede obtener un aspecto suficientemente bueno (desde mi punto de vista) para la interfaz de usuario.

De hecho, he llegado a comprar un par de esas plantillas, pero, aunque cumplen con el asunto del aspecto, al ver la estructura interna y la cantidad de Javascript que utilizan, he quedado un poco decepcionado.

Asi que, aprovechando que recientemente terminé un proyecto donde usé una plantilla, reanudé la búsqueda y conseguí [CoreUI](http://coreui.io/), que es open source, está basada en [Bootstrap 4](https://getbootstrap.com/docs/4.0/getting-started/introduction/) y tiene una versión "pro", con muchas cosas ya resueltas por un buen precio, sobre todo considerando el tiempo necesario para conseguir algo así.

En este artículo me voy a enfocar en el proceso de adaptación de la versión HTML 5 estática de [CoreUI (OpenSource)](http://coreui.io/) bajándola directamente desde [GitHub](https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template), reemplazamdo los componentes HTML de una aplicación básica ASP.NET MVC Core 2 generada directamente desde Visual Studio 2017.

Espero hacerlo de forma que sea sencillo actualizar el proyecto base, cuando [Lukas Holeczek](https://about.me/lukaszholeczek) publique nuevas versiones de la plantilla.



### Herramientas y plataforma

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

* [.NET Core SDK 2.0.2 with .NET Core 2.0.0 - x64 SDK Installer](https://download.microsoft.com/download/7/3/A/73A3E4DC-F019-47D1-9951-0453676E059B/dotnet-sdk-2.0.2-win-x64.exe)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).


## Paso a paso

### 1 - Crear proyecto ASP.NET MVC Core 2

Vamos a comenzar por crear una aplicación MVC estándar, con la plantilla integrada en Visual Studio 2017.

#### 1.1 - Crear solución en blanco

1. Crear la "blank solution" "AspNetCore2CoreUI" con repositorio

2. Agregar carpeta "src" a la solución

3. En este momento el resultado debe ser similar a esto:

{{<image src="/posts/images/devenv_2017-10-31_17-06-44.png">}}

#### 1.2 - Agregar el proyecto ASP.NET MVC Core 2

1. Crear el proyecto **CoreUI.Web** de tipo "ASP.NET Core Web Application" sobre la carpeta de la solución "src" y crear también la carpeta "src" en el file system.
{{<image src="/posts/images/devenv_2017-10-31_17-13-01.png">}}
y seleccionando esa carpeta para crear el proyecto
{{<image src="/posts/images/2017-10-31_17-22-03.png">}}

2. Seleccionar una aplicación **ASP.NET Core 2.0** tipo **MVC** y 
{{<image src="/posts/images/devenv_2017-10-31_17-30-23.png">}}

3. Cambiar autenticación a **Individual User Accounts**, que estén **guardadas en la aplicación**
{{<image src="/posts/images/devenv_2017-10-31_17-38-40.png">}}

#### 1.3 - Crear la base de datos

1. Cambiar el string de conexión en el archivo `appsettings.json` para trabajar con SQL Server Developer Edition
`Server=localhost; Initial Catalog=CoreUI.Web; Trusted_Connection=True; MultipleActiveResultSets=true`

2. Correr la aplicación con [Ctrl]+[F5]

3. Registrarse como usuario en la aplicación para forzar la creación de la base de datos

4. Aplicar la migración, cuando aparezca el error por falta de la base de datos
{{<image src="/posts/images/chrome_2017-10-31_18-02-35.png">}}

5. Refrescar la pantalla cuando se complete el proceso de creación de la base de datos, para terminar el registro del usuario-
{{<image src="/posts/images/chrome_2017-10-31_18-05-42.png">}}

**Este es buen momento para guardar el proyecto en nuestro repositorio!**

#### 1.4 - Eliminar librerías "client-side" originales

Ahora vamos a eliminar todas las librerías "client-side" incluidas por la plantilla de Visual Studio, porque vamos a utilizar las de CoreUI.

También vamos a eliminar el uso de Bower como gestor de paquetes "client-side", ya que vamos a utilizar [npm](https://www.npmjs.com/) y [Node](https://nodejs.org) como parte de CoreUI y así aprovechamos para empezar a conocer un poco más sobre "client-side".

Cómo con [npm](https://www.npmjs.com/) conseguimos todos los paquetes que necesitamos, entonces simplemente quitamos Bower.

Empezamos entonces:

1. Anotar las dependencias "client-side" utilizadas (en el archivo bower.json)
{{<image src="/posts/images/devenv_2017-11-01_13-14-56.png">}}

2. Eliminar el archivo bower.json

3. Cerrar la solución y volver a abrirla para eliminar la carpeta "bower" de las dependencias del proyecto.

4. Abrir la carpeta wwwroot y eliminar la carpeta "lib"
{{<image src="/posts/images/devenv_2017-11-01_14-50-59.png">}}

5. Ejecutar la aplicación con [Ctrl]+[F5] para verla sin ningún estilo (ni Javascript)
{{<image src="/posts/images/chrome_2017-11-01_14-54-42.png">}}

**Vamos a guardar ahora esta versión en el repositorio**

### 2 - Preparar sitio base

Ahora vamos a preparar la carpeta base con CoreUI, que luego vamos a utilizar para pasar los componentes a nuestra aplicación MVC.

En este proceso aprenderemos algo (o, al menos yo aprendí algo) sobre el manejo de las librerías "client-side" en Javascript.

#### 2.1 - Clonar el repositorio de CoreUI

<div style="background-color: #ffe680; padding: 0.1rem 1rem;">
<p>
<span style="color: red; font-weight: bold">NOTA</span>: Este artículo se escribió usando CoreUI v1.0.4, para completar con éxito estos pasos, debe clonar <a href="https://github.com/coreui/coreui-free-bootstrap-admin-template/tree/v1.0.4">CoreUI v1.0.4 desde GitHub</a>.
</p>
<p>
Estoy trabajando en un artículo actualizado, que cubre las próximas versiones de ASP.NET Core MVC 2.1 y CoreUI 2.0.0 que publicaré en breve.
</p>
</div>

Clonar el [repositorio de CoreUI en GitHub](https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template) en cualquier carpeta que resulte conveniente, fuera de la solución.

#### 2.2 Copiar la versión de HTML 5 estática en la solución

Vamos a copiar todo el contenido de la carpeta "**Static_Full_Project_GULP**" del repositorio en la carpeta "**src\CoreUI**" de la solución.

Inicialmente hice esto dentro del proyecto CoreUI.Web, pero Visual Studio se puso extremadamente lento después de descargar los paquetes "client-side" en la carpeta **node_modules**, no estoy seguro si fue por Visual Studio propiamente dicho o por [Resharper](https://www.jetbrains.com/resharper/), pero al hacerlo fuera del proyecto web todo funcionó muy bien.

Más adelante incluiremos un paso para copiar la versión de CoreUI optimizada para despliegue en el proyecto web.

En este momento la solución se debería ver como esto desde el file system:
{{<image src="/posts/images/explorer_2017-11-03_11-33-21.png">}}

> {{< IMPORTANT "src\CoreUI no es parte de la solución de Visual Studio" >}}.

> 0. Note que, aunque src\CoreUI está dentro de las carpetas de la solución y bajo control de versiones, no es parte de la solución de Visual Studio, es decir, esta carpeta no se ve en el explorador de la solución.

#### 2.3 - Instalar los paquetes "client-side" necesarios

1. Primero editamos el archivo **packages.json** para incluir las versiones más recientes de los paquetes utilizados por la plantilla de Visual Studio, que vimos en el [punto 1.4](#1-4-eliminar-librerías-client-side-originales) y 

2. Eliminamos **gulp-bower-src** porque no vamos a utilizar Bower.

{{<image src="/posts/images/Code_2017-11-03_13-12-37.png">}}

Luego ejecutamos los pasos de [instalación de la versión estática de CoreUI](http://coreui.io/docs/getting-started/static-version/), excepto el paso de instalación de Bower, desde la interfaz de comandos en la carpeta **src\CoreUI** de la solución.

Al terminar deberíamos ver el sitio de esta forma (corriendo con `gulp serve`):
{{<image src="/posts/images/chrome_2017-11-03_12-53-00.png">}}

También podemos ver la carpeta **node_modules**, que contiene los paquetes "client-side" especificados en packages.json, así como los requeridos por estos.

{{<image src="/posts/images/explorer_2017-11-03_13-19-44.png">}}

> {{< IMPORTANT "Instalación de paquetes con npm" >}}

> 0. Cuando se instalan paquetes con **npm** se crea la carpeta **node_modules** con todos los componentes necesarios, tanto para las herramientas de desarrollo como [Gulp](https://gulpjs.com/), como para la ejecución de la aplicación. Esta carpeta suele ocupar mucho espacio y no se debe incluir en el despliegue de las aplicaciones.

> 0. El archivo **package.json** determina cuales paquetes se van a instalar al ejecutar **npm install**.

#### 2.4 - Preparar versión de "distribución" base

Todos los html que copiamos del repositorio de CoreUI hacen referencia a los archivos en **node_modules**, pero estas carpetas son recursos para desarrollo, no para despliegue.

Para preparar la versión optimizada para despliegue, vamos a ejecutar el comando `gulp build:dist` desde la interfaz de comandos sobre la carpeta **src\CoreUI**.

Esto va a generar una nueva carpeta **dist** en **src\CoreUI** con el contenido preparado para despliegue, haciendo referencia a las versiones minificadas de las librerías necesarias.

{{<image src="/posts/images/explorer_2017-11-03_14-43-37.png">}}

Si hacemos click sobre el archivo **src\CoureUI\dist\index.html** veremos cómo se cambiaron todas las referencias a **node_modules** por **vendors** y se unificaron todos los componentes necesarios en las carpetas **css**, **fonts** y **js** dentro de **vendors**.

{{<image src="/posts/images/Code_2017-11-03_15-08-59.png">}}

#### 2.5 - Preparar versión de "distribución" para integrarla con proyecto ASP.NET MVC

Ahora vamos a hacer unos ajustes menores para comenzar el proceso de integración con nuestro proyecto ASP.NET MVC.

Primero vamos a incluir las librerías de validación necesarias, modificando el archivo **src\CoreUI\gulp-tasks\build-dist.js**.

Ese archivo lo usa [Gulp](https://gulpjs.com/), una herramienta para automatizar tareas, y que se utiliza frecuentemente en el desarrollo "client-side".

Entonces, vamos a incluir entonces la línea: `node_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js`  
para indicar que esta librería se debe copiar a la versión para despliegue.

{{<image src="/posts/images/Code_2017-11-03_15-19-33.png">}}

Además, vamos a:

1. Crear una tarea adicional en este mismo archivo, para copiar los archivos de distribución a la carpeta **wwwroot** de nuestro proyecto MVC, donde los vamos a usar y

2. Incluir esa tarea como parte de `build:dist`

{{<image src="/posts/images/Code_2017-11-03_15-34-56.png">}}

> {{< IMPORTANT "Paquetes para despliegue" >}}

> 0. Las listas **vendorJS**, **vendorCSS** y **vendorFonts** de **build-dist.js** tienen los componentes utilizados por la versión PRO de CoreUI, pero como no están en **package.json**, no se incluyen en el despliegue.

> 0. Normalmente los paquetes tienen una carpeta **dist** que contiene los componentes optimizados para el despliegue y es responsabilidad del desarrollador indicar los archivos correctos en el las listas indicadas.

Después de esto volvemos a ejecutar `gulp build:dist` y si ejecutamos la aplicación MVC con [Ctrl]+[F5] y vamos a la dirección https://localhost:#####/index.html (##### = puerto asignado por VS) debemos ver lo siguiente:

{{<image src="/posts/images/chrome_2017-11-03_15-40-22.png">}}

La misma página anterior, pero mostrada como una página estática dentro de nuestra aplicación MVC.

**Vamos a guardar ahora la solución en el repositorio.**

#### 2.6 - Cómo incluir nuevos componentes

Con este proceso también entendimos cómo se incluyen nuevos componentes "client-side", como puede ser un date-picker, en la interfaz de usuario:

> {{< IMPORTANT "Pasos para incluir nuevos componentes "client-side"" >}}

> 1. Incluir referencia la librería en **src\CoreUI\package.json**

> 2. Ejecutar **npm install** para traer el paquete desde el repositorio de **npm**

> 2. Editar el archivo **src\CoreUI\gulp-tasks\build-dist.js** para actualizar la lista los archivos que se deben copiar, en caso que no estén.

> 3. Ejecutar `gulp build:dist` para generar la versión de distribución.

Después de eso, sólo falta incluir las nuevas referencias (scripts, estilos, imágenes, etc) en las vistas Razor que lo necesiten.

### 3 - Desarrollar versión Razor de las páginas de CoreUI

En esta sección vamos a convertir las páginas estáticas html de CoreUI en vistas Razor (.cshtml) que se pueden utilizar en cualquier aplicación.

#### 3.1 - Desarrollar controlador genérico para las vistas de CoreUI

Este es un controlador muy sencillo, que recibe el nombre de la vista a mostrar y la retorna.

{{<renderSourceFile "src\CoreUI.Web\Controllers\CoreUIController.cs">}}

#### 3.2 - Crear vista Index inicial

Para esto simplemente:

1. Copiamos la página **index.html** de **wwwroot** a la nueva carpeta **Views\CoreUI**

2. Cambiamos la extensión a .cshtlm

3. Eliminamos el uso del _Layout estándar escribiendo esto al comienzo de la página:
```cs
@{
	    Layout = "";
}
```
4. Cambiamos los "@" por "@@" para evitar el error de sintaxis de Razor

> {{< IMPORTANT "Vistas Razor y html" >}}

> 0. Cualquier archivo .html válido es también una vista Razor válida, sólo hace falta cambiar la extensión a .cshtml

Luego, al ejecutar la aplicación con [Ctrl]+[F5] y navegar hasta https://localhost:#####/CoreUI/Index (##### = puerto asignado por VS) debemos ver lo siguiente:

{{<image src="/posts/images/chrome_2017-11-03_17-37-33.png">}}

Esto ocurre porque falta corregir las referencias a los archivos css, js, imágenes, etc.

Para esto sólo hay que:

1. Identificar las direcciones a los archivos y 
2. Agregar ```~/``` delante:

{{<image src="/posts/images/devenv_2017-11-03_17-45-17.png">}}

Eventualmente pueden ayudar las herramientas de desarrollo de los navegadores modernos para identificar los archivos faltantes:

{{<image src="/posts/images/2017-11-03_17-58-39.png">}}

Una vez que estén corregidas todas las referencias, tendremos la página que ya conocemos, pero esta vez generada por una vista Razor desde la dirección https://localhost:#####/CoreUI/Index.

#### 3.2 - Dividir la vista Index.cshtml en componentes

Ahora vamos a dividir la vista Index.cshtml en:

1. Una vista "_Layout.cshtml"
2. Varias vistas parciales, a modo de componentes y 
3. la vista Index.cshtml, con el contenido principal de la página.

No vamos a mostrar todo el proceso, sólo la vista _Layout final y la lista de archivos resultantes, con lo que debería ser bastante evidente el trabajo y, en último caso, se puede ver el resultado final en el repositorio del artículo.

```
<!--
* CoreUI - Open Source Bootstrap Admin Template
* @@version v1.0.4
* @@link http://coreui.io
* Copyright (c) 2017 creativeLabs Łukasz Holeczek
* @@license MIT
 -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="description" content="CoreUI - Open Source Bootstrap Admin Template">
    <meta name="author" content="Łukasz Holeczek">
    <meta name="keyword" content="Bootstrap,Admin,Template,Open,Source,AngularJS,Angular,Angular2,Angular 2,Angular4,Angular 4,jQuery,CSS,HTML,RWD,Dashboard,React,React.js,Vue,Vue.js">
    <link rel="shortcut icon" href="img/favicon.png">

    <title>CoreUI - Open Source Bootstrap Admin Template</title>

    <!-- Icons -->
    <link href="~/vendors/css/font-awesome.min.css" rel="stylesheet">
    <link href="~/vendors/css/simple-line-icons.min.css" rel="stylesheet">

    <!-- Main styles for this application -->
    <link rel="stylesheet" href="~/css/style.css">

    <!-- Styles required by this view -->

</head>
<!-- BODY options, add following classes to body to change options

// Header options
1. '.header-fixed'					- Fixed Header

// Brand options
1. '.brand-minimized'       - Minimized brand (Only symbol)

// Sidebar options
1. '.sidebar-fixed'					- Fixed Sidebar
2. '.sidebar-hidden'				- Hidden Sidebar
3. '.sidebar-off-canvas'		- Off Canvas Sidebar
4. '.sidebar-minimized'			- Minimized Sidebar (Only icons)
5. '.sidebar-compact'			  - Compact Sidebar

// Aside options
1. '.aside-menu-fixed'			- Fixed Aside Menu
2. '.aside-menu-hidden'			- Hidden Aside Menu
3. '.aside-menu-off-canvas'	- Off Canvas Aside Menu

// Breadcrumb options
1. '.breadcrumb-fixed'			- Fixed Breadcrumb

// Footer options
1. '.footer-fixed'					- Fixed footer

-->
<body class="app header-fixed sidebar-fixed aside-menu-fixed aside-menu-hidden">

    <!-- *APP-HEADER* -->
    @Html.Partial("_AppHeader")
    <!-- /*APP-HEADER* -->

    <div class="app-body">
        <div class="sidebar">

            <!-- *SIDEBAR-NAV* -->
            @Html.Partial("_SidebarNav")
            <!-- /*SIDEBAR-NAV* -->

            <button class="sidebar-minimizer brand-minimizer" type="button"></button>
        </div>

        <!-- *MAIN CONTENT* -->
        <main class="main">

            <!-- *BREADCRUMB* -->
            @Html.Partial("_BreadCrumb")
            <!-- /*BREADCRUMB* -->

            <!-- *CONTAINER-FLUID* -->
            <div class="container-fluid">

                <!-- *PAGE* -->
                @RenderBody()
                <!-- /*PAGE* -->

            </div>
            <!-- /*CONTAINER-FLUID* -->

        </main>
        <!-- /*MAIN CONTENT* -->

        <!-- *ASIDE-MENU* -->
        @Html.Partial("_AsideMenu")
        <!-- /*ASIDE-MENU* -->

    </div>
    <footer class="app-footer">
        <span><a href="http://coreui.io">CoreUI</a> © 2017 creativeLabs.</span>
        <span class="ml-auto">Powered by <a href="http://coreui.io">CoreUI</a></span>
    </footer>
    <!-- Bootstrap and necessary plugins -->
    <script src="~/vendors/js/jquery.min.js"></script>
    <script src="~/vendors/js/popper.min.js"></script>
    <script src="~/vendors/js/bootstrap.min.js"></script>
    <script src="~/vendors/js/pace.min.js"></script>
    <!-- Plugins and scripts required by all views -->
    <script src="~/vendors/js/Chart.min.js"></script>
    <!-- CoreUI main scripts -->
    <script src="~/js/app.js"></script>
    <!-- Plugins and scripts required by this views -->
    <!-- Custom scripts required by this view -->

</body>
</html>
```

{{<image src="/posts/images/devenv_2017-11-03_18-36-32.png">}}

Si en este momento volvemos a la dirección https://localhost:#####/CoreUI/Index, veremos la misma pantalla, pero esta vez como una composición del contenido principal sobre el layout y las parciales.

#### 3.3 - Convertir el resto de las páginas de CoreUI

La conversión del resto de las páginas en vistas razor es bastante sencilla, aunque hay algunos detalles que es mejor verlos directamente en el repositorio.

El trabajo básicamente es:

1. Dejar en cada vista sólo el contenido que está dentro del tag `<div class="container-fluid">`, ya que éste y todo lo demás está, directa o indirectamente, en **_Layout.cshtml** y

2. Cambiar la extensión a .cshtml

### 4 - Integrar vistas CoreUI con aplicación MVC

Ya para terminar, lo que falta, en esencia es:

1. Mover el _Layout y todas las vistas parciales a la carpeta Shared
2. Modificar los enlaces a las páginas de CoreUI para usar el controlador
3. Pasar la funcionalidad de _LoginPartial.cshtml a _UserNav.cshtml
4. Incluir en _Layout.cshtml manejo de estilos y scripts por vista
5. Adaptar el Carousel a Bootstrap 4

Más alguno que otro detalle menor que se puede ver mejor en el repositorio.

Con esto, finalmente llegamos a lo que buscábamos:

{{<image src="/posts/images/chrome_2017-11-03_23-29-54.png">}}

* El menú de la aplicación MVC en la barra de navegación superior
* El menú de CoreUI en el panel de navegación lateral
* El menú de usuario en la foto del perfil, con la opción de "Profile" y "Logout" funcionando

En resumen, creo que un buen punto de inicio para una interfaz de usuario elegante y atractiva en el próximo proyecto.

## Resumen

En este artículo vimos con bastante detalle el proceso de adaptación de una plantilla HTML estática, para facilitar el desarrollo de aplicaciones ASP.NET MVC atractivas.

En este proceso aprendimos un poco más sobre la estructura y el uso de los paquetes "client-side" en el desarrollo de las interfaces de usuario.

---

{{< goodbye >}}

---

### Enlaces relacionados

**Bootstrap 4**<br/>
https://getbootstrap.com/docs/4.0/getting-started/introduction/

**CoreUI**<br/>
http://coreui.io/

**CoreUI en GitHub**<br/>
https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template

**Gulp**<br/>
https://gulpjs.com/

**Instalación de la versión estática de CoreUI**<br/>
http://coreui.io/docs/getting-started/static-version/

**Node**<br/>
https://nodejs.org

**npm**<br/>
https://www.npmjs.com/

**Resharper**<br/>
https://www.jetbrains.com/resharper/


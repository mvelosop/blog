---
language: es
englishVersion: /posts/building-elegant-applications-aspnet-core-mvc-2.1-coreui-2-bootstrap-4/
title: Construyendo aplicaciones elegantes con ASP.NET Core MVC 2.1 y CoreUI 2 (Bootstrap 4)
draft: false
author: Miguel Veloso
date: 2018-05-21
description: Adaptar el template CoreUI 2.0.0, basado en BS 4 a una aplicación ASP.NET Core MVC 2.1, para mejorar la experiencia del usuario.
thumbnail: posts/images/asp.net-core-mvc-coreui-es.png
tags: [ "User Experience", "Client Side Development", "Bootstrap 4", "CoreUI" ]
repoName: AspNetCore2CoreUI
repoRelease: "2.0"
toc: true
image:
    authorName: Benjamin Child
    url: https://unsplash.com/photos/0sT9YhNgSEs
---

Esta es la versión **ACTUALIZADA** y **REVISADA** de mi artículo anterior: [Construyendo aplicaciones elegantes con ASP.NET MVC Core 2 y Bootstrap 4 usando CoreUI](/posts/construyendo-aplicaciones-elegantes-aspnet-mvc-core-2-bootstrap-4-coreui/)

En este artículo explicaré cómo adaptar la recién lanzada versión [(v2.0.0)](https://github.com/coreui/coreui-free-bootstrap-admin-template/tree/v2.0.0) del template 
 [CoreUI](http://coreui.io/), basado en [Bootstrap 4](http://getbootstrap.com/), para usarlo como base para aplicaciones ASP.NET MVC Core 2.1.

En realidad, "adaptar" es más que nada ejecutar mi primer programa en JS, [presentado como un PR al repositorio de CoreUI](https://github.com/coreui/coreui-free-bootstrap-admin-template/pull/379).

Aunque este es un artículo específico de ASP.NET Core MVC 2.1, la idea general debería, al menos, servir como guía para otros frameworks fuera del mundo .NET.

> {{< IMPORTANT "Puntos Importante" >}}

> 0. Conceptos clave sobre el manejo de paquetes del lado del cliente con **npm**

> 0. Comprender las relaciones entre las vistas principales, las vistas de layout y las vistas parciales.

> 0. Manejo de layouts anidados

> 0. Personalización de vistas de Identidad 2.1 desde paquetes de UI

{{< repoUrl >}}

## Contexto

Ha pasado un tiempo desde que escribí el primer artículo en CoreUI y en estas fechas (MAY-2017), hay nuevas versiones/candidatos tanto para ASP.NET MVC Core (v2.1) como para CoreUI (v2.0.0) y, además, he llegado a conocer un poco más sobre temas de front-end, así que pensé que sería un buen momento para publicar un artículo actualizado y revisado.

El proceso de adaptación de CoreUI va a ser un poco diferente al del artículo anterior, para empezar, ahora todo está centrado en **npm**, ya que **bower** y **gulp** han sido eliminados tanto de VS como de CoreUI y CoreUI está utilizando las capacidades de ejecución de tareas de npm.

Por otro lado, el proceso será mucho más rápido gracias a un programa de NODE que desarrollé y envié como PR a CoreUI. También espero que el proceso sea mucho más claro.

**Desde MAY-2018, ASP.NET Core 2.1 sólo es compatible con Visual Studio 2017 15.7 o posterior..**

No espero que este artículo se vea demasiado afectado por el lanzamiento final de todos los productos involucrados, pero lo actualizaré en caso de que sea necesario.

### Platforma y Herramientas

* [Visual Studio 2017 Community Edition v15.7 or later](https://www.visualstudio.com/thank-you-downloading-visual-studio/?ch=pre&sku=Community&rel=15)  
(go to [Visual Studio's download page](https://www.visualstudio.com/downloads/) for other versions).

* [EditorConfig Language Service for Visual Studio](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.EditorConfig)

* [CoreUI v2.0.0](https://github.com/coreui/coreui-free-bootstrap-admin-template)

* [.NET Core 2.1.0-rc1 with SDK 2.1.300-rc1 - x64 SDK Installer](https://download.microsoft.com/download/B/1/9/B19A2F87-F00F-420C-B4B9-A0BA4403F754/dotnet-sdk-2.1.300-rc1-008673-win-x64.exe)  
(go to [.NET Core's download page](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) for other versions).

* [Node 9.11.1](https://nodejs.org/)

* [npm 5.8.0](https://www.npmjs.com/)

El proceso de adaptación se simplifica mucho utilizando una herramienta de comparación de archivos. Recomiendo Beyond Compare, que he estado usando desde 1998, pero hay otras herramientas similares que deberían funcionar igual de bien.

* [Beyond Compare, from Scooter Software](http://www.scootersoftware.com/)

## Paso a paso

### 1 - Crear un proyecto ASP.NET MVC Core 2.1

Comencemos por crear una aplicación MVC estándar, usando la plantilla integrada de Visual Studio 2017.

#### 1.1 - Crear una solución en blanco

1. Crear una "solución en blanco" llamada **AspNetCore21CoreUI2** con un repositorio Git

1. Añada un archivo **.editorconfig** a la solución para estandarizar algunas opciones de formato para el proyecto. (después de instalar el [EditorConfig Language Services](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.EditorConfig))

1. Agregar un solution folder **src**

{{<image src="/posts/images/2018-04-25_19-27-17.png">}}

{{<renderSourceFile ".editorconfig">}} 

Ahora mismo su solución debería verse como esto:

{{<image src="/posts/images/devenv_2018-04-25_19-43-26.png">}}

#### 1.2 - Añadir un proyecto ASP.NET MVC Core 2.1

1. Cree el proyecto **CoreUI.Mvc** de tipo **ASP.NET Core Web Application** en la carpeta **src** de la solución:
{{<image src="/posts/images/devenv_2018-04-08_14-25-47.png">}}
y al buscar la carpeta, cree la carpeta **src** en el sistema de archivos:
{{<image src="/posts/images/2018-04-08_14-31-44.png">}}

2. Seleccione una aplicación de tipo **ASP.NET Core 2.1** **MVC** y 
{{<image src="/posts/images/devenv_2018-04-08_14-37-27.png">}}

3. Cambiar la autenticación a **Individual User Accounts** y **Store user accounts in-app**.
{{<image src="/posts/images/devenv_2018-04-08_14-40-18.png">}}

Ahora mismo su solución debería verse así:
{{<image src="/posts/images/devenv_2018-04-25_19-48-08.png">}}

#### 1.3 - Crear la base de datos

1. Cambie el string de conexión en el archivo `appsettings.json` para usar un nombre más "bonito" para la base de datos.
{{<renderSourceCode "text" "linenos=table">}}
Server=(localdb)\\mssqllocaldb; Database=CoreUI.Mvc; Trusted_Connection=True; MultipleActiveResultSets=true
{{</renderSourceCode>}}

1. Ejecute la aplicación usando [Ctrl]+[F5]

1. Regístrese para forzar la creación de la base de datos

1. Haga click en **Apply Migrations**, cuando se presente el error por falta de la base de datos:
{{<image src="/posts/images/chrome_2018-04-08_14-56-23.png">}}

1. Refresque la pantalla (re-posteando el registro) cuando termine el proceso de creación de la base de datos, para completar el registro del usuario
{{<image src="/posts/images/chrome_2018-04-08_14-58-08.png">}}

**Este es un buen momento para guardar el proyecto en el repositorio!**

#### 1.4 - Eliminar bibliotecas originales del lado del cliente

La carpeta **wwwroot** es la raíz del lado cliente de la aplicación, por lo que todos los archivos estáticos deben estar dentro de este árbol de carpetas.

Ahora vamos a eliminar todas las librerías del lado del cliente incluidas por la plantilla de Visual Studio, porque vamos a usar las de CoreUI.

1. Abra la carpeta wwwroot y anote las librerías utilizadas:
{{<image src="/posts/images/devenv_2018-04-08_15-13-44.png">}}
La carpeta bootstrap contiene todo el "look and feel" general de la aplicación, pero esto es lo que va a ser reemplazado por CoreUI, así que vamos a ignorar completamente esta carpeta. <br /><br />
CoreUI está basado en Bootstrap, pero crea un **style.css** adaptado, compilando los archivos **.scss** de Bootstrap.  <br /><br />
Las librerías **jquery*** se utilizan para la interactividad y validación del lado del cliente, por lo que más adelante las añadiremos a la aplicación MVC CoreUI, pero sólo tomaremos nota de las librerías utilizadas, podemos comprobar las versiones en el archivo **.bower.json** en cada carpeta, en este caso:
    - jquery (3.3.1)
    - jquery-validation (1.17.0)
    - jquery-validation-unobtrusive (3.2.9)

1. Elimine la carpeta **wwwroot\lib**.  
<small>(Si no la puede eliminar, podría ser necesario cerrar VS y hacerlo desde el explorador de archivos.) </small>

3. Ejecute la aplicación con [Ctrl]+[F5] para mostrarla sin ningún estilo (o Javascript)
{{<image src="/posts/images/chrome_2018-04-08_15-40-50.png">}}

**Guardemos esta versión en el repositorio ahora**

### 2 - Prepare el sitio de despliegue de CoreUI

Ahora vamos a preparar el sitio de despliegue (distribución) a partir de la última versión de CoreUI, que luego copiaremos a nuestra aplicación ASP.NET Core MVC.

#### 2.1 - Prepare su repositorio base CoreUI

En este momento tiene dos opciones:

1. Clonar [el repositorio maestro de CoreUI en GitHub](https://github.com/coreui/coreui-free-bootstrap-admin-template) para aprender el proceso en detalle o

1. Clonar [mi fork, específico para ASP.NET Core MVC 2.1, del repositorio de CoreUI en GitHub](https://github.com/mvelosop/coreui-free-bootstrap-admin-template) para obtener resultados más rápidamente.

Mi fork agrega un conjunto de programas Node (Javascript) para generar vistas básicas de Razor a partir de los archivos html estáticos de CoreUI, con sólo un comando.

Ya he [enviado un PR](https://github.com/coreui/coreui-free-bootstrap-admin-template/pull/379) para incluir esto en el repositorio maestro de CoreUI pero, mientras tanto, Supongo que prefirirá la ruta rápida, así que explicaré los pasos suponiendo que está clonando mi [fork de CoreUI](https://github.com/mvelosop/coreui-free-bootstrap-admin-template).

De todos modos en esta sección (#2) debería obtener casi los mismos resultados de ambos repositorios, la sección #3 es donde se mostrarán las grandes diferencias.

Entonces, comencemos por clonar mi fork, desde la línea de comandos ejecute:

{{<renderSourceCode "bat" "linenos=table">}}
git clone https://github.com/mvelosop/coreui-free-bootstrap-admin-template mvelosop-coreui-free-bootstrap-admin-template
{{</renderSourceCode>}}

En mi fork, la rama por defecto es `aspnetcore` (específica para ASP.NET Core MVC), mientras que la rama por defecto en el repositorio maestro es `master`.

Ahora vamos a crear una nueva rama para hacer nuestras personalizaciones, entonces vamos a llamarla **dist**:

{{<renderSourceCode "bat" "linenos=table">}}
cd ./mvelosop-coreui-free-bootstrap-admin-template
git checkout -b dist aspnetcore
{{</renderSourceCode>}}

#### 2.2 - Crear la carpeta de distribución (archivos estáticos estándar)

Primero necesitamos instalar las dependencias del lado del cliente, así que, desde la carpeta **coreui-freebootstrap-admin-template**, ejecute este comando:

{{<renderSourceCode "bat" "linenos=table">}}
npm install
{{</renderSourceCode>}}

Esto crea la carpeta **node_modules** con todas las dependencias, tal y como se ha configurado en la colección **dependencias** en el archivo **package.json**. 

Mi fork agrega algunos scripts a **package.json**, y algunos paquetes adicionales:

* **build-aspnetcore** : Este es el que hace la magia, y
* **test** * Esta es la que se asegura de que la magia esté bien ;-)

Ejecute este comando para verificar que el sitio funciona correctamente:

{{<renderSourceCode "bat" "linenos=table">}}
npm run serve
{{</renderSourceCode>}}

Eso debería abrir su navegador por defecto en http://localhost:3000/ donde debería ver algo como esto:
{{<image src="/posts/images/chrome_2018-04-08_19-54-45.png">}}

Para generar finalmente la carpeta de distribución basta con ejecutar el comando (tras interrumpir el servidor de node con [Ctrl]+[C]):

{{<renderSourceCode "bat" "linenos=table">}}
npm run build
{{</renderSourceCode>}}

Esto crea la carpeta **dist** (.gitignore'd) dentro de la carpeta repo con algo como esto:
{{<image src="/posts/images/explorer_2018-04-09_13-47-00.png">}}

Este es el sitio de despliegue estático base, listo para ser publicado, y con sólo hacer doble clic en cualquier archivo html (excepto 404, 500, login o register) puede explorar la plantilla CoreUI base.

Hasta este punto, obtendrá el mismo resultado con el repo maestro o con mi fork.

#### 2.3 - Crear la carpeta de distribución para ASP.NET Core MVC

Similar a lo que acabamos de hacer, para crear la carpeta de distribución para ASP.NET Core MVC, **cuando se clona mi fork**, sólo es necesario ejecutar:

{{<renderSourceCode "bat" "linenos=table">}}
npm run build-aspnetcore
{{</renderSourceCode>}}

Este comando borra la carpeta **dist** anterior y genera algo como esto, siguiendo las convenciones de ASP.NET:
{{<image src="/posts/images/explorer_2018-04-26_15-37-04.png">}}

Donde podemos destacar lo siguiente:

1. Las imágenes están ahora en la carpeta **images** en lugar de **img**.

1. Los archivos de los paquetes requeridos están en la carpeta **lib** en lugar de **vendors**.

1. La carpeta **lib** está ahora organizada de la misma manera que **node_modules** (pero incluyendo sólo los archivos realmente referenciados, más cualquier archivo **.map** existente y requerido)

1. Una vista Razor (**.cshtml**) por cada archivo html estático de CoreUI.

Al igual que antes, podemos hacer doble clic en cualquier archivo html (excepto 404, 500, login o register) para explorar la plantilla CoreUI base, con la estructura de carpetas ASP.NET Core MVC.

Si abrimos un par de archivos .html/.cshtml con Beyond Compare, o un comparador de archivos similar, podemos comprobar las diferencias para entender los cambios que hace el script **build-aspnetcore**:

{{<image src="/posts/images/BCompare_2018-04-26_15-53-10.png">}}

{{<image src="/posts/images/BCompare_2018-04-26_15-55-23.png">}}

1. Borrar el layout por defecto porque (en este momento) la página tiene todo lo que necesita

2. Escapar el caracter `@` (un símbolo reservado de Razor)

3. Añadir el prefijo de ruta "home" `~/` para los archivos css y js

4. Añadir el prefijo de ruta "home" `~/` para las imágenes y todos los demás archivos estáticos

5. Cambiar los enlaces a los archivos html, para apuntar a las acciones del controlador ASP.NET usando [tag helpers de Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-2.1)

**Si no estuviese usando el script `build-aspnetcore` de mi fork, tendría que hacer esos cambios a mano en todos y cada uno de los archivos html.**

#### 2.4 - Instalar las dependencias del lado del cliente para las vistas ASP.NET MVC

Ahora instalaremos las dependencias identificadas en [1.4](#1-4-delete-original-client-side-libraries).

Para instalar las dependencias sólo tenemos que añadir estas líneas a la colección **dependencies** de **packages.json**, haciendo referencia a las últimas versiones de los paquetes:

{{<renderSourceCode "json" "linenos=table">}}
"jquery":"3.3.1",
"jquery-validation":"1.17.0",
"jquery-validation-unobtrusive":"3.2.9",
{{</renderSourceCode>}}

Así que el archivo debería resultar en algo como esto:
{{<image src="/posts/images/Code_2018-04-09_13-09-42.png">}}

Si prefija un número de versión con el símbolo `^`, la versión menor y el número de actualización podrían cambiar cada vez que ejecute el comando **install**.

Luego, tenemos que instalar las nuevas dependencias usando:

{{<renderSourceCode "bat" "linenos=table">}}
npm install
{{</renderSourceCode>}}

Una de las tareas de **build-aspnetcore** es copiar todos los archivos de paquetes referenciados de **node_modules** a **lib**, así que creamos un archivo llamado **vendors.html** (o cualquier otro) para incluir los archivos que vamos a referenciar desde las vistas Razor de la aplicación, así:

{{<renderSourceCode "html" "linenos=table">}}
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8" />
  <title>Vendor list</title>
</head>

<body style="font-family: Arial, Helvetica, sans-serif; font-size: 18px;">

<h3>Files to deploy to dist/lib</h3>
<ul>
  <li>"node_modules/jquery-validation/dist/jquery.validate.min.js"</li>
  <li>"node_modules/jquery-validation/dist/additional-methods.js"</li>
  <li>"node_modules/jquery-validation/dist/localization/messages_es.js"</li>
  <li>"node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js"</li>
</ul>

</body>

</html>
{{</renderSourceCode>}}

Este archivo se escaneará con el comando **build/build-aspnetcore** para seleccionar los archivos que se copiarán de **node_modules** a **dist/lib**.

**Es importante (para que el script funcione) incluir la ruta relativa y el nombre del archivo, desde la carpeta de inicio, incluyendo la misma carpeta "node_modules", entre comillas.**

El programa también incluirá el archivo **.map** relacionado (si existe).

Así que ahora podemos crear/actualizar la carpeta de distribución con:

{{<renderSourceCode "bat" "linenos=table">}}
npm run build-aspnetcore
{{</renderSourceCode>}}

> {{< IMPORTANT "Gestores de paquetes client-side" >}}.

> - En algún momento usted podría querer usar algún gestor de paquetes especializado del lado del cliente, como WebPack, pero no vamos a cubrir eso en este artículo.

#### 2.5 - Copiar la carpeta de distribución en la solución

Ahora copiaremos el contenido de la carpeta **dist** en la nueva carpeta **src\CoreUI** de nuestra solución.

El **src\CoreUI** será nuestra **carpeta de referencia** que usaremos para comparar con las nuevas versiones de CoreUI, cuando estén disponibles, para actualizar los componentes de nuestra aplicación según sea necesario.

La solución VS debería verse así:

{{<image src="/posts/images/explorer_2018-05-11_18-08-35.png">}}

En un momento fusionaremos esa carpeta **src\CoreUI** a las carpetas **wwwroot** y **Views** de nuestra aplicación.

**Este es un momento excelente para guardar el proyecto en su repositorio local!**

> {{< IMPORTANT "src\CoreUI no es parte de la solución en Visual Studio" >}}.

> 0. Note que, aunque src\CoreUI está dentro de la estructura de carpetas de la solución y bajo control de versiones, no es parte de la solución de Visual Studio, es decir, no lo verá en el explorador de la solución.

### 3 - Integrar la carpeta de referencia CoreUI en la aplicación MVC

Primero haremos una integración básica, sólo para verificar que todas las vistas de Razor funcionen correctamente:

1. Copiar todos los archivos estáticos, que no sean .html, a la carpeta **wwwwroot**.
2. Crear un controlador genérico para mostrar todas las vistas de Razor
3. Copiar las vistas **Razor** a la carpeta Views del controlador

Luego reorganizaremos las vistas usando parciales, para que se parezca más a una aplicación estándar de Razor MVC.

Aquí es donde una herramienta como Beyond Compare resulta muy útil, especialmente a la hora de actualizar los archivos a nuevas versiones de CoreUI.

#### 3.1 - Copiar archivos estáticos a la carpeta wwwroot

Así que tenemos que copiar estas carpetas de **src\CoreUI** a **src\CoreUI.Mvc\wwwwroot**:

- css 
- imágenes
- js
- vendors

Esta carpeta **vendors** en realidad contiene archivos css específicos de CoreUI, pero no quería cambiar ninguno de los scripts estándar de la plantilla.

Si se utiliza [Beyond Compare](http://www.scootersoftware.com/), el resultado debería ser algo así:

{{<image src="/posts/images/BCompare_2018-05-11_18-36-03.png">}}

#### 3.2 - Crear un controlador genérico para las vistas CoreUI

A continuación crearemos un controlador simple para mostrar cualquiera de las vistas Razor de CoreUI (***.cshtml**), que sólo recibe el nombre de la vista a mostrar.

También necesitamos crear la carpeta **Views\CoreUI** correspondiente, para las vistas Razor de este controlador.

#### 3.3 - Copiar las vistas de Razor a la carpeta Views\CoreUI

Usando BeyondCompare copie las vistas de Razor generadas desde la carpeta de referencia CoreUI a la carpeta Views\CoreUI:

{{<image src="/posts/images/BCompare_2018-05-11_19-03-04.png">}}

#### 3.4 - Ejecute la aplicación

Ahora debería poder ejecutar la aplicación con [Ctrl]+[F5] y al navegar a https://localhost:#####/CoreUI/Index (###### = puerto asignado por VS) para obtener algo así:

{{<image src="/posts/images/chrome_2018-05-11_20-13-18.png">}}

Debería poder navegar a cualquier página de la plantilla y todas las peticiones serán manejadas por el controlador, lo que puede verificar en la barra de direcciones.

Una vez más, este es un buen momento para comprometer tu trabajo.

#### 3.5 - Crear una vista _Disposición

Cuando se trabaja en Razor, la página renderizada normalmente tiene, al menos, dos partes:

1. El **contenido**, que es el núcleo de cualquier cosa que quieras mostrar en un momento dado, por ejemplo, la página de índice, y puede llamarse de cualquier manera que tenga sentido, por ejemplo, **Index.cshtml**.

1. El **layout**, que "rodea" el contenido y se suele llamar **_Layout.cshtml**, pero no es más que otra vista de Razor que usamos de forma diferente.

Correlacionando esto con las vistas de CoreUI Razor generadas, es fácil darse cuenta que para cualquier vista (excepto 404, 500, login y registro):

El layout es equivalente a **blank.cshtml**.
El contenido es equivalente a la diferencia entre la vista y **blank.cshtml**.

Así que empecemos copiando **blank.cshtml** a **_Layout.cshtml** y añadiendo el render body helper en **_Layout.cshtml** así:

{{<image src="/posts/images/devenv_2018-05-11_22-06-37.png">}}

En ASP.NET MVC, cuando solicitamos una vista del controlador, el motor de renderizado:

1. Encuentra la vista solicitada
2. Encuentra el layout configurado para la vista
3. Presenta el layout insertando el contenido de la vista en el helper **`@RenderBody()`**.

Así que vamos a comparar ahora **blank.cshtml** (izquierda) con **index.cshtml** (derecha), debería obtener algo como esto:

{{<image src="/posts/images/BCompare_2018-05-11_20-51-32.png">}}

OEn la vista de miniaturas de la izquierda podemos identificar cuatro zonas:

1. Antes de la zona de contenido del índice, ambos archivos son idénticos
2. En la zona de contenido del índice el **blank.cshtml** no tiene líneas
3. Después de la zona de contenido del índice, ambos archivos son idénticos de nuevo
4. Hay algunas líneas adicionales al final del archivo **index.cshtml**.

Así que al borrar las líneas comunes de las zonas **1**, **2** y **las dos últimas líneas** en el archivo **index.cshtml** (a la derecha) debería llegar a esto:

{{<image src="/posts/images/BCompare_2018-05-11_21-48-08.png">}}

Observe las cuatro líneas al final que existen sólo en **index.cshtml**.

La forma de resolver esto es creando una sección **Scripts** en el archivo **index.cshtml** y añadiendo una llamada al helper **RenderSection** en el archivo **_Layout.cshtml**.

Después de arreglar eso, las últimas líneas de **Index.cshtml** deberían ser:

{{<image src="/posts/images/devenv_2018-05-11_21-59-24.png">}}

Y ahora las últimas líneas de **_Layout.cshtml** deberían ser:

{{<image src="/posts/images/devenv_2018-05-11_22-03-47.png">}}

Por lo tanto, si ahora ejecutamos la aplicación de nuevo y refrescamos la página https://localhost:#####/CoreUI/Index, deberíamos obtener algo así:

{{<image src="/posts/images/chrome_2018-04-10_21-34-56.png">}}

Que se ve igual que antes, sólo que esta vez hemos separado el contenido del layout.

Pruebe comentar la línea `@RenderBody()` que agregamos a **_Layout.cshtml** para comprobar lo que sucede, en caso de que no esté claro en este punto.

También puede buscar los comentarios del marcador de contenido que agregamos a **_Layout.cshtml**.

**Este es otro buen momento para guardar su trabajo**

#### 3.6 - Convertir el resto de las páginas de CoreUI

Ahora puede repetir el proceso con el resto de las páginas de CoreUI, a excepción de las páginas 404, 500, login y register, que no tienen layout.

Hay un par de detalles adicionales, pero no voy a cubrirlos aquí, puede revisar las vistas finales en el repositorio.

#### 3.7 - Componentizar la vista _Layout

No es práctico tener una vista _Layout masiva de más de 700 líneas, por lo que es mejor dividirla en componentes más pequeños.

No voy a pasar por todo el proceso aquí, sino que sólo mostraré el resultado final, incluyendo la fusión con el archivo **_Layout** por defecto de ASP.NET Core MVC, que puede encontrar en la carpeta **Views\Shared** y puede revisar todos los detalles en el repositorio del artículo:

{{<renderSourceFile "src\CoreUI.Mvc\Views\Shared\_Layout.cshtml">}}

Vale la pena notar que los Breadcrumbs son manejados como una sección en la vista, más que como una vista parcial, porque probablemente están vinculados a la vista. En caso de que no lo sean, ahora debería poder adaptarlo a sus necesidades.

De todos modos veremos en un momento sobre una forma interesante de manejar esto usando layouts anidados.

**Nuevamente, este es un buen momento para salvar su trabajo**

### 4 - Integrar las vistas CoreUI y la aplicación MVC

Sólo faltan unos pocos pasos para terminar la integración con la aplicación ASP.NET Core MVC.

1. Mover **_Layout.cshtml** y las parciales a la carpeta **Views\Shared

1. Mostrar las vistas MVC estándar con el nuevo **_Layout.cshtml**.

1. Integrar las vistas de Account con la plantilla CoreUI

El ítem #1 es algo trivial, sólo vale la pena notar que la vista **_app-app-header-nav.cshtml** contendrá ahora el menú original de ASP.NET Core MVC:

{{<renderSourceFile "src\CoreUI.Mvc\Views\Shared\_app-header-nav.cshtml">}}

Usted puede, por supuesto, ajustar esto a lo que mejor se adapte a sus necesidades.

También vale la pena señalar aquí, que el elemento #2 sólo ocurre porque el layout de las vistas está configurado para ser **_Layout.cshtml** en:

{{<renderSourceFile "src\CoreUI.Mvc\Views\_ViewStart.cshtml">}}

Pero no queremos (sólo por diversión) mostrar la vista de breadcrumbs en las vistas estándar de ASP.NET MVC.

Resolveremos esto usando "nested layouts" para probar una característica interesante de ASP.NET MVC.

#### 4.1 - Aplicación de layouts anidados para breadcrumbs

Al dividir el diseño, se hizo evidente que era necesario algo especial para manejar los breadcrumbs, ya que son algo que tiene que adaptarse al contexto, probablemente para cada vista.

Así que era obvio que los breadcrumbs deberían ser una sección de la vista, pero añadirlo a todas las vistas de CoreUI era un poco tedioso.

Así que una solución interesante para esto fue añadir un layout local en la carpeta **Views\CoreUI** para incluir los breadcrumbs y separar los elementos y el menú de los breadcrumbs, así:

{{<renderSourceFile "src\CoreUI.Mvc\Views\CoreUI\_CoreUILayout.cshtml">}}

Para configurar el layout para todas las vistas CoreUI, sólo necesitamos añadir este archivo en la carpeta **Views\CoreUI**: 

{{<renderSourceFile "src\CoreUI.Mvc\Views\CoreUI\_ViewStart.cshtml">}}

Así es como estas partes trabajan juntas en la carpeta **Views\CoreUI**:

1. El contenido de la vista se muestra en **@RenderBody()** en el layout **_CoreUILayout**.

2. La sección **Scripts** se muestra en la línea 32 de **_CoreUILayout.cshtml**.

3. Pero, como la línea 32 está dentro de otra sección **Scripts**, se renderizará en la línea 133 en **_Layout.cshtml**.

4. Porque **_Layout.cshtml** es el layout para la vista **_CoreUILayout.cshtml**, tal y como se configuró en su primera línea.

De esta manera, cualquier vista puede definir una sección **Breadcrumbs**, igual que las líneas 9-21 en **_CoreUILayout.cshtml**, y esto podría ser:

1. Una vista parcial local (p. ej. **_breadcrumb-items**)
2. Una vista compartida (ej. **_breadcrumbs-menu**) 
3. Sólo el markup html requerido justo ahí o
4. No usar breadcrumbs en absoluto, como en las vistas iniciales de Razor creadas por VS.

Un último detalle, para manejar correctamente los breadcrumbs de esta manera, es necesario añadir un pequeño ajuste a la css en **src\CoreUI.Mvc\wwwroot\css\site.css**:

{{<renderSourceCode "css" "linenos=table">}}
.container-fluid > .animated {
    margin-top: 24px !important;
}
{{</renderSourceCode>}}

Por lo tanto, no es de extrañar que esta sea la vista **blank.cshtml** resultante, destacando que todas las secciones que se muestran son sólo marcadores y opcionales, es decir, se pueden omitir por completo.

{{<renderSourceFile "src\CoreUI.Mvc\Views\CoreUI\blank.cshtml">}}

Si ahora regresamos a https://localhost:#####/CoreUI/Index, veremos la misma vista, pero esta vez como una composición del contenido principal del layout y de las vistas parciales.

Y ahora con el menú MVC en la barra superior y los breadcrumbs sólo en las vistas CoreUI.

#### 4.2 - Integrar las vistas "Account" de Identity

> {{< IMPORTANT "Personalizanfo las vistas de Identity 2.1" >}}.

> - Comenzando con ASP.NET Core 2.1 hay una nueva característica que permite incluir vistas de Razor en librerías de clases, por lo que las vistas ya no se generan, y por lo tanto debe seguir este procedimiento para personalizarlas en caso de que lo necesite.

Sólo falta un paso más para integrar las vistas estándar de Identity con CoreUI.

No voy a entrar en detalles aquí, pero los pasos de "alto nivel", usando **_LoginPartial.cshtml** como guía, son: **_LoginPartial.cshtml**:

1. Mostrar todos los elementos relacionados con el usuario en la parcial **_app-header.cshtml** sólo cuando el usuario está conectado y
2. Agregar enlaces a las opciones de Profile y Logout en la parcial **_user-nav.cshtlm** y, finalmente
3. Eliminar **_LoginPartial.cshtml**

Así que el resultado final, cuando el usuario no está conectado debería ser así:

{{<image src="/posts/images/chrome_2018-05-21_13-23-35.png">}}

#### 4.3 - Personalizar vistas de identidad

Desde VS 2017 v15.7 y posteriores, las plantillas de proyecto ASP.NET Core MVC utilizan las vistas de identidad por defecto del paquete **Microsoft.AspNetCore.Identity.UI** así que fue necesario personalizar un poco siguiendo las instrucciones en https://blogs.msdn.microsoft.com/webdev/2018/04/12/asp-net-core-2-1-0-preview2-now-available/#user-content-customize-default-identity-ui.

Para mostrar las opciones de navegación en la vista de perfil de usuario fue necesario personalizar la vista **AccountManageManageManageNav** para aplicar las clases CoreUI correctas de esta manera:

{{<renderSourceFile "src\CoreUI.Mvc\Areas\Identity\Pages\Account\Manage\_ManageNav.cshtml">}}

Además de algunos otros detalles que puede revisar mejor en el repositorio.

Para obtener una vista del perfil de usuario como esta:

{{<image src="/posts/images/chrome_2018-05-21_17-05-58.png">}}

Así que, en este punto, creo que llegamos a un buen punto de partida para tener una interfaz de usuario elegante y atractiva para su próximo proyecto.

## Resumen

En este artículo estudiamos en detalle el proceso de adaptación de la última versión de la plantilla HTML CoreUI para facilitar el desarrollo de aplicaciones atractivas en ASP.NET Core MVC 2.1.

---

{{< goodbye >}}

---

### Enlaces relacionados

**.NET Core 2.1.0-rc1 with SDK 2.1.300-rc1 - x64 SDK Installer**<br/>
https://download.microsoft.com/download/B/1/9/B19A2F87-F00F-420C-B4B9-A0BA4403F754/dotnet-sdk-2.1.300-rc1-008673-win-x64.exe)  

**Beyond Compare, from Scooter Software**<br/>
http://www.scootersoftware.com/

**Bootstrap 4.1**<br/>
https://getbootstrap.com/docs/4.1/getting-started/introduction/

**CoreUI 2.0.0 in GitHub**<br/>
https://github.com/coreui/coreui-free-bootstrap-admin-template/tree/v2.0.0

**CoreUI build-aspnetcore PR in GitHub**<br/>
https://github.com/coreui/coreui-free-bootstrap-admin-template/pull/379

**Customize default ASP.NET Core 2.1 Identity UI**<br/>
https://blogs.msdn.microsoft.com/webdev/2018/04/12/asp-net-core-2-1-0-preview2-now-available/#user-content-customize-default-identity-ui

**EditorConfig Language Service for Visual Studio**<br/>
https://marketplace.visualstudio.com/items?itemName=MadsKristensen.EditorConfig

**Node**<br/>
https://nodejs.org/

**npm**<br/>
https://www.npmjs.com/

**Razor tag helpers**<br/>
https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-2.1



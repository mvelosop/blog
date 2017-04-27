---
title: Tareas de fondo en aplicaciones ASP.NET MVC Core
draft: true
author: Miguel Veloso
date: 2017-04-24
description: Cómo implementar tareas en background usando HangFire
thumbnail: posts/images/plumbing-840835_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp" ]
series: [ ]
repoName: HangFireCoreWebApp
#repoRelease: "1.0"
---

Es este artículo vamos a desarrollar la estructura de una aplicación web en .NET Core, para analizar cómo se integra la librería [HangFire](https://www.hangfire.io) (https://www.hangfire.io)

> ### <i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Puntos principales

> 0. Incorporar Hangfire como gestor de tareas en backgroud.
> 0. Incorporar NLog para realizar registro de eventos.
> 0. Cargar assemblies en forma dinámica
> 0. Usar reflection para identificar clases en un assembly.
> 0. Crear atributos para especificar metadata al usar las clases.
> 0. Crear/Actualizar la base de datos al arrancar la aplicación.

{{< repoUrl >}}

## Contexto

Al desarrollar aplicaciones web es frecuente que sea necesario ejecutar tareas de fondo, como envío de correos o tareas de mantenimiento y, según lo comentado por [Scott Hanselman](https://www.hanselman.com/blog/HowToRunBackgroundTasksInASPNET.aspx), aunque esto puede parecer sencillo cuando se utilizan librerías como [Quartz.NET](https://www.quartz-scheduler.net/), puede llegar a ser complicado por la cantidad de escenarios que se deben manejar.

En su [artículo](https://www.hanselman.com/blog/HowToRunBackgroundTasksInASPNET.aspx), Hanseman menciona la librería [HangFire](https://www.hangfire.io) (https://www.hangfire.io) y la verdad es que ofrece una solución muy completa, incluso viene con una interfaz web para gestionar los procesos programados. Esta interfaz facilita la gestión de las tareas programas y requiere muy poco esfuerzo ponerla en funcionamiento.

Además, creo que la interfaz de gestión se ve muy bien y, dado que está principalmente orientada a usuarios de sistemas, tampoco es tan importante que el aspecto sea diferente del resto de la aplicación.

Entonces, la idea en este artículo es desarrollar una estructura donde sea muy sencillo manejar las tareas programadas para una aplicación modular, donde cada módulo puede tener assemblies con una convención de nombre que permita incorporarlos y programar sus tareas, con solo copiar esos assemblies en la carpeta con los ejecutables (.dll) de la aplicación.

### Herramientas utilizadas

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## Paso a paso

### 1) Crear la solución HangFireCoreWebApp

1. Crear una solución "blank" llamada HangFireCoreWebApp
2. Crear el "solution folder" "src"
3. Crear la carpeta "src" dentro de la carpeta de la solución

### 2) Desarrollar atributo para facilitar la programación de las tareas

La idea es utilizar el atributo ```[HangfireJobMinutes(#)]``` para, para indicar la frecuencia de ejecución.

Este es un atributo muy sencillo, a modo de ejemplo, para aplicaciones reales seguramente habrá que manejar escenarios más complejos.

1. **Crear proyecto "src\HangFireCore.Core"**

2. **Agregar archivo "HangfireJobMinutesAttribute.cs"** </br>
{{<getSourceFile "src\HangFireCore.Core\HangfireJobMinutesAttribute.cs">}}

### 3) Desarrollar un "módulo" con tareas programadas

Este es un "módulo" que sólo tiene una tarea programada, para demostrar la funcionalidad básica.

En el repositorio se incluye también el módulo **HangFire.Job.Two**, pero no se incluye en el artículo.

> ### <i style="font-size: larger" class="fa fa-exclamation-triangle" aria-hidden="true"></i> Advertencia

> Cuando hablamos de módulos dinámicos es importante tener en cuenta que al compilarlos no se van a copiar los .dll a la carpeta de la aplicación web, por lo que hay que hacerlo a mano para ver los cambios.

1. **Crear proyecto "src\HangFireCore.Job.One"**

2. **Incluir referencias y paquetes necesarios** </br>
   * referencia al proyecto **src\HangFireCore.Core**
   * Paquete **NLog.5.0.0-beta06**

3. **Incluir archivo "JobOne.cs"**
{{<getSourceFile "src\HangFireCore.Job.One\JobOne.cs">}}

### 4) Crear el proyecto src\HangFireCore.WebApp

En la aplicación web se integrarán los componentes de la solución.

La idea de la estructura presentada es que no existe un referencia directa entre la aplicación web y los proyectos (assemblies) de tareas programadas, sino que al arrancar, la aplicación busque los assemblies y los cargue automáticamente, programando su ejecución según los atributos utilizados.

Como no hay un referencia directa de esos assemblies (en este caso sólo HangFire.Job.One.dll), estos no serán incluidos directamente por el proceso de "Build" y entonces es necesario copiarlos a mano en la carpeta de la aplicación.

1. **Crear un projecto .NET Core Web Application (.NET Core)**

    {{<image src="/posts/images/devenv_2017-04-25_11-01-58.png">}}

    Usamos el proyecto con autenticación por cuentas individuales, para que se cree todo lo relacionado con la conexión a la base de datos, porque la vamos a necesitar para el próximo paso.

### 5) Agregar carga dinámica de "módulos"

Se implementa la carga de "modulos" como un [ExtensionMethod](https://msdn.microsoft.com/en-us//library/bb383977.aspx) de **IHostingEnvironment** para facilitar su invocación desde **Startup.cs**.

1. **Agregar la clase "helpers\ScheduleJobsHelpers.cs** </br></br>
   {{<getSourceFile "src\HangFireCore.WebApp\helpers\ScheduleJobsHelpers.cs">}}

### 6) Incluir HangFire en la aplicación web

Vamos a orientarnos por lo indicado en [Integrate HangFire With ASP.NET Core](http://dotnetthoughts.net/integrate-hangfire-with-aspnet-core/), para configurar Hangfire:

1. **Incluir el paquete NuGet Hangfire 1.6.12**

2. **Modificar el archivo de configuración para trabajar con SQL Server en vez de LocalDb**  
   {{<getSourceFile "src\HangFireCore.WebApp\appsettings.json">}}  
   Prefiero trabajar con SQL Server Developer Edition, para que sea lo más parecido posible al ambiente de producción y eliminamos la sección de "Logging", porque vamos a utilizar el archivo NLog.config, según veremos más adelante.

3. **Crear un AuthorizationFilter para poder acceder al dashboard de Hangfire**
   {{<getSourceFile "src\HangFireCore.WebApp\Helpers\HangfireDashboardAuthorizationFilter.cs">}}  
   Esto es un filtro básico que sólo verifica que el usuario esté autenticado para permitir el acceso, en la práctica se debe aplicar algún criterio más estricto para permitirlo.

### 7) Incluir NLog en la aplicación web

Vamos a utilizar [NLog](http://nlog-project.org/) (http://nlog-project.org/) para verificar el funcionamiento del sistema de tareas en background y como buena práctica general del desarrollo de aplicaciones.

1. **Incluir paquetes NLog en la aplicación web**

    * NLog.Web.AspNetCore 4.3.1
    * NLog.Config 4.4.7

2. **Resolver problema por incompatibilidad con ASP.NET Core** </br>  
Según lo indicado en https://github.com/NLog/NLog.Extensions.Logging/blob/master/README.md (para el 25/04/2017), la instalación de NLog.Config no copia el archivo NLog.xsd, así que hay que extraerlo manualmente del nuget y copiarlo en la raíz del proyecto **HangFireCore.WebApp**. </br>  
Para esto se debe buscar el paquete el la carpeta **.nuget** dentro del perfil del usuario y abrir el .nupkg correspondiente con un gestor de archivos .zip como WinRAR o WinZIP o cambiando temporalmente el nombre del .nupkg a .zip para usar el explorador de Windows.

3. **Crear archivo NLog.config en la raíz de la aplicación web**  
   {{<getSourceFile "src\HangFireCore.WebApp\NLog.config">}}  
Según lo indicado en https://github.com/NLog/NLog.Extensions.Logging/blob/master/README.md, todavía no está soportado el ```${basedir}```, para manejar rutas relativas en los archivos .log, así que por ahora es necesario configurar una ruta absoluta (c:\temp\logs).  </br></br>
Un aspecto interesante de NLog es que se puede modificar el archivo de configuración sin necesidad de detener la aplicación, por ejemplo para ajustar el nivel de detalle que se registra, para ver los resultados de inmediato.</br></br>
Esta configuración crea archivos de log rotativos en c:\temp\logs:
    * **nlog-all-current.log** día en curso para todas las aplicaciones (bueno para detectar interferencias)
    * **nlog-all-archive.1.log** histórico del día -1 para todas las aplicaciones
    * **nlog-HangFireCoreApp-current.log** día en curso para nuestra aplicación
    * **nlog-HangFireCoreApp-archive.1.log** histórico del día -1 para nuestra aplicación

### 8) Configurar Hangfire y NLog en la aplicación web

Aquí se detallan las modificaciones individuales y luego se muestra el archivo **Startup.cs** resultante.

1. **Agregar el método InitDb en Startup.cs para crear/actualizar la base de datos**    
```cs
    private void InitDb()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

        using (var dbContext = new ApplicationDbContext(optionsBuilder.Options))
        {
            dbContext.Database.Migrate();
        };
    }
```  
Este método se usa para crear/actualizar la base de datos cuando arranque la aplicación y no con el primer request, porque debe existir la base de datos antes de iniciar Hangfire.

2. **Agregar líneas para configurar Hangfire** </br> </br>
Estos son los cambios necesarios, en los distintos métodos de Startup.cs, para configurar Hangfire:
```cs
    using Hangfire;
    using HangFireCore.WebApp.Data;
    using HangFireCore.WebApp.Helpers;
    using HangFireCore.WebApp.Models;
    using HangFireCore.WebApp.Services;

    public void ConfigureServices(IServiceCollection services)
    {
        // Add Hangfire
        services.AddHangfire(config => config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        // Initialize Dabatabase
        InitDb();

        // Configure Hangfire
        app.UseHangfireServer();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });
    }
```  

3. **Agregar la carga dinámica de módulos** </br></br>
Estos son los cambios necesarios para configurar la carga dinámica de módulos en Startup.cs, En este caso es importante cargar los módulos después que esté inicializada la base de datos y el servidor de Hangfire:
```cs
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        // Initialize Dabatabase
        InitDb();

        // Configure Hangfire
        app.UseHangfireServer();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });

        // Recurring jobs
        env.ScheduleRecurringJobs();
    }
``` 

4. **Agregar líneas para configurar NLog** </br> </br>
Estos son los cambios necesarios, en los distintos métodos de Startup.cs, para configurar NLog:
```cs
    using Microsoft.AspNetCore.Http;
    using NLog.Extensions.Logging;
    using NLog.Web;

    public Startup(IHostingEnvironment env)
    {
        env.ConfigureNLog("nlog.config");
    }

    public void ConfigureServices(IServiceCollection Services)
    {
        //call this in case you need aspnet-user-authtype/aspnet-user-identity
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        //add NLog to ASP.NET Core
        loggerFactory.AddNLog();

        //add NLog.Web
        app.AddNLogWeb();
    }
```  

5. **Verificar cambios en Startup.cs** </br>  
Con los cambios anteriores el archivo de arranque debe quedar así:
   {{<getSourceFile "src\HangFireCore.WebApp\Startup.cs">}}

6. **Agregar opción Hangfire la barra de navegación** </br>  
Modificar el archivo Views\Shared\_Layout.cshtml para agregar la opción HangFire, como se indica a continuación:  
```html
    <div class="navbar-collapse collapse">
        <ul class="nav navbar-nav">
            <li><a asp-area="" asp-controller="Home" asp-action="Index">Home</a></li>
            <li><a asp-area="" asp-controller="Home" asp-action="About">About</a></li>
            <li><a asp-area="" asp-controller="Home" asp-action="Contact">Contact</a></li>
            <li><a asp-area="" asp-controller="Hangfire" asp-action="Index">Hangfire</a></li>
        </ul>
        @await Html.PartialAsync("_LoginPartial")
    </div>
```

### 9) Probar la aplicación

Ejecutar la aplicación con [Ctrl]+[F5] y navegar hasta /hangfire, si es la primera vez le pedira que se registre en la aplicación y luego debe ver algo similar a esto:
{{<image src="/posts/images/chrome_2017-04-27_21-49-34.png">}} 

Se puede ver que en este caso no hay ningún trabajo programado

Eventualmente se puede ver más de un servidor activo. Esto ocurre porque la tareas en background corren en threads independientes y no se cierran inmediatamente al reiniciar la aplicación, pero se si lo hacen después de unos minutos.

El achivo de log debe ser similar a este, pero ubicado en c:\temp\logs:
{{<getSourceFile "src\HangFireCore.WebApp\temp\nlog-HangFireCoreApp-current.log">}}

Y con esto terminamos el artículo.

---

Espero que sea de ayuda.

**Miguel.**

---
#### Enlaces relacionados

**How to run Background Tasks in ASP.NET**
https://www.hanselman.com/blog/HowToRunBackgroundTasksInASPNET.aspx

**The Dangers of Implementing Recurring Background Tasks In ASP.NET**  
http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx

**Integrate HangFire With ASP.NET Core**
http://dotnetthoughts.net/integrate-hangfire-with-aspnet-core/

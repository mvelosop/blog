---
title: Crear una librería con Entity Framework Core
draft: false
author: Miguel Veloso
date: 2017-03-20
description: Desarrollo de una librería EF Core para entender la separación de componentes con EF Core.
thumbnail: posts/images/books-1281581_1280.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework Core", "EF Core Configuration", "EF Core CLI", "Migrations", "CSharp" ]
series: [ "Entity Framework Core" ]
repoName: EFCoreLib
repoRelease: "1.0"
---

En este artículo vamos a desarrollar una versión de la misma [aplicación de consola desarrollada anteriormente (migrada a VS 2017)](/posts/migrar-aplicacion-consola-visual-studio-2017) pero separándola en dos capas:

1. La capa de datos en el proyecto EFCore.Lib y
2. La capa cliente en EFCore.App.

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Puntos Importantes</span>

> 0. Separar la aplicación en una capa cliente y una capa de datos
> 0. Usar EF Core CLI en aplicación multicapa para crear las migraciones
> 0. Apreciar la simplicidad de los nuevos archivos .csproj

{{< repoUrl >}}

## Contexto

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(ver la [página de descargas de Visual Studio](https://www.visualstudio.com/es/downloads/) para otras versiones).

* [.NET Core 1.1.1 con SDK 1.0.1 - x64 Installer](https://go.microsoft.com/fwlink/?linkid=843448)  
(ver la [página de descargas de .NET Core](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) para otras versiones).

## Paso a paso

Como en este artículo estamos separando los componentes de la aplicación, vamos a crear los archivos de programas y luego incluiremos los paquetes necesarios para poder compilar y ejecutar la solución.

En este artículo vamos a desarrollar la solución desde el principio, para apreciar los archivos .proj en su expresión más simple. No podríamos apreciar esto si hacemos la migración de un proyecto de VS 2015.

### 1) Crear la solución EFCoreLib

1. Crear una solución "blank" llamada EFCoreLib
2. Crear el "solution folder" "src"
3. Crear la carpeta "src" dentro de la carpeta de la solución

### 2) Crear proyecto src\EFCore.App 

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Este proyecto es la capa **Cliente** que usa la "capa de datos" (el proyecto EFCore.Lib)

1. Crear el proyecto como una "Console Application (.NET Core)"

### 3) Crear proyecto src\EFCore.Lib

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> Este proyecto es la capa de **Datos**, desde luego de una forma muy rudimentaria.

1. Crear el proyecto como una "Class Library (.NET Core)"

### 4) Crear los archivos de programa en EFCore.Lib

Vamos a crear en esencia los mismos archivos que usamos en el artículo [Crear Aplicación EF Core](/posts/crear-aplicacion-entity-framework-core).

0. **Model\Currency.cs**
   {{<getSourceFile "src\EFCore.Lib\Model\Currency.cs">}}

0. **Base\EntityTypeConfiguration.cs**
   {{<getSourceFile "src\EFCore.Lib\Base\EntityTypeConfiguration.cs">}}

0. **Data\CurrencyConfiguration.cs**
   {{<getSourceFile "src\EFCore.Lib\Data\CurrencyConfiguration.cs">}}

0. **Config\ConnectionStrings.cs** <br/>  
A diferencia de lo que hicimos en el artículo [Crear Aplicación EF Core](/posts/crear-aplicacion-entity-framework-core), en el proyecto EFCore.Lib sólo incluimos la clase de configuración **ConnectionStrings.cs** porque sólo esta tiene que ver con la "capa de datos".  
   {{<getSourceFile "src\EFCore.Lib\Config\ConnectionStrings.cs">}}

0. **Data\CommonDbContext.cs**
   {{<getSourceFile "src\EFCore.Lib\Data\CommonDbContext.cs">}}

Para que el proyecto pueda compilar en este momento es necesario incluir los siguientes paquetes:

* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Relational
* Microsoft.EntityFrameworkCore.SqlServer

El archivo **EFCore.Lib.csproj** resultante es así:

{{<getSourceFile "src\EFCore.Lib\EFCore.Lib.csproj">}}

### 5) Crear los archivos de programa en EFCore.App

1. **Config\AppOptions.cs** <br/>  
En este caso incluimos la clase que maneja todas las configuraciones de la "aplicación" haciendo referencia a la clase de configuración de la capa de datos.
   {{<getSourceFile "src\EFCore.App\Config\AppOptions.cs">}}

2. **Program.cs**
   {{<getSourceFile "src\EFCore.App\Program.cs">}}

Para que el proyecto pueda compilar en este momento es necesario incluir las siguientes referencias:

1. Referencia al proyecto EFCore.Lib de la solución y

2. Los siguientes paquetes:
   * Microsoft.Extensions.Configuration
   * Microsoft.Extensions.Configuration.Binder
   * Microsoft.Extensions.Configuration.Json

### 6) Crear migración inicial

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> 

> En este caso, el proyecto EFCore.App, además de ser la "capa cliente" de la solución, va a ser el "host" para ejecutar los comandos de la [interfaz de comandos .NET EF Core (.NET Core EF CLI)](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet).

> Tal como se indica en la página de la [interfaz de comandos .NET EF Core (.NET Core EF CLI)](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#net-standard-limitation) hay una limitación de .NET Standard que no permite ejecutar ```dotnet``` en un proyecto "Class Library", así que hay que instalar los componentes necesarios en EFCore.App para ejecutarlo desde allí.

Si en este momento ejecutamos ```dotnet ef``` desde el proyecto EFCore.App 

{{<image src="/posts/images/cmd_2017-03-18_21-23-38.png">}}

Para esto hay que instalar el paquete **Microsoft.EntityFrameworkCore.Tools** en EFCore.App, pero este es un tipo de paquete **"DotNetCliTool"**, que no se puede instalar como un NuGet cualquiera.

Entonces, siguiendo lo indicado en la página de la [interfaz de comandos .NET EF Core (.NET Core EF CLI)](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet#workaround-1---use-an-app-as-the-startup-project), hay que editar el archivo .csproj del proyecto (Solution Explorer, sobre EFCore.App: Botón derecho > Edit EFCore.App.csproj) y agregar las líneas siguientes:


```xml
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0" />
  </ItemGroup>
```

Al salvar el archivo se debe instalar el paquete automáticamente. En caso contrario utilice el comando ```dotnet restore``` desde la interfaz de comandos en el proyecto **EFCore.App**.

También hay que instalar el paquete: **Microsoft.EntityFrameworkCore.Design**.

Sin embargo, como sólo vamos a utilizar **EFCore.App** para ejecutar la [.NET Core EF CLI](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet), no es necesario agregar el atributo **PrivateAssets="All"** manualmente en el archivo .csproj y, por lo tanto, podemos instalarlo como cualquier paquete NuGet.

Después de esto ya podemos ejecutar el comando para crear la migración: ```dotnet ef migrations add InitialCreateMigration --project ..\EFCore.Lib```.

Observe que usamos la opción ```--project``` o ```-p``` para indicar donde se va a crear la migración.

Después de ejecutar el comando obtenemos los archivos de la migración con el mismo contenido de la aplicación inicial:

{{<getSourceFile "src\EFCore.Lib\Migrations\20170318215905_InitialCreateMigration.cs">}}

### 7) Preparar la aplicación para ejecución

Para esto debemos:

1. Crear el archivo de configuración en el proyecto EFCore.App:
   {{<getSourceFile "src\EFCore.App\appsettings.json">}}

2. Configurar el archivo **"appasettings.json"** para que se copie a la carpeta de salida

   * **Botón Derecho > Properties** sobre el archivo **appsettings.json** 
   * Propiedad "Copy to Output Directory" => **"Copy if newer"**

3. Establecer el proyecto **EFCore.App** como la aplicación de arranque  

   * **Botón Derecho > Set as Startup Project** sobre el proyecto **EFCore.App** 

El archivo **EFCore.App.csproj** resultante es así:

> ### <span class="important"><i style="font-size: larger" class="fa fa-info-circle" aria-hidden="true"></i> Importante</span>
> En el formato .csproj no es necesario especificar todos los archivos que conforman la solución, porque se considera, por omisión, todos los archivos de todas las carpetas.

> Una ventaja importante de este formato es que elimina los frecuentes conflictos al hacer "Merge" de dos ramas, producidos por movimiento de archivos dentro del .csproj.

{{<getSourceFile "src\EFCore.App\EFCore.App.csproj">}}

### 8) Ejecutar la aplicación

Ahora basta con pulsar [Ctrl]+[F5], con lo que obtenemos el resultado esperado:
   {{<image src="/posts/images/cmd_2017-03-20_16-14-12.png">}}

---
{{< goodbye.html >}}

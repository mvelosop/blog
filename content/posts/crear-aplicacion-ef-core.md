---
title: Crear Aplicación EF Core
author: Miguel Veloso
date: 2017-03-02
description: Desarrollo de una aplicación de consola para entender aspectos de configuración de EF Core.
thumbnail: posts/images-ef-core/post.jpg
categorías: [ "Desarrollo" ]
tags: [ "Entity Framework", "CSharp" ]
series: [ "Entity Framework Core" ]
---


# Crear una aplicación con Entity Framework Core 1.1

En este artículo se muestra el desarrollo de una aplicación de consola sencilla usando Code First con EF Core 1.1, con el fin de entender los aspectos básicos del trabajo con EF Core.

Finalmente se espera poder migrar una aplicación mediana (~100 clases del modelo de dominio), haciendo lo posible por mantener la experiencia de desarrollo lo más parecida posible a la de EF 6.

El repositorio con la solución completa está aquí:

## Contexto

### Herramientas

* [Visual Studio 2015 Community Edition](https://www.visualstudio.com/post-download-vs/#)
* [SQL Server 2016 Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-editions-developers)
* [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### .NET Core 1.1
* [Visual Studio 2015 Tools (Preview 2)](https://go.microsoft.com/fwlink/?LinkId=827546)  
* [.NET Core 1.1 SDK - Installer x64](https://go.microsoft.com/fwlink/?LinkID=835014)  
* [.NET Core 1.1 runtime - Installer x64](https://go.microsoft.com/fwlink/?LinkID=835009)  
  (Este es necesario, además del SDK, para soportar .NET Core 1.1)


## Paso a paso

### 1) Crear la solución EFCoreApp

1. Crear una solución "blank" llamada EFCoreApp
2. Crear el solution folder "src"
3. Crear la carpeta "src" dentro de la carpeta de la solución

### 2) Crear proyecto src/EFCore.App 

```
{{% getRepoDir %}}
```
{{% getRepoSource %}}


1. Crear el proyecto como una "Console Application (.NET Core)"
2. Actualizar project.json a lo siguiente:

	```json
	{
		"buildOptions": {
			"copyToOutput": {
				"include": [ "appsettings.json" ]
			},
			"emitEntryPoint": true
		},
		"dependencies": {
			"Microsoft.EntityFrameworkCore": "1.1.0",
			"Microsoft.EntityFrameworkCore.Design": {
				"type": "build",
				"version": "1.1.0"
			},
			"Microsoft.EntityFrameworkCore.SqlServer": "1.1.0",
			"Microsoft.Extensions.Configuration": "1.1.0",
			"Microsoft.Extensions.Configuration.Binder": "1.1.0",
			"Microsoft.Extensions.Configuration.Json": "1.1.0",
			"Microsoft.NETCore.App": {
				"type": "platform",
				"version": "1.1.0"
			},
			"System.ComponentModel.Annotations": "4.3.0"
		},
		"frameworks": {
			"netcoreapp1.0": {
				"imports": "dnxcore50"
			}
		},
		"tools": {
			"Microsoft.EntityFrameworkCore.Tools.DotNet": "1.1.0-preview4-final"
		},
		"version": "1.0.0-*"
	}
	```
3. Salvar el archivo desde VS para actualizar todos los paquetes o, si prefiere usar la interfaz de comandos de desarrollo ([Shift]+[Alt]+[,]), ejecute **```dotnet restore```**

#### Detalles de project.json

* Paquetes de Entity Framework Core

   * **Microsoft.EntityFrameworkCore**: Paquete base
   * **Microsoft.EntityFrameworkCore.Design**: Componentes para EF Core CLI, sólo para desarrollo, por eso ```"type": "build"```
   * **Microsoft.EntityFrameworkCore.Tools.DotNet**: EF Core CLI
   * **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider

* Paquetes para manejar archivos de configuración

   * **Microsoft.Extensions.Configuration**: Paquete base
   * **Microsoft.Extensions.Configuration.Binder**: Para manejar configuraciones "strongly typed".
   * **Microsoft.Extensions.Configuration.Json**: Manejo de archivos json

* Otros

   * **System.ComponentModel.Annotations**: Annotations para los modelos
  
### 3) Agregar archivos del proyecto

#### Model/Currency.cs

La clase del modelo, Divisas en este caso.

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace EFCore.App.Model
{
    public class Currency
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(3)]
        public string IsoCode { get; set; }

        [Required]
        [MaxLength(100)] // Default string length
        public string Name { get; set; }

        public byte[] RowVersion { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; }
    }
}
```
#### Base/EntityTypeConfiguration.cs

Estas clases permiten manejar una clase de configuración por cada clase del modelo, para mantener el DbContext lo más sencillo posible, de forma similar a como se puede hacer con EF 6, según lo sugerido en https://github.com/aspnet/EntityFramework/issues/2805

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.App.Base
{
    // As suggested in: https://github.com/aspnet/EntityFramework/issues/2805

    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, EntityTypeConfiguration<TEntity> configuration) 
            where TEntity : class
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }
    }

    public abstract class EntityTypeConfiguration<TEntity> 
        where TEntity : class
    {
        public abstract void Map(EntityTypeBuilder<TEntity> builder);
    }
}
```
#### Data/CurrencyConfiguration.cs

Clase de configuración del modelo en EF. Así se mantienen fuera del modelo los detalles que corresponden a la capa de base de datos.

```csharp
using EFCore.App.Base;
using EFCore.App.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.App.Data
{
    public class CurrencyConfiguration : EntityTypeConfiguration<Currency>
    {
        public override void Map(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies", schema: "Common");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RowVersion)
                .IsRowVersion();

            builder.HasIndex(e => e.IsoCode)
                .IsUnique();
        }
    }
}
```
#### Config/ConfigClasses.cs

Clases de configuración de la aplicación, permiten manejar la configuraciones que se carguen del archivo 
**appsettings.json** de una forma "strongly typed".

```csharp
namespace EFCore.App.Config
{
    public class AppOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
```

#### Data/CommonDbContext.cs

El DbContext para la aplicación, define la vista de la base de datos a la que tiene acceso la aplicación.

```csharp
using EFCore.App.Base;
using EFCore.App.Config;
using EFCore.App.Model;
using Microsoft.EntityFrameworkCore;

namespace EFCore.App.Data
{
    public class CommonDbContext : DbContext
    {
        // Must not be null or empty for initial create migration
        private string _connectionString = "ConnectionString";

        // Default constructor for initial create migration
        public CommonDbContext()
        {
        }

        // Normal use constructor
        public CommonDbContext(ConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.DefaultConnection;
        }

        public DbSet<Currency> Currencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddConfiguration(new CurrencyConfiguration());
        }
    }
}
```

#### Program.cs

El programa principal de la aplicación. Aquí están los métodos que crean/actualizan la base de datos y realizar la carga de datos iniciales.

```csharp
using EFCore.App.Config;
using EFCore.App.Data;
using EFCore.App.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EFCore.App
{
    public class Program
    {
        private static Currency[] _currencyData = new[]
        {
            new Currency { IsoCode = "USD", Name = "US Dolar", Symbol = "US$" },
            new Currency { IsoCode = "EUR", Name = "Euro", Symbol = "€" },
            new Currency { IsoCode = "CHF", Name = "Swiss Franc", Symbol = "Fr." },
        };

        public static AppOptions AppOptions { get; set; }

        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("EF Core App\n");

            ReadConfiguration();

            InitDb();

            PrintDb();
        }

        private static void InitDb()
        {
            using (var db = new CommonDbContext(AppOptions.ConnectionStrings))
            {
                Console.WriteLine("Creating database...\n");

                db.Database.EnsureCreated();

                Console.WriteLine("Seeding database...\n");

                LoadInitalData(db);
            }
        }

        private static void LoadInitalData(CommonDbContext db)
        {
            foreach (var item in _currencyData)
            {
                Currency currency = db.Currencies.FirstOrDefault(c => c.Symbol == item.Symbol);

                if (currency == null)
                {
                    db.Currencies.Add(item);
                }
                else
                {
                    currency.Name = item.Name;
                    currency.Symbol = item.Symbol;
                }
            }

            db.SaveChanges();
        }

        private static void PrintDb()
        {
            using (var db = new CommonDbContext(AppOptions.ConnectionStrings))
            {
                Console.WriteLine("Reading database...\n");

                Console.WriteLine("Currencies");
                Console.WriteLine("----------");

                int symbolLength = _currencyData.Select(c => c.Symbol.Length).Max();
                int nameLength = _currencyData.Select(c => c.Name.Length).Max();

                foreach (var item in db.Currencies)
                {
                    Console.WriteLine($"| {item.IsoCode} | {item.Symbol.PadRight(symbolLength)} | {item.Name.PadRight(nameLength)} |");
                }

                Console.WriteLine();
            }
        }

        private static void ReadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            // Reads appsettings.json into a (strongly typed) class
            AppOptions = Configuration.Get<AppOptions>();

            Console.WriteLine("Configuration\n");
            Console.WriteLine($@"connectionString (defaultConnection) = ""{AppOptions.ConnectionStrings.DefaultConnection}""");
            Console.WriteLine();
        }
    }
}
```

### 4) Generar la migración inicial

Ahora es necesario generar la migración inicial que utilizará EF para crear la base de datos cuando se ejecute la aplicación por primera vez.

1. Abrir la interfaz de comandos de desarrollo

   * Hacer click sobre el nodo del proyecto EFCore.App en el explorador de la solución.
   * Pulsar [Shift]+[Alt]+[,] o Botón derecho > Open Command Line > Developer Command Prompt
   * Ejecutar **```dotnet ef```**
   * Si todo marchó bien, debe observar la una pantalla similar a la siguiente:
   
	{{<img-popup src="/posts/images-ef-core/cmd_2017-02-27_23-41-30.png" width="100%">}}

2. Crear la migración inicial

   * Ejecutar **```dotnet ef migrations add InitialCreateMigration```** en la interfaz de comandos.
   * Se puede utilizar cualquier nombre para la clase de la migración, pero es recomendable utilizar el sufijo "Migration" para evitar conflictos de nombres con otras clases de la aplicación.

3. Verificar que se hayan creado los archivos del la migración inicial, en la carpeta Migrations, similar a los siguientes:
 
#### Migrations/CommonDbContextModelSnapshot.cs

Este archivo es la configuración de la última versión del modelo, se utiliza al ejecutar el método DbContext.Database.EnsureCreated().

```csharp
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EFCore.App.Data;

namespace EFCore.App.Migrations
{
    [DbContext(typeof(CommonDbContext))]
    partial class CommonDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EFCore.App.Model.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .HasMaxLength(3);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("IsoCode")
                        .IsUnique();

                    b.ToTable("Currencies","Common");
                });
        }
    }
}
```
#### Migrations/20170227231210_InitialCreateMigration

Este archivo es el encargado de generar la migración desde la versión anterior de CommonDbContextModelSnapshot.cs, se utiliza al ejecutar el método DbContext.Database.Migrate().

Los números iniciales del nombre indican el año-mes-día-hora-minuto-segundo (yyyyMMddHHmmss) de generación de la migración.


```csharp
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFCore.App.Migrations
{
    public partial class InitialCreateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Common");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsoCode = table.Column<string>(maxLength: 3, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Symbol = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsoCode",
                schema: "Common",
                table: "Currencies",
                column: "IsoCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "Common");
        }
    }
}
```

### 5) Crear archivo de configuración

#### appsettings.json
```json
{
    "connectionStrings": {
        "defaultConnection": "Data Source=localhost;Initial Catalog=EFCore.App;Integrated Security=SSPI;"
    }
}
```

Verificar que project.json incluya la opción para copiar este archivo a la carpeta de salida:

```json
    "buildOptions": {
        "copyToOutput": {
            "include": [ "appsettings.json" ]
        },
        "emitEntryPoint": true
    },
```

### 6) Ejecutar la aplicación

Al ejecutar la aplicación con [Ctrl]+[F5] se debe obtener una salida similar a esta:

{{<img-popup src="/posts/images-ef-core/cmd_2017-02-28_00-47-31.png">}}

## Conclusiones


## Enlaces relacionados

**.NET Core current downloads**  
https://www.microsoft.com/net/download/core#/current

**.NET Core EF CLI**  
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet


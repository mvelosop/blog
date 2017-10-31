using Autofac;
using DFlow.Budget.Lib.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DFlow.Budget.Setup
{
    public class BudgetContainerSetup
    {
        private const string _modulePrefix = "DFlow.Budget";

        private readonly BudgetDbSetupHelper DbSetupHelper;

        public BudgetContainerSetup(BudgetDbSetupHelper dbSetupHelper)
        {
            DbSetupHelper = dbSetupHelper ?? throw new ArgumentNullException(nameof(dbSetupHelper));
        }

        /// <summary>
        ///     Adds module's classes registration to the received container builder
        /// </summary>
        /// <param name="builder"></param>
        public void RegisterTypes(ContainerBuilder builder)
        {
            // This defers instance registration until it is actually needed
            builder.Register<BudgetDbContext>((c) => DbSetupHelper.CreateDbContext())
                .InstancePerLifetimeScope();

            RegisterCommonModuleTypes(builder, _modulePrefix);
        }

        private static Assembly[] LoadModuleAssemblies(string modulePrefix)
        {
            // From: https://stackoverflow.com/questions/2384592/is-there-a-way-to-force-all-referenced-assemblies-to-be-loaded-into-the-app-doma
            // With some fixes!

            AppDomain currentDomain = AppDomain.CurrentDomain;

            var loadedAssemblies = currentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(modulePrefix)).ToList();

            var loadedFiles = loadedAssemblies.Select(a => a.CodeBase.Replace("file:///", "").Replace('/', Path.DirectorySeparatorChar)).ToArray();

            var existingFiles = Directory.GetFiles(currentDomain.BaseDirectory, $"{modulePrefix}*.dll");

            var filesToLoad = existingFiles.Where(r => !loadedFiles.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            filesToLoad.ForEach(path => loadedAssemblies.Add(currentDomain.Load(AssemblyName.GetAssemblyName(path))));

            return loadedAssemblies.ToArray();
        }

        private static void RegisterCommonModuleTypes(ContainerBuilder builder, string modulePrefix)
        {
            Assembly[] appAssemblies = LoadModuleAssemblies(modulePrefix);

            foreach (var asm in appAssemblies)
            {
                builder.RegisterAssemblyTypes(asm)
                    .Where(t => t.Name.EndsWith("DataHelper"))
                    .InstancePerLifetimeScope();

                builder.RegisterAssemblyTypes(asm)
                    .Where(t => t.Name.EndsWith("DataMapper"))
                    .InstancePerLifetimeScope();

                builder.RegisterAssemblyTypes(asm)
                    .Where(t => t.Name.EndsWith("Manager"))
                    .InstancePerLifetimeScope()
                    .AsSelf()
                    .AsImplementedInterfaces();

                builder.RegisterAssemblyTypes(asm)
                    .Where(t => t.Name.EndsWith("ManagerHelper"))
                    .InstancePerLifetimeScope();
            }
        }
    }
}

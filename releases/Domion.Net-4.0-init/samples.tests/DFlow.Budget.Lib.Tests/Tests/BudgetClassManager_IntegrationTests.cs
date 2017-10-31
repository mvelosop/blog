using Autofac;
using DFlow.Budget.Core.Model;
using DFlow.Budget.Lib.Services;
using DFlow.Budget.Lib.Tests.Helpers;
using DFlow.Budget.Setup;
using Domion.FluentAssertions.Extensions;
using Domion.Lib.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace DFlow.Budget.Lib.Tests.Tests
{
    [Trait("Type", "Integration")]
    public class BudgetClassManager_IntegrationTests
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=DFlow.Budget.Lib.Tests;Integrated Security=SSPI;MultipleActiveResultSets=true";

        private static readonly BudgetDbSetupHelper DbSetupHelper;

        private readonly IContainer Container;

        static BudgetClassManager_IntegrationTests()
        {
            DbSetupHelper = SetupDatabase(ConnectionString);
        }

        public BudgetClassManager_IntegrationTests()
        {
            Container = SetupContainer(DbSetupHelper);
        }

        [Fact]
        public void TryDelete_DeletesRecord_WhenValidData()
        {
            // Arrange ---------------------------

            var data = new BudgetClassData("Delete-Success-Valid - Inserted", TransactionType.Income);

            UsingManagerHelper((scope, helper) =>
            {
                helper.EnsureEntitiesExist(data);
            });

            // Act -------------------------------

            IEnumerable<ValidationResult> errors = null;

            UsingManager((scope, manager) =>
            {
                BudgetClass entity = manager.SingleOrDefault(bc => bc.Name == data.Name);

                errors = manager.TryDelete(entity).ToList();

                manager.SaveChanges();
            });

            // Assert ----------------------------

            errors.Should().BeEmpty();

            UsingManagerHelper((scope, helper) =>
            {
                helper.AssertEntitiesDoNotExist(data);
            });
        }

        [Fact]
        public void TryInsert_Fails_WhenDuplicateKeyData()
        {
            // Arrange ---------------------------

            var data = new BudgetClassData("Insert-Error-Duplicate - Inserted", TransactionType.Income);

            UsingManagerHelper((scope, helper) =>
            {
                helper.EnsureEntitiesExist(data);
            });

            // Act -------------------------------

            IEnumerable<ValidationResult> errors = null;

            UsingManager((scope, manager) =>
            {
                var mapper = scope.Resolve<BudgetClassDataMapper>();

                BudgetClass entity = mapper.CreateEntity(data);

                errors = manager.TryInsert(entity).ToList();
            });

            // Assert ----------------------------

            errors.Should().ContainErrorMessage(BudgetClassManager.duplicateByNameError);
        }

        [Fact]
        public void TryInsert_InsertsRecord_WhenValidData()
        {
            IEnumerable<ValidationResult> errors = null;

            // Arrange ---------------------------

            var data = new BudgetClassData("Insert-Success-Valid - Inserted", TransactionType.Income);

            UsingManagerHelper((scope, helper) =>
            {
                helper.EnsureEntitiesDoNotExist(data);
            });

            // Act -------------------------------

            UsingManager((scope, manager) =>
            {
                var mapper = scope.Resolve<BudgetClassDataMapper>();

                BudgetClass entity = mapper.CreateEntity(data);

                errors = manager.TryInsert(entity).ToList();

                manager.SaveChanges();
            });

            // Assert ----------------------------

            errors.Should().BeEmpty();

            UsingManagerHelper((scope, helper) =>
            {
                helper.AssertEntitiesExist(data);
            });
        }

        [Fact]
        public void TryUpdate_Fails_WhenDuplicateKeyData()
        {
            // Arrange ---------------------------

            var data = new BudgetClassData("Update-Error-Duplicate - Inserted first", TransactionType.Income);
            var update = new BudgetClassData("Update-Error-Duplicate - Inserted second", TransactionType.Income);

            UsingManagerHelper((scope, helper) =>
            {
                helper.EnsureEntitiesExist(data, update);
            });

            // Act -------------------------------

            IEnumerable<ValidationResult> errors = null;

            UsingManager((scope, manager) =>
            {
                var mapper = scope.Resolve<BudgetClassDataMapper>();

                BudgetClass entity = manager.SingleOrDefault(bc => bc.Name == data.Name);

                entity = mapper.UpdateEntity(entity, update);

                errors = manager.TryUpdate(entity).ToList();
            });

            // Assert ----------------------------

            errors.Should().ContainErrorMessage(BudgetClassManager.duplicateByNameError);
        }

        [Fact]
        public void TryUpdate_UpdatesRecord_WhenValidData()
        {
            // Arrange ---------------------------

            var data = new BudgetClassData("Update-Success-Valid - Inserted", TransactionType.Income);
            var update = new BudgetClassData("Update-Success-Valid - Updated", TransactionType.Income);

            UsingManagerHelper((scope, helper) =>
            {
                helper.EnsureEntitiesExist(data);
                helper.EnsureEntitiesDoNotExist(update);
            });

            // Act -------------------------------

            IEnumerable<ValidationResult> errors = null;

            UsingManager((scope, manager) =>
            {
                var mapper = scope.Resolve<BudgetClassDataMapper>();

                BudgetClass entity = manager.SingleOrDefault(bc => bc.Name == data.Name);

                entity = mapper.UpdateEntity(entity, update);

                errors = manager.TryUpdate(entity).ToList();

                manager.SaveChanges();
            });

            // Assert ----------------------------

            errors.Should().BeEmpty();

            UsingManagerHelper((scope, helper) =>
            {
                helper.AssertEntitiesExist(update);
            });
        }

        private static IContainer SetupContainer(BudgetDbSetupHelper dbHelper)
        {
            var setupHelper = new BudgetAutofacSetupHelper(dbHelper);

            var builder = new ContainerBuilder();

            setupHelper.SetupContainer(builder);

            IContainer container = builder.Build();

            return container;
        }

        private static BudgetDbSetupHelper SetupDatabase(string connectionString)
        {
            var dbHelper = new BudgetDbSetupHelper(connectionString);

            dbHelper.SetupDatabase();

            return dbHelper;
        }

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
    }
}

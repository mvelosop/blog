using Autofac;
using DFlow.Budget.Core.Model;
using DFlow.Budget.Lib.Services;
using Domion.Lib.Extensions;
using FluentAssertions;
using System;

namespace DFlow.Budget.Lib.Tests.Helpers
{
    public class BudgetClassManagerHelper
    {
        private readonly Lazy<BudgetClassDataMapper> LazyBudgetClassDataMapper;
        private readonly Lazy<BudgetClassManager> LazyBudgetClassManager;
        private readonly ILifetimeScope Scope;

        /// <summary>
        ///     Creates a Helper for BudgetClassManager to help in the test's Arrange and Assert sections
        /// </summary>
        public BudgetClassManagerHelper(
            ILifetimeScope scope,
            Lazy<BudgetClassDataMapper> lazyBudgetClassDataMapper,
            Lazy<BudgetClassManager> lazyBudgetClassManager)
        {
            Scope = scope;

            LazyBudgetClassManager = lazyBudgetClassManager;
            LazyBudgetClassDataMapper = lazyBudgetClassDataMapper;
        }

        private BudgetClassManager BudgetClassManager => LazyBudgetClassManager.Value;

        private BudgetClassDataMapper BudgetClassMapper => LazyBudgetClassDataMapper.Value;

        /// <summary>
        ///     Asserts that entities with the supplied key data values do not exist
        /// </summary>
        /// <param name="dataSet">Data for the entities to be searched for</param>
        public void AssertEntitiesDoNotExist(params BudgetClassData[] dataSet)
        {
            using (ILifetimeScope scope = Scope.BeginLifetimeScope())
            {
                var manager = scope.Resolve<BudgetClassManager>();

                foreach (BudgetClassData data in dataSet)
                {
                    BudgetClass entity = manager.SingleOrDefault(e => e.Name == data.Name);

                    entity.Should().BeNull(@"because BudgetClass ""{0}"" MUST NOT EXIST!", data.Name);
                }
            }
        }

        /// <summary>
        ///     Asserts that entities equivalent to the supplied input data classes exist
        /// </summary>
        /// <param name="dataSet">Data for the entities to be searched for</param>
        public void AssertEntitiesExist(params BudgetClassData[] dataSet)
        {
            using (ILifetimeScope scope = Scope.BeginLifetimeScope())
            {
                var manager = scope.Resolve<BudgetClassManager>();
                var mapper = scope.Resolve<BudgetClassDataMapper>();

                foreach (BudgetClassData data in dataSet)
                {
                    BudgetClass entity = manager.SingleOrDefault(e => e.Name == data.Name);

                    entity.Should().NotBeNull(@"because BudgetClass ""{0}"" MUST EXIST!", data.Name);

                    BudgetClassData entityData = mapper.CreateData(entity);

                    entityData.ShouldBeEquivalentTo(data);
                }
            }
        }

        /// <summary>
        ///     Ensures that the entities do not exist in the database or are successfully removed
        /// </summary>
        /// <param name="dataSet">Data for the entities to be searched for and removed if necessary</param>
        public void EnsureEntitiesDoNotExist(params BudgetClassData[] dataSet)
        {
            foreach (BudgetClassData data in dataSet)
            {
                BudgetClass entity = BudgetClassManager.SingleOrDefault(e => e.Name == data.Name);

                if (entity == null) continue;

                var errors = BudgetClassManager.TryDelete(entity);

                errors.Should().BeEmpty(@"because BudgetClass ""{0}"" has to be removed!", data.Name);
            }

            BudgetClassManager.SaveChanges();

            AssertEntitiesDoNotExist(dataSet);
        }

        /// <summary>
        ///     Ensures that the entities exist in the database or are successfully added
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataSet">Data for the entities to be searched for and added or updated if necessary</param>
        public void EnsureEntitiesExist(params BudgetClassData[] dataSet)
        {
            foreach (BudgetClassData data in dataSet)
            {
                BudgetClass entity = BudgetClassManager.SingleOrDefault(e => e.Name == data.Name);

                entity = entity == null ? BudgetClassMapper.CreateEntity(data) : BudgetClassMapper.UpdateEntity(entity, data);

                var errors = BudgetClassManager.TryUpsert(entity);

                errors.Should().BeEmpty(@"because BudgetClass ""{0}"" has to be added!", data.Name);
            }

            BudgetClassManager.SaveChanges();

            AssertEntitiesExist(dataSet);
        }
    }
}

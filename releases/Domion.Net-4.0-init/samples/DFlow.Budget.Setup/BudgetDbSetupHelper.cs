using DFlow.Budget.Lib.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace DFlow.Budget.Setup
{
    public class BudgetDbSetupHelper
    {
        private DbContextOptions<BudgetDbContext> _options;

        private readonly string _connectionString;

        public BudgetDbSetupHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns the DbContext if the database has been set up.
        /// </summary>
        /// <returns></returns>
        public BudgetDbContext CreateDbContext()
        {
            if (_options == null) throw new InvalidOperationException($"Must run {nameof(BudgetDbSetupHelper)}.{nameof(SetupDatabase)} first!");

            return new BudgetDbContext(_options);
        }

        /// <summary>
        /// Creates the database and applies pending migrations.
        /// </summary>
        public void SetupDatabase()
        {
            var optionBuilder = new DbContextOptionsBuilder<BudgetDbContext>();

            optionBuilder.UseSqlServer(_connectionString);

            _options = optionBuilder.Options;

            using (var dbContext = CreateDbContext())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}

using EFCore.Lib.Data;
using EFCore.Lib.Model;
using System;
using System.Linq;

namespace EFCore.Specs.Helpers
{
    public class CurrencyTestData
    {
        public string Código { get; set; }

        public string Nombre { get; set; }

        public string Símbolo { get; set; }

        public Currency CreateCurrency()
        {
            var currency = new Currency
            {
                IsoCode = Código,
                Name = Nombre,
                Symbol = Símbolo,
            };

            return currency;
        }

        public Currency UpdateCurrency(Currency currency)
        {
            currency.Symbol = Símbolo;
            currency.Name = Nombre;

            return currency;
        }
    }

    public class CurrencyTestHelpers
    {
        private Lazy<CommonDbContext> _lazyCommonDbContext;

        public CurrencyTestHelpers(
            Lazy<CommonDbContext> lazyCommonDbContext)
        {
            _lazyCommonDbContext = lazyCommonDbContext;
        }

        public CommonDbContext DbContext => _lazyCommonDbContext.Value;

        public Currency UpsertCurrency(CurrencyTestData testData)
        {
            Currency currency = FindByCode(testData.Código);

            if (currency == null)
            {
                currency = testData.CreateCurrency();

                DbContext.Currencies.Add(currency);
            }
            else
            {
                currency = testData.UpdateCurrency(currency);

                DbContext.Currencies.Update(currency);
            }

            DbContext.SaveChanges();

            return currency;
        }

        private Currency FindByCode(string code)
        {
            return DbContext.Currencies.SingleOrDefault(c => c.IsoCode == code);
        }
    }
}

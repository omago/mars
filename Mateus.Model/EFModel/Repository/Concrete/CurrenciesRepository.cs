using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class CurrenciesRepository : Repository<Currency>, ICurrenciesRepository
    {
        public CurrenciesRepository(ObjectContext context): base(context)
        {

        }

        public Currency GetCurrencyByPK(int currencyPK)
        {
            Currency a = GetAll().Where(currency => currency.CurrencyPK == currencyPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Currency> GetValid()
        {
           return GetAll().Where(currency => currency.Deleted == false || currency.Deleted == null);
        }

    }
}

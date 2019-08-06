using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class CountriesRepository : Repository<Country>, ICountriesRepository
    {
        public CountriesRepository(ObjectContext context): base(context)
        {

        }

        public Country GetCountryByPK(int countryPK)
        {
            Country a = GetAll().Where(country => country.CountryPK == countryPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Country> GetValid()
        {
           return GetAll().Where(country => country.Deleted == false || country.Deleted == null);
        }

        public IQueryable<Country> GetCitizenships()
        {
            return GetValid().Where(c => c.Citizenship != null);
        }
    }
}

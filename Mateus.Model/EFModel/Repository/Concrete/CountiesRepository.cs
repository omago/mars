using System;
using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class CountiesRepository : Repository<County>, ICountiesRepository
    {
        ICountriesRepository countriesRepository;
        IPostalOfficesRepository postalOfficesRepository;

        public CountiesRepository(ObjectContext context)
            : base(context)
        {
            countriesRepository = new CountriesRepository(context);
            postalOfficesRepository = new PostalOfficesRepository(context);
        }

        public County GetCountyByPK(int countyPK)
        {
            County a = GetAll().Where(county => county.CountyPK == countyPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<County> GetValid()
        {
            return GetAll().Where(county => county.Deleted == false || county.Deleted == null);
        }


        public IQueryable<County> GetCountiesByCountry(int countryFK)
        {
            var allCounties = GetValid();

            var counties = (from county in allCounties.Where(c => c.CountryFK == countryFK)
                            select county);

            return counties;
        }

        public IQueryable<County> GetCountiesByUserPK(int userPK)
        {
            throw new NotImplementedException();
        }


        public IQueryable<County> GetCountyForPostalOffice(int postalOfficePK)
        {
            throw new NotImplementedException();
        }
    }
}

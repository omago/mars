using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ICountriesRepository : IRepository<Country>
    {
        Country GetCountryByPK(int countryPK);

        IQueryable<Country> GetValid();
        IQueryable<Country> GetCitizenships();
    }
}

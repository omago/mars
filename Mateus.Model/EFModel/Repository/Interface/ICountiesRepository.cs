using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ICountiesRepository : IRepository<County>
    {
        County GetCountyByPK(int countyPK);

        IQueryable<County> GetValid();
        IQueryable<County> GetCountiesByCountry(int countryFK);
        IQueryable<County> GetCountiesByUserPK(int userPK);
        IQueryable<County> GetCountyForPostalOffice(int postalOfficePK);
    }
}

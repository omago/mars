using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IPostalOfficesRepository : IRepository<PostalOffice>
    {
        PostalOffice GetPostalOfficeByPK(int postalOfficePK);

        IQueryable<PostalOffice> GetValid();
        IQueryable<PostalOffice> GetValidByCounty(int countyFK);
    }
}

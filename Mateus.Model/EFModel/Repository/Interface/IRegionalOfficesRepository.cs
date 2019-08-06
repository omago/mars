using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IRegionalOfficesRepository : IRepository<RegionalOffice>
    {
        RegionalOffice GetRegionalOfficeByPK(int regionalOfficePK);

        IQueryable<RegionalOffice> GetValid();
    }
}

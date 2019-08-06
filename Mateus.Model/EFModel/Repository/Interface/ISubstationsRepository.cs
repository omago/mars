using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ISubstationsRepository : IRepository<Substation>
    {
        Substation GetSubstationByPK(int substationPK);

        IQueryable<Substation> GetValid();
        IQueryable<Substation> GetValidByRegionalOffice(int regionalOfficeFK);
    }
}

using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IPhysicalEntitiesRepository : IRepository<PhysicalEntity>
    {
        PhysicalEntity GetPhysicalEntityByPK(int physicalEntityPK);

        IQueryable<PhysicalEntity> GetValid();
        IQueryable<PhysicalEntity> GetValidReferentsBySubstation(int substationFK);
    }
}

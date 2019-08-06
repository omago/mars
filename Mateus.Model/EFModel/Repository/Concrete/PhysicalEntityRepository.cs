using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class PhysicalEntitiesRepository : Repository<PhysicalEntity>, IPhysicalEntitiesRepository
    {
        public PhysicalEntitiesRepository(ObjectContext context): base(context)
        {

        }


        public PhysicalEntity GetPhysicalEntityByPK(int physicalEntityPK)
        {
            PhysicalEntity a = GetAll().Where(physicalEntity => physicalEntity.PhysicalEntityPK == physicalEntityPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<PhysicalEntity> GetValid()
        {
           return GetAll().Where(physicalEntity => physicalEntity.Deleted == false || physicalEntity.Deleted == null);
        }


        public IQueryable<PhysicalEntity> GetValidReferentsBySubstation(int substationFK)
        {
           return GetAll().Where(physicalEntity => (physicalEntity.Deleted == false || physicalEntity.Deleted == null) && physicalEntity.Referent == true && physicalEntity.ReferentSubstationFK == substationFK);
        }

    }
}
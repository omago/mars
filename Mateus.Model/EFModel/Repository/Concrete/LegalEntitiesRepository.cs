using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class LegalEntitiesRepository : Repository<LegalEntity>, ILegalEntitiesRepository
    {
        IBanksRepository banksRepository;
        ILegalEntityBanksRepository legalEntitiesBanksRepository;

        public LegalEntitiesRepository(ObjectContext context)
            : base(context)
        {
            banksRepository = new BanksRepository(context);
            legalEntitiesBanksRepository = new LegalEntityBanksRepository(context);
        }

        public LegalEntity GetLegalEntityByPK(int legalEntityPK)
        {
            LegalEntity a = GetAll().Where(legalEntity => legalEntity.LegalEntityPK == legalEntityPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public LegalEntity GetLegalEntityByName(string name)
        {
            LegalEntity a = GetValid().Where(legalEntity => legalEntity.Name.Contains(name)).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<LegalEntity> GetValid()
        {
            return GetAll().Where(legalEntity => legalEntity.Deleted == false || legalEntity.Deleted == null).Where(legalEntity => legalEntity.Active == true);
        }

        public IQueryable<LegalEntity> GetValidOwners()
        {
            return GetValid().Where(legalEntity => legalEntity.Owner == true);
        }

        public IQueryable<LegalEntity> GetValidLegalEntities()
        {
            return GetValid().Where(legalEntity => legalEntity.Company == true);
        }

    }
}

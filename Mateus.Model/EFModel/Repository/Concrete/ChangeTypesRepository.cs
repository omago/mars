using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ChangeTypesRepository : Repository<ChangeType>, IChangeTypesRepository
    {
        public ChangeTypesRepository(ObjectContext context): base(context)
        {

        }

        public ChangeType GetChangeTypeByPK(int changeTypePK)
        {
            ChangeType a = GetAll().Where(changeType => changeType.ChangeTypePK == changeTypePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<ChangeType> GetValid()
        {
           return GetAll().Where(changeType => changeType.Deleted == false || changeType.Deleted == null);
        }
    }
}

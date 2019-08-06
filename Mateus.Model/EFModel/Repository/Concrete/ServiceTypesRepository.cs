using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ServiceTypesRepository : Repository<ServiceType>, IServiceTypesRepository
    {
        public ServiceTypesRepository(ObjectContext context): base(context)
        {

        }

        public ServiceType GetServiceTypeByPK(int serviceTypePK)
        {
            ServiceType a = GetAll().Where(serviceType => serviceType.ServiceTypePK == serviceTypePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<ServiceType> GetValid()
        {
           return GetAll().Where(serviceType => serviceType.Deleted == false || serviceType.Deleted == null);
        }
    }
}

using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IServiceTypesRepository : IRepository<ServiceType>
    {
        ServiceType GetServiceTypeByPK(int serviceTypePK);

        IQueryable<ServiceType> GetValid();
    }
}

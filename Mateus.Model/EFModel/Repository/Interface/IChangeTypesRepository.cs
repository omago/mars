using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IChangeTypesRepository : IRepository<ChangeType>
    {
        ChangeType GetChangeTypeByPK(int changeTypePK);

        IQueryable<ChangeType> GetValid();
    }
}
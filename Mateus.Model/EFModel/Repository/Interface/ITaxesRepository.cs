using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ITaxesRepository : IRepository<Tax>
    {
        Tax GetTaxByPK(int taxPK);

        IQueryable<Tax> GetValid();
    }
}
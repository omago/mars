using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IBanksRepository : IRepository<Bank>
    {
        Bank GetBankByPK(int bankPK);

        IQueryable<Bank> GetValid();
    }
}

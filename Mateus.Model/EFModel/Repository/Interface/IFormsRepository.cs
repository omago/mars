using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IFormsRepository : IRepository<Form>
    {
        Form GetFormByPK(int formPK);

        IQueryable<Form> GetValid();
    }
}

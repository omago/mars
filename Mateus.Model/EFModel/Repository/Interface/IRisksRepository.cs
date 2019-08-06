using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IRisksRepository : IRepository<Risk>
    {
        Risk GetRiskByPK(int riskPK);

        IQueryable<Risk> GetValid();

        Risk GetRiskByName(string name);
    }
}

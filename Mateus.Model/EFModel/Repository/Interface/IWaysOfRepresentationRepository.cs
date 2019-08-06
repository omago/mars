using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IWaysOfRepresentationRepository : IRepository<WayOfRepresentation>
    {
        WayOfRepresentation GetWayOfRepresentationByPK(int waysOfRepresentationPK);

        IQueryable<WayOfRepresentation> GetValid();
    }
}

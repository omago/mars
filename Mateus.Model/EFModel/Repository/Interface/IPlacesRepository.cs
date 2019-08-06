using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IPlacesRepository : IRepository<Place>
    {
        Place GetPlaceByPK(int placePK);

        IQueryable<Place> GetPlacesByPostalOffice(int postalOfficeFK);
        IQueryable<Place> GetValid();
    }
}
using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class PlacesRepository : Repository<Place>, IPlacesRepository
    {
        public PlacesRepository(ObjectContext context)
            : base(context)
        {

        }

        public Place GetPlaceByPK(int placePK)
        {
            Place p = GetAll().Where(place => place.PlacePK == placePK).First();

            if (p != null)
            {
                return p;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Place> GetPlacesByPostalOffice(int postalOfficeFK)
        {
            var allPlaces = GetValid();

            var places = (from place in allPlaces.Where(c => c.PostalOfficeFK == postalOfficeFK)
                            select place);

            return places;
        }

        public IQueryable<Place> GetValid()
        {
            return GetAll().Where(place => place.Deleted == false || place.Deleted == null);
        }
    }
}

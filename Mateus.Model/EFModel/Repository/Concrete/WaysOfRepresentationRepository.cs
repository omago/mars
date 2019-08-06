using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class WaysOfRepresentationRepository : Repository<WayOfRepresentation>, IWaysOfRepresentationRepository
    {
        public WaysOfRepresentationRepository(ObjectContext context): base(context)
        {

        }

        public WayOfRepresentation GetWayOfRepresentationByPK(int wayOfRepresentationPK)
        {
            WayOfRepresentation a = GetAll().Where(wayOfRepresentation => wayOfRepresentation.WayOfRepresentationPK == wayOfRepresentationPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<WayOfRepresentation> GetValid()
        {
           return GetAll().Where(wayOfRepresentation => wayOfRepresentation.Deleted == false || wayOfRepresentation.Deleted == null);
        }

    }
}

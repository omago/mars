using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class PostalOfficesRepository : Repository<PostalOffice>, IPostalOfficesRepository
    {
        public PostalOfficesRepository(ObjectContext context): base(context)
        {

        }

        public PostalOffice GetPostalOfficeByPK(int postalOfficePK)
        {
            PostalOffice a = GetAll().Where(postalOffice => postalOffice.PostalOfficePK == postalOfficePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<PostalOffice> GetValid()
        {
           return GetAll().Where(postalOffice => postalOffice.Deleted == false || postalOffice.Deleted == null);
        }

        public IQueryable<PostalOffice> GetValidByCounty(int countyFK)
        {
            return GetValid().Where(p => p.CountyFK == countyFK); 
        }
    }
}

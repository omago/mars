using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class RegionalOfficesRepository : Repository<RegionalOffice>, IRegionalOfficesRepository
    {
        public RegionalOfficesRepository(ObjectContext context): base(context)
        {

        }

        public RegionalOffice GetRegionalOfficeByPK(int regionalOfficePK)
        {
            RegionalOffice a = GetAll().Where(regionalOffice => regionalOffice.RegionalOfficePK == regionalOfficePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<RegionalOffice> GetValid()
        {
           return GetAll().Where(regionalOffice => regionalOffice.Deleted == false || regionalOffice.Deleted == null);
        }
    }
}

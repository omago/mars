using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class SubstationsRepository : Repository<Substation>, ISubstationsRepository
    {
        public SubstationsRepository(ObjectContext context): base(context)
        {

        }

        public Substation GetSubstationByPK(int substationPK)
        {
            Substation a = GetAll().Where(substation => substation.SubstationPK == substationPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Substation> GetValid()
        {
           return GetAll().Where(substation => substation.Deleted == false || substation.Deleted == null);
        }


        public IQueryable<Substation> GetValidByRegionalOffice(int regionalOfficeFK)
        {
            return  GetValid().Where(p => p.RegionalOfficeFK == regionalOfficeFK); 
        }

    }
}

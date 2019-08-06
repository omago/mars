using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class BussinesShareBurdensRepository : Repository<BussinesShareBurden>, IBussinesShareBurdensRepository
    {
        public BussinesShareBurdensRepository(ObjectContext context): base(context)
        {

        }

        public BussinesShareBurden GetBussinesShareBurdenByPK(int bussinesShareBurdenPK)
        {
            BussinesShareBurden a = GetAll().Where(bussinesShareBurden => bussinesShareBurden.BussinesShareBurdenPK == bussinesShareBurdenPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<BussinesShareBurden> GetValid()
        {
           return GetAll().Where(bussinesShareBurden => bussinesShareBurden.Deleted == false || bussinesShareBurden.Deleted == null);
        }
    }
}

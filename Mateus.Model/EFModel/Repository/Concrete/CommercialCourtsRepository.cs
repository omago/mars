using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class CommercialCourtsRepository : Repository<CommercialCourt>, ICommercialCourtsRepository
    {
        public CommercialCourtsRepository(ObjectContext context)
            : base(context)
        {

        }

        public IQueryable<CommercialCourt> GetValid()
        {
            return GetAll().Where(commercialCourt => commercialCourt.Deleted == false || commercialCourt.Deleted == null);
        }

        public CommercialCourt GetCommercialCourtByPK(int commercialCourtPK)
        {
            CommercialCourt c = GetAll().Where(commercialCourt => commercialCourt.CommercialCourtPK == commercialCourtPK).First();

            if (c != null)
            {
                return c;
            }
            else
            {
                return null;
            }
        }
    }
}

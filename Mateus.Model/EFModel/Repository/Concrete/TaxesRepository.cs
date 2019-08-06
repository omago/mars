using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class TaxesRepository : Repository<Tax>, ITaxesRepository
    {
        public TaxesRepository(ObjectContext context): base(context)
        {

        }

        public Tax GetTaxByPK(int taxPK)
        {
            Tax a = GetAll().Where(tax => tax.TaxPK == taxPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Tax> GetValid()
        {
           return GetAll().Where(tax => tax.Deleted == false || tax.Deleted == null);
        }
    }
}

using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class FormsRepository : Repository<Form>, IFormsRepository
    {
        public FormsRepository(ObjectContext context): base(context)
        {

        }

        public Form GetFormByPK(int formPK)
        {
            Form a = GetAll().Where(form => form.FormPK == formPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Form> GetValid()
        {
           return GetAll().Where(form => form.Deleted == false || form.Deleted == null);
        }
    }
}

using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class BanksRepository : Repository<Bank>, IBanksRepository
    {
        public BanksRepository(ObjectContext context): base(context)
        {

        }

        public Bank GetBankByPK(int bankPK)
        {
            Bank a = GetAll().Where(bank => bank.BankPK == bankPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Bank> GetValid()
        {
           return GetAll().Where(bank => bank.Deleted == false || bank.Deleted == null);
        }
    }
}

using System.Linq;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Support
{
    public static class RepositoryExtensionMethods
    {
        #region LegalEntity

        public static IQueryable<LegalEntity> GetLegalEntitiesByTax(this IQueryable<LegalEntity> legalEntities, int? taxPK)
        {
            return legalEntities.Where(c => c.TaxFK == taxPK);
        }

        public static IQueryable<LegalEntity> GetLegalEntitiesByBank(this IQueryable<LegalEntity> legalEntities, ObjectContext context, int? bankPK)
        {
            IBanksRepository banksRepository = new BanksRepository(context);
            ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(context);
            
            var banks = banksRepository.GetValid();
            var legalEntitiesBanks = legalEntitiesBanksRepository.GetValid();

            var legalEntitiesByBank = (from mn in legalEntitiesBanks
                                   from b in banks.Where(b => b.BankPK == mn.BankFK).DefaultIfEmpty()
                                   from c in legalEntities.Where(c => c.LegalEntityPK == mn.LegalEntityFK).DefaultIfEmpty()
                                   where b.BankPK == bankPK
                                   select c);

            return legalEntitiesByBank;
        }

        #endregion
    }
}
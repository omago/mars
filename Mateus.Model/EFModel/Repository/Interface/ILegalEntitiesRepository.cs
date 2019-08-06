using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ILegalEntitiesRepository : IRepository<LegalEntity>
    {
        LegalEntity GetLegalEntityByPK(int legalEntityPK);
        LegalEntity GetLegalEntityByName(string name);

        IQueryable<LegalEntity> GetValid();
        IQueryable<LegalEntity> GetValidOwners();
        IQueryable<LegalEntity> GetValidLegalEntities();

        //IQueryable<LegalEntity> GetLegalEntitiesByName(string name);
        //IQueryable<LegalEntity> GetLegalEntitiesByOIB(string oib);
        //IQueryable<LegalEntity> GetLegalEntitiesByMB(string mb);
        //IQueryable<LegalEntity> GetLegalEntitiesByMBS(string mbs);

        //IQueryable<LegalEntity> GetLegalEntitiesByEmployeesRange(int? numberOfEmployeesFrom, int? numberOfEmployeesTo);
        //IQueryable<LegalEntity> GetLegalEntitiesByFundamentalCapitalRange(int? fundamentalCapitalFrom, int? fundamentalCapitalTo);

        //IQueryable<LegalEntity> GetLegalEntitiesByBank(int? bankPK);
        //IQueryable<LegalEntity> GetLegalEntitiesByTax(int? taxPK);

        //IQueryable<LegalEntity> GetLegalEntitiesBySubstation(int? substationPK);
        //IQueryable<LegalEntity> GetLegalEntitiesByCommercialCourt(int? commercialCourtPK);

        //IQueryable<LegalEntity> GetLegalEntitiesByForm(int? formPK);
        //IQueryable<LegalEntity> GetLegalEntitiesByActivity(int? activityPK);

        //IQueryable<LegalEntity> GetLegalEntitiesByMonumentAnnuity(bool? monumentAnnuity);
        //IQueryable<LegalEntity> GetLegalEntitiesByTouristOffice(bool? touristOffice);
    }
}

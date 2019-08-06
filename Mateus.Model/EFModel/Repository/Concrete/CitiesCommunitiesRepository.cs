using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class CitiesCommunitiesRepository : Repository<CityCommunity>, ICitiesCommunitiesRepository
    {
        public CitiesCommunitiesRepository(ObjectContext context)
            : base(context)
        {
            base.AuditRepository = true;
        }

        public CityCommunity GetCityCommunityByPK(int cityCommunityPK)
        {
            CityCommunity a = GetAll().Where(cityCommunity => cityCommunity.CityCommunityPK == cityCommunityPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<CityCommunity> GetCitiesCommunitiesByCounty(int countyFK)
        {
            var allCitiesCommunities = GetValid();

            var citiesCommunities = (from cityCommunity in allCitiesCommunities.Where(c => c.CountyFK == countyFK)
                            select cityCommunity);

            return citiesCommunities;
        }

        public IQueryable<CityCommunity> GetValid()
        {
           return GetAll().Where(cityCommunity => cityCommunity.Deleted == false || cityCommunity.Deleted == null);
        }
    }
}

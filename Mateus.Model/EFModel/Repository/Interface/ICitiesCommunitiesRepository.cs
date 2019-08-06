using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ICitiesCommunitiesRepository : IRepository<CityCommunity>
    {
        CityCommunity GetCityCommunityByPK(int cityCommunityPK);

        IQueryable<CityCommunity> GetCitiesCommunitiesByCounty(int countyFK);
        IQueryable<CityCommunity> GetValid();
    }
}

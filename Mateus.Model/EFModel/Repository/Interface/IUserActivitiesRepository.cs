using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IUserActivitiesRepository : IRepository<UserActivity>
    {
        UserActivity GetUserActivityByPK(int userActivityPK);

        IQueryable<UserActivity> GetValid();
    }
}

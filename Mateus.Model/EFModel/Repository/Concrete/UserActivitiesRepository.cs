using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class UserActivitiesRepository : Repository<UserActivity>, IUserActivitiesRepository
    {
        public UserActivitiesRepository(ObjectContext context)
            : base(context)
        {

        }

        public UserActivity GetUserActivityByPK(int userActivityPK)
        {
            var ua = GetAll().Where(userActivity => userActivity.UserActivityPK == userActivityPK);

            if (ua != null && ua.Count() > 0)
            {
                return ua.First();
            }
            else
            {
                return null;
            }
        }

        public IQueryable<UserActivity> GetValid()
        {
            return GetAll().Where(userActivity => userActivity.Deleted == false || userActivity.Deleted == null);
        }
    }
}

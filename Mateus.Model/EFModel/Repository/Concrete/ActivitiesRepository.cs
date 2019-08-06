using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class ActivitiesRepository : Repository<Activity>, IActivitiesRepository
    {
        public ActivitiesRepository(ObjectContext context)
            : base(context)
        {
            base.AuditRepository = true;
        }

        public Activity GetActivityByPK(int activityPK)
        {
            Activity a = GetAll().Where(activity => activity.ActivityPK == activityPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Activity> GetValid()
        {
           return GetAll().Where(activity => activity.Deleted == false || activity.Deleted == null);
        }
    }
}

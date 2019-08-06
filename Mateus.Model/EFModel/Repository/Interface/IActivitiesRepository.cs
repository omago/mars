using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IActivitiesRepository : IRepository<Activity>
    {
        Activity GetActivityByPK(int activityPK);

        IQueryable<Activity> GetValid();
    }
}

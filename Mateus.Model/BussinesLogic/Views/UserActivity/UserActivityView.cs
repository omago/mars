using System;
using System.Linq;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.UserActivityModel
{
    public class UserActivityView
    {
        public int UserActivityPK { get; set; }

        public string ActivityDescription { get; set; }

        public DateTime? ActivityTime { get; set; }

        public bool? Deleted { get; set; }

        public int? UserFK { get; set; }

        public string UserFullName { get; set; }

        public void ConvertFrom(UserActivity userActivity, UserActivityView userActivityView) 
        {
            userActivityView.UserActivityPK = userActivity.UserActivityPK;
            userActivityView.ActivityTime = userActivity.ActivityTime;
            userActivityView.ActivityDescription = userActivity.ActivityDescription;
            userActivityView.Deleted = userActivity.Deleted;
            userActivityView.UserFK = userActivity.UserFK;
        }

        public void ConvertTo(UserActivityView userActivityView, UserActivity userActivity) 
        {
            userActivity.UserActivityPK = userActivityView.UserActivityPK;
            userActivity.ActivityTime = userActivityView.ActivityTime;
            userActivity.ActivityDescription = userActivityView.ActivityDescription;
            userActivity.Deleted = userActivityView.Deleted;
            userActivity.UserFK = userActivityView.UserFK;
        }

        public static IQueryable<UserActivityView> GetHomeView(IQueryable<UserActivity> userActivities, IQueryable<User> users)
        {
            IQueryable<UserActivityView> activitiesView = (from ua in userActivities
                                                           from u in users.Where(u => u.UserPK == ua.UserFK).DefaultIfEmpty()
                                                           select new UserActivityView
                                                           {
                                                               UserActivityPK = ua.UserActivityPK,
                                                               ActivityDescription = ua.ActivityDescription,
                                                               ActivityTime = ua.ActivityTime,
                                                               UserFK = ua.UserFK,
                                                               UserFullName = (from u1 in users 
                                                                               where u1.UserPK == ua.UserFK 
                                                                               select new { FullName = u1.FirstName + " " + u1.LastName })
                                                                               .Select( r => r.FullName).FirstOrDefault()
                                                           }).AsQueryable<UserActivityView>();

            return activitiesView;
        }

        public static UserActivity LogUserActivity(int? UserFK, string ActivityDescription, DateTime ActivityTime) 
        {
            UserActivity userActivity = new UserActivity();

            userActivity.UserFK = UserFK;
            userActivity.ActivityDescription = ActivityDescription;
            userActivity.ActivityTime = ActivityTime;

            return userActivity;
        }
    }
}

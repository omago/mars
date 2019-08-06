using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.ActivityModel
{
    public class ActivityView
    {
        public int? ActivityPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Šifra je obavezna."), StringLength(256, ErrorMessage = "Šifra ne smije biti duža od 10 znakova.")]
        public string Code { get; set; }

        public bool? HighRisk { get; set; }
        public bool? LowRisk { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(Activity activity, ActivityView activityView) 
        {
            activityView.ActivityPK = activity.ActivityPK;
            activityView.Name = activity.Name;
            activityView.Code = activity.Code;
            activityView.Deleted = activity.Deleted;
            activityView.HighRisk = activity.HighRisk;
            activityView.LowRisk = activity.LowRisk;
        }

        public void ConvertTo(ActivityView activityView, Activity activity) 
        {
            activity.Name = activityView.Name;
            activity.Code = activityView.Code;
            activity.Deleted = activityView.Deleted;
            activity.HighRisk = activityView.HighRisk;
            activity.LowRisk = activityView.LowRisk;
        }

        public static IQueryable<ActivityView> GetHomeView(IQueryable<Activity> activities)
        {
            IQueryable<ActivityView> activitiesView = (from a in activities
                                                     select new ActivityView
                                                 {
                                                     ActivityPK = a.ActivityPK,
                                                     Name = a.Name,
                                                     Code = a.Code,
                                                     Deleted = a.Deleted
                                                 }).AsQueryable<ActivityView>();

            return activitiesView;
        }
    }
}

using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Support;
using System.Data.Objects.SqlClient;

namespace Mateus.Controllers
{
    public class AjaxController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AjaxController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region DDL

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetCountiesByCountry(int country)
        {
            ICountiesRepository countiesRepository = new CountiesRepository(db);
            var counties = countiesRepository.GetCountiesByCountry(country).OrderBy(c => c.Name);

            return Json(counties.Select(c => new { value = c.CountyPK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetCitiesCommunitiesByCounty(int county)
        {
            ICitiesCommunitiesRepository citiesRepository = new CitiesCommunitiesRepository(db);
            var cities = citiesRepository.GetCitiesCommunitiesByCounty(county).OrderBy(c => c.Name);

            return Json(cities.Select(c => new { value = c.CityCommunityPK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetPostalOfficesByCounty(int county)
        {
            IPostalOfficesRepository postalOfficesRepository = new PostalOfficesRepository(db);
            var postalOffices = postalOfficesRepository.GetValidByCounty(county).OrderBy(c => c.Name);

            return Json(postalOffices.Select(c => new { value = c.PostalOfficePK, text = c.Name + " (" + SqlFunctions.StringConvert((double)c.Number).Trim() + ")" }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetPlacesByPostalOffice(int postalOffice)
        {
            IPlacesRepository placesRepository = new PlacesRepository(db);
            var places = placesRepository.GetPlacesByPostalOffice(postalOffice).OrderBy(x => x.Name);

            return Json(places.Select(c => new { value = c.PlacePK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetSubstationsByRegionalOffice(int regionalOffice)
        {
            ISubstationsRepository substationsRepository = new SubstationsRepository(db);
            var substations = substationsRepository.GetValidByRegionalOffice(regionalOffice).OrderBy(x => x.Name);

            return Json(substations.Select(c => new { value = c.SubstationPK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetWorkSubtypesByWorkType(int workType)
        {
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            var workSubtypes = workSubtypesRepository.GetValidByWorkType(workType).OrderBy(c => c.Name);

            return Json(workSubtypes.Select(c => new { value = c.WorkSubtypePK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetAssessmentGroupsByType(int assessmentType)
        {
            IAssessmentGroupsRepository assessmentGroupsRepository = new AssessmentGroupsRepository(db);
            var assessmentGroups = assessmentGroupsRepository.GetAssessmentGroupsByType(assessmentType).OrderBy(c => c.Name);

            return Json(assessmentGroups.Select(c => new { value = c.AssessmentGroupPK, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult GetReferentsBySubstation(int substation)
        {
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            var physicalEntities = physicalEntitiesRepository.GetValidReferentsBySubstation(substation).OrderBy(c => c.Firstname);

            return Json(physicalEntities.Select(c => new { value = c.PhysicalEntityPK, text = c.Firstname + " " + c.Lastname }), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Autocompletes

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntities(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValid().Where(c => c.OIB.Contains(term) || c.Name.Contains(term)).OrderBy(x => x.Name);

            return Json(legalEntities.Select(c => new { value = c.Name + " (" + c.OIB + ")", value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntitiesWithOIB(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValidLegalEntities().Where(c => c.OIB.Contains(term) || c.Name.Contains(term)).OrderBy(x => x.Name);

            return Json(legalEntities.Select(c => new { value = c.Name + " (" + c.OIB + ")", value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntitiesName(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValidLegalEntities().Where(c => c.Name.Contains(term)).OrderBy(x => x.Name);

            return Json(legalEntities.Select(c => new { value = c.Name, value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntitiesMB(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValidLegalEntities().Where(c => c.MB.Contains(term)).OrderBy(x => x.MB);

            return Json(legalEntities.Select(c => new { value = c.MB, value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntitiesMBS(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValidLegalEntities().Where(c => c.MBS.Contains(term)).OrderBy(x => x.MBS);

            return Json(legalEntities.Select(c => new { value = c.MBS, value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteLegalEntitiesOIB(string term) 
        { 
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            var legalEntities = legalEntitiesRepository.GetValidLegalEntities().Where(c => c.OIB.Contains(term)).OrderBy(x => x.OIB);

            return Json(legalEntities.Select(c => new { value = c.OIB, value_id = c.LegalEntityPK }), JsonRequestBehavior.AllowGet);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult AutocompleteActivities(string term) 
        { 
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
            var legalEntities = activitiesRepository.GetValid().Where(c => c.Name.Contains(term) || c.Code.Contains(term)).OrderBy(x => x.Name);

            return Json(legalEntities.Select(c => new { value = c.Name + " (" + c.Code + ")", value_id = c.ActivityPK }), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
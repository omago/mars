using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ServiceTypeModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class ServiceTypeController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ServiceTypeController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "ServiceTypePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<ServiceType> serviceTypes = serviceTypesRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                serviceTypes = serviceTypes.Where(c => c.Name.Contains(searchString));
            }

            serviceTypes = serviceTypes.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = serviceTypesRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = serviceTypesRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("ServiceType?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", serviceTypes.ToList());
            }

        }

        #endregion

        #region Add new ServiceType

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ServiceTypeView serviceTypeView)
        {
            if (ModelState.IsValid)
            {
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                ServiceType serviceType = new ServiceType();

                serviceTypeView.ConvertTo(serviceTypeView, serviceType);

                serviceTypesRepository.Add(serviceType);
                serviceTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", serviceType.ServiceTypePK);

                return RedirectToAction("Index", "ServiceType");
            }
            else
            {
                return View(serviceTypeView);
            }
        }

        #endregion

        #region Edit ServiceType

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? serviceTypePK)
        {
            if (serviceTypePK != null)
            {
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                ServiceType serviceType = serviceTypesRepository.GetServiceTypeByPK((int)serviceTypePK);
                ServiceTypeView serviceTypeView = new ServiceTypeView();

                serviceTypeView.ConvertFrom(serviceType, serviceTypeView);

                return View(serviceTypeView);
            }
            else
            {
                return RedirectToAction("Index", "ServiceType");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ServiceTypeView serviceTypeModel)
        {
            if (ModelState.IsValid)
            {
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                ServiceType serviceType = serviceTypesRepository.GetServiceTypeByPK((int)serviceTypeModel.ServiceTypePK);
                serviceTypeModel.ConvertTo(serviceTypeModel, serviceType);

                serviceTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", serviceType.ServiceTypePK);

                return RedirectToAction("Index", "ServiceType");
            }
            else
            {
                return View(serviceTypeModel);
            }
        }

        #endregion

        #region Delete ServiceType
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? serviceTypePK)
        {
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
            if (serviceTypePK != null)
            {
                ServiceType serviceType = serviceTypesRepository.GetServiceTypeByPK((int)serviceTypePK);

                serviceType.Deleted = true;

                serviceTypesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", serviceType.ServiceTypePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}

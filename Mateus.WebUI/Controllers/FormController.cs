using System;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.FormModel;
using Mateus.Support;
using PITFramework.Support.Grid;

namespace Mateus.Controllers
{
    public class FormController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public FormController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IFormsRepository formsRepository = new FormsRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "FormPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<Form> forms = formsRepository.GetValid()
                                                        .OrderBy(ordering);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                forms = forms.Where(c => c.Name.Contains(searchString));
            }

            forms = forms.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = formsRepository.GetValid().Where(c => c.Name.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = formsRepository.GetValid().Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("Form?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", forms.ToList());
            }

        }

        #endregion

        #region Add new Form

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(FormView formView)
        {
            if (ModelState.IsValid)
            {
                IFormsRepository formsRepository = new FormsRepository(db);
                Form form = new Form();

                formView.ConvertTo(formView, form);

                formsRepository.Add(form);
                formsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("INSERT", form.FormPK);

                return RedirectToAction("Index", "Form");
            }
            else
            {
                return View(formView);
            }
        }

        #endregion

        #region Edit Form

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? formPK)
        {
            if (formPK != null)
            {
                IFormsRepository formsRepository = new FormsRepository(db);
                Form form = formsRepository.GetFormByPK((int)formPK);
                FormView formView = new FormView();

                formView.ConvertFrom(form, formView);

                return View(formView);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(FormView formView)
        {
            if (ModelState.IsValid)
            {
                IFormsRepository formsRepository = new FormsRepository(db);

                Form form = formsRepository.GetFormByPK((int)formView.FormPK);
                formView.ConvertTo(formView, form);

                formsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", form.FormPK);

                return RedirectToAction("Index", "Form");
            }
            else
            {
                return View(formView);
            }
        }

        #endregion

        #region Delete Form
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? formPK)
        {
            IFormsRepository formsRepository = new FormsRepository(db);
            if (formPK != null)
            {
                Form form = formsRepository.GetFormByPK((int)formPK);

                form.Deleted = true;

                formsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", form.FormPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}

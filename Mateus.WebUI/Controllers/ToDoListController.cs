using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.ToDoListModel;
using Mateus.Model.BussinesLogic.Views.GeneratorModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using PITFramework.Security;

namespace Mateus.Controllers
{
    public class ToDoListController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public ToDoListController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            IUsersRepository usersRepository = new UsersRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "ASC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "Deadline";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<ToDoListView> toDoLists = ToDoListView.GetToDoListView(toDoListsRepository.GetValid(), workDonesRepository.GetValid(), usersRepository.GetValid())
                                                             .OrderBy(ordering);

            string status = "obligations";
            if (!String.IsNullOrWhiteSpace(Request.QueryString["Status"]))
            {
                status = Request.QueryString["Status"];
            }

            ViewBag.dateFrom = Request.QueryString["dateFrom"] != null ? Request.QueryString["dateFrom"] : DateTime.Now.AddDays(-30).ToString("dd.MM.yyyy.");
            ViewBag.dateTo = Request.QueryString["dateTo"] != null ? Request.QueryString["dateTo"] : DateTime.Now.AddDays(30).ToString("dd.MM.yyyy.");

            ViewBag.FinishedStatuses = new SelectList(GeneratorView.GenerateFinishedStauses(), "Value", "Text", status);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                toDoLists = toDoLists.Where(c => c.Name.Contains(searchString));
            }

            if (status == "obligations")
            {
                toDoLists = toDoLists.Where(c => c.Finished == false || c.Finished == null);
            }
            else
            {
                if (status == "finished")
                {
                    toDoLists = toDoLists.Where(c => c.Finished == true);
                }

                if (Request.QueryString["dateFrom"] != "")
                {
                    DateTime dateFromDate = new DateTime();
                    dateFromDate = DateTime.ParseExact(ViewBag.dateFrom, "dd.MM.yyyy.", null);
                    toDoLists = toDoLists.Where(c => c.Deadline >= dateFromDate);
                }

                if (Request.QueryString["dateTo"] != "")
                {
                    DateTime dateToDate = new DateTime();
                    dateToDate = DateTime.ParseExact(ViewBag.dateTo, "dd.MM.yyyy.", null);
                    toDoLists = toDoLists.Where(c => c.Deadline <= dateToDate);
                }
            }

            toDoLists = toDoLists.Page(page, pageSize);

            ViewData["numberOfRecords"] = toDoLists.Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("ToDoList?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", toDoLists.ToList());
            }

        }

        #endregion

        #region Add new ToDoList

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? toDoListPK)
        {
            ToDoListView toDoListView = new ToDoListView();

            if (toDoListPK != null)
            {
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int) toDoListPK);
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);

                toDoList.Finished = false;

                toDoListView.ConvertFrom(toDoList, toDoListView);

                var legalEntitiesSelectedValue = workDonesRepository.GetWorkDonesCreatedWithToDo((int) toDoListPK).Select(a => a.LegalEntityFK);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                toDoListView.Companies = new MultiSelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name ASC"), "LegalEntityPK", "Name", legalEntitiesSelectedValue);
            }
            else
            {
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                toDoListView.Companies = new MultiSelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name ASC"), "LegalEntityPK", "Name"); 
            }

            return View(toDoListView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(ToDoListView toDoListView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                ToDoList toDoList = new ToDoList();

                toDoListView.ConvertTo(toDoListView, toDoList);

                toDoList.UserFK = SecurityHelper.GetUserPKFromCookie();
                toDoList.CreationDate = DateTime.Now;

                toDoListsRepository.Add(toDoList);
                toDoListsRepository.SaveChanges();

                string[] legalEntitiesSelectedValues = new string[500];
                if (form["LegalEntityPK"] != null)
                {
                    legalEntitiesSelectedValues = ((string) form["LegalEntityPK"]).Split(',');
                }
                else
                {
                    legalEntitiesSelectedValues = null;
                }

                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);

                if (legalEntitiesSelectedValues != null)
                {
                    // Add work dones
                    foreach (string legalEntity in legalEntitiesSelectedValues)
                    {
                        WorkDone workDone = new WorkDone();

                        workDone.ToDoListFK = toDoList.ToDoListPK;
                        workDone.LegalEntityFK = Convert.ToInt32(legalEntity);
                        workDone.CreatedWithToDo = true;

                        workDonesRepository.Add(workDone);
                    }

                    workDonesRepository.SaveChanges();
                }
                 
                TempData["message"] = LayoutHelper.GetMessage("INSERT", toDoList.ToDoListPK);

                return RedirectToAction("Index", "ToDoList");
            }
            else
            {
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                toDoListView.Companies = new MultiSelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name ASC"), "LegalEntityPK", "Name");

                return View(toDoListView);
            }
        }

        #endregion

        #region Edit ToDoList

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? toDoListPK)
        {
            if (toDoListPK != null)
            {
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int)toDoListPK);
                ToDoListView toDoListView = new ToDoListView();
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);

                toDoListView.ConvertFrom(toDoList, toDoListView);

                var legalEntitiesSelectedValue =  workDonesRepository.GetWorkDonesCreatedWithToDo((int)toDoListPK).Select(a => a.LegalEntityFK);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                toDoListView.Companies = new MultiSelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name ASC"), "LegalEntityPK", "Name", legalEntitiesSelectedValue);

                return View(toDoListView);
            }
            else
            {
                return RedirectToAction("Index", "ToDoList");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(ToDoListView toDoListView, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int)toDoListView.ToDoListPK);
                toDoListView.ConvertTo(toDoListView, toDoList);

                toDoListsRepository.SaveChanges();

                string[] legalEntitiesSelectedValues = new string[500];
                if (form["LegalEntityPK"] != null)
                {
                    legalEntitiesSelectedValues = ((string) form["LegalEntityPK"]).Split(',');
                }
                else
                {
                    legalEntitiesSelectedValues = null;
                }

                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                
                var legalEntitiesValueFromDB =  workDonesRepository.GetWorkDonesCreatedWithToDo((int)toDoListView.ToDoListPK).Select(a => a.LegalEntityFK);

                if (legalEntitiesSelectedValues != null)
                {
                    // Add work dones
                    foreach (string legalEntity in legalEntitiesSelectedValues)
                    {
                        int legalEntityFK = Convert.ToInt32(legalEntity);

                        if (!legalEntitiesValueFromDB.Contains(legalEntityFK))
                        {
                            WorkDone workDone = new WorkDone();

                            workDone.ToDoListFK = toDoList.ToDoListPK;
                            workDone.LegalEntityFK = legalEntityFK;
                            workDone.CreatedWithToDo = true;

                            workDonesRepository.Add(workDone);
                        }
                    }

                    workDonesRepository.SaveChanges();

                    List<WorkDone> workDones =  workDonesRepository.GetWorkDonesCreatedWithToDo((int)toDoListView.ToDoListPK).ToList();

                    // delete existing
                    foreach (WorkDone workDone in workDones)
                    {
                        int legalEntityFK = (int)workDone.LegalEntityFK;

                        if (!legalEntitiesSelectedValues.ToList().Contains(legalEntityFK.ToString()))
                        {
                            workDone.Deleted = true;

                            workDonesRepository.SaveChanges();
                        }
                    }

                }

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", toDoList.ToDoListPK);

                return RedirectToAction("Index", "ToDoList");
            }
            else
            {
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);

                var legalEntitiesSelectedValue =  workDonesRepository.GetWorkDonesCreatedWithToDo((int)toDoListView.ToDoListPK).Select(a => a.LegalEntityFK);

                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                toDoListView.Companies = new MultiSelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name ASC"), "LegalEntityPK", "Name", legalEntitiesSelectedValue);

                return View(toDoListView);
            }
        }

        #endregion

        #region Finish ToDoList

        [PITAuthorize(Roles = "edit")]
        public ActionResult Unfinish(int? toDoListPK)
        {
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            if (toDoListPK != null)
            {
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int)toDoListPK);

                toDoList.Finished = false;

                toDoListsRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "edit")]
        public ActionResult Finish(int? toDoListPK)
        {
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            if (toDoListPK != null)
            {
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int)toDoListPK);

                toDoList.Finished = true;

                toDoListsRepository.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

        #region Delete ToDoList
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? toDoListPK)
        {
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            if (toDoListPK != null)
            {
                ToDoList toDoList = toDoListsRepository.GetToDoListByPK((int)toDoListPK);

                toDoList.Deleted = true;

                toDoListsRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", toDoList.ToDoListPK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion
    }
}

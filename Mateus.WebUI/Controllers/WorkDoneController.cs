using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.WorkDoneModel;
using Mateus.Model.BussinesLogic.Views.WorkDoneAttachmentModel;
using Mateus.Model.BussinesLogic.Views.GeneratorModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using System.IO;
using PITFramework.Auditing;
using PITFramework.Security;

namespace Mateus.Controllers
{
    public class WorkDoneController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public WorkDoneController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
            WorkDoneView workDoneView = new WorkDoneView();
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
            IUsersRepository usersRepository = new UsersRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkDonePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<WorkDoneView> workDones = WorkDoneView.GetWorkDoneView(workDonesRepository.GetValid(), toDoListsRepository.GetValid(), workDoneAttachmentsRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities(), workTypesRepository.GetValid(), workSubtypesRepository.GetValid(), serviceTypesRepository.GetValid(), usersRepository.GetValid())
                                                        .OrderBy(ordering);

            //grid filters ddl
            ViewBag.ToDoLists = new SelectList(toDoListsRepository.GetValid().ToList(), "ToDoListPK", "Name", Request.QueryString["toDoListFK"]);
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("name").ToList(), "LegalEntityPK", "Name", Request.QueryString["legalEntityFK"]);
            ViewBag.WorkTypes = new SelectList(workTypesRepository.GetValid().OrderBy("name").ToList(), "WorkTypePK", "Name", Request.QueryString["workTypeFK"]);
            if (!String.IsNullOrWhiteSpace(Request.QueryString["workTypeFK"]))
            {
                int workTypeFK = Convert.ToInt32(Request.QueryString["workTypeFK"]);
                ViewBag.WorkSubtypes = new SelectList(workSubtypesRepository.GetValidByWorkType(workTypeFK).OrderBy("name").ToList(), "WorkSubtypePK", "Name", Request.QueryString["workSubtypeFK"]);
            }
            else
            {
                ViewBag.WorkSubtypes = new SelectList(new List<County>(), "WorkSubtypePK", "Name");
            }
            ViewBag.Users = new SelectList(usersRepository.GetValid().OrderBy("username").ToList(), "UserPK", "Username", Request.QueryString["userFK"]);

            // search construct
            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                workDones = workDones.Where(c => c.Description.Contains(searchString) || c.Comment.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["createdWithToDo"]))
            {
                bool createdWithToDo = Convert.ToBoolean(Request.QueryString["createdWithToDo"]);
                workDones = workDones.Where(c => c.CreatedWithToDo == createdWithToDo);
            }
            else
            {
                workDones = workDones.Where(c => c.Description != null && c.WorkTypeName != null && c.WorkSubtypeName != null && c.ServiceTypeName != null && c.TimeSpent != null);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["toDoListFK"]))
            {
                int toDoListFK = Convert.ToInt32(Request.QueryString["toDoListFK"]);
                workDones = workDones.Where(c => c.ToDoListFK == toDoListFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["legalEntityFK"]))
            {
                int legalEntityFK = Convert.ToInt32(Request.QueryString["legalEntityFK"]);
                workDones = workDones.Where(c => c.LegalEntityFK == legalEntityFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["workTypeFK"]))
            {
                int workTypeFK = Convert.ToInt32(Request.QueryString["workTypeFK"]);
                workDones = workDones.Where(c => c.WorkTypeFK == workTypeFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["workSubtypeFK"]))
            {
                int workSubtypeFK = Convert.ToInt32(Request.QueryString["workSubtypeFK"]);
                workDones = workDones.Where(c => c.WorkSubtypeFK == workSubtypeFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["userFK"]))
            {
                int userFK = Convert.ToInt32(Request.QueryString["userFK"]);
                workDones = workDones.Where(c => c.UserFK == userFK);
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["date"]))
            {
                DateTime Date = DateTime.ParseExact(Request.QueryString["date"], "dd.MM.yyyy.", null);
                workDones = workDones.Where(c => c.Date == Date);
            }

            workDones = workDones.Page(page, pageSize);

            ViewData["numberOfRecords"] = workDones.Count();

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("WorkDone?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", workDones.ToList());
            }
        }

        #endregion

        #region Add new WorkDone

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? toDoListFK)
        {
            WorkDoneView workDoneView = new WorkDoneView();

            //to do list ddl
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            workDoneView.ToDoLists = new SelectList(toDoListsRepository.GetNotFinished().ToList(), "ToDoListPK", "Name");

            //legalEntities ddl
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            workDoneView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name");

            //service type ddl
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
            workDoneView.ServiceTypes = new SelectList(serviceTypesRepository.GetValid().ToList(), "ServiceTypePK", "Name");

            //Work type ddl
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            workDoneView.WorkTypes = new SelectList(workTypesRepository.GetValid().ToList(), "WorkTypePK", "Name");

            //worksubtypes ddl
            workDoneView.WorkSubtypes = new SelectList(new List<County>(), "WorkSubtypePK", "Name");

            //hours and minutes ddl
            workDoneView.Hours = GeneratorView.GenerateHours();
            workDoneView.Minutes = GeneratorView.GenerateMinutes();

            if (toDoListFK != null)
            {
                workDoneView.ToDoListFK = toDoListFK;
            }

            workDoneView.ServiceTypeFK = 1;

            return View(workDoneView);
        }


        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(WorkDoneView workDoneView, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                WorkDone workDone = new WorkDone();

                workDoneView.ConvertTo(workDoneView, workDone);

                workDone.UserFK = SecurityHelper.GetUserPKFromCookie();
                workDone.CreationDate = DateTime.Now;

                workDonesRepository.Add(workDone);
                workDonesRepository.SaveChanges(sessionToken);

                TempData["message"] = LayoutHelper.GetMessage("INSERT", workDone.WorkDonePK);

                int workDoneFK = workDone.WorkDonePK;

                foreach (var file in files) {
                    if(file != null && file.ContentLength > 0) {

                        // save attachment to local drive
                        string originalFileName = Path.GetFileName(file.FileName); 
                        string fileExtension = Path.GetExtension(originalFileName);
                        string newFileName = System.Guid.NewGuid().ToString();
                       
                        var path = Path.Combine(Path.GetDirectoryName(Server.MapPath("~")) + "\\Attachments", newFileName + fileExtension);

                        file.SaveAs(path);

                        // save attachment to database
                        IAttachmentsRepository attachmentsRepository = new AttachmentsRepository(db);

                        Attachment attachment = new Attachment();

                        attachment.Name = originalFileName;
                        attachment.Filename = newFileName;
                        attachment.Extension = fileExtension;
                        attachment.ContentType = "text/plain";

                        attachmentsRepository.Add(attachment);
                        attachmentsRepository.SaveChanges(sessionToken);

                        int attachmentFK = attachment.AttachmentPK;

                        // save attachment to work done attachments
                        IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);

                        WorkDoneAttachment workDoneAttachment = new WorkDoneAttachment();
 
                        workDoneAttachment.AttachmentFK = attachmentFK;
                        workDoneAttachment.WorkDoneFK = workDoneFK;

                        workDoneAttachmentsRepository.Add(workDoneAttachment);
                        workDoneAttachmentsRepository.SaveChanges(sessionToken);
                    }
                }

                return RedirectToAction("Index", "WorkDone");
            }
            else
            {
                //to do list ddl
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                workDoneView.ToDoLists = new SelectList(toDoListsRepository.GetValid().ToList(), "ToDoListPK", "Name");

                //legalEntities ddl
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                workDoneView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name");

                //service type ddl
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                workDoneView.ServiceTypes = new SelectList(serviceTypesRepository.GetValid().ToList(), "ServiceTypePK", "Name");

                //Work done ddl
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                workDoneView.WorkTypes = new SelectList(workTypesRepository.GetValid().ToList(), "WorkTypePK", "Name");

                //worksubtypes ddl
                if (!String.IsNullOrWhiteSpace(form["WorkTypeFK"]))
                {
                    IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                    workDoneView.WorkSubtypes = new SelectList(workSubtypesRepository.GetValidByWorkType(Convert.ToInt32(form["WorkTypeFK"])), "WorkSubtypePK", "Name", form["WorkSubtypeFK"]);
                }
                else
                {
                    workDoneView.WorkSubtypes = new SelectList(new List<County>(), "WorkSubtypePK", "Name");
                }

                //hours and minutes ddl
                workDoneView.Hours = GeneratorView.GenerateHours();
                workDoneView.Minutes = GeneratorView.GenerateMinutes();

                return View(workDoneView);
            }
        }

        #endregion

        #region Edit WorkDone

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? workDonePK)
        {
            if (workDonePK != null)
            {
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                WorkDone workDone = workDonesRepository.GetWorkDoneByPK((int)workDonePK);
                WorkDoneView workDoneView = new WorkDoneView();

                workDoneView.ConvertFrom(workDone, workDoneView);

                IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
                IAttachmentsRepository attachmentsRepository = new AttachmentsRepository(db);
                IQueryable<WorkDoneAttachmentView> workDoneAttachments = WorkDoneAttachmentView.GetWorkDoneAttachmentView(workDoneAttachmentsRepository.GetValid(), attachmentsRepository.GetValid(), workDonesRepository.GetValid())
                                                                                    .Where(c => c.WorkDoneFK == workDonePK).OrderBy("WorkDoneAttachmentPK DESC");

                ViewBag.Attachments = workDoneAttachments.ToList();

                //to do list ddl
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                workDoneView.ToDoLists = new SelectList(toDoListsRepository.GetValid().ToList(), "ToDoListPK", "Name");

                //legalEntities ddl
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                workDoneView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name");

                //service type ddl
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                workDoneView.ServiceTypes = new SelectList(serviceTypesRepository.GetValid().ToList(), "ServiceTypePK", "Name");

                //Work done ddl
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                workDoneView.WorkTypes = new SelectList(workTypesRepository.GetValid().ToList(), "WorkTypePK", "Name");

                //worksubtypes ddl
                if (workDoneView.WorkTypeFK != null)
                {
                    IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                    workDoneView.WorkSubtypes = new SelectList(workSubtypesRepository.GetValidByWorkType(Convert.ToInt32(workDoneView.WorkTypeFK)), "WorkSubtypePK", "Name", workDoneView.WorkSubtypeFK);
                }
                else
                {
                    workDoneView.WorkSubtypes = new SelectList(new List<County>(), "WorkSubtypePK", "Name");
                }

                //hours and minutes ddl
                workDoneView.Hours = GeneratorView.GenerateHours();
                workDoneView.Minutes = GeneratorView.GenerateMinutes();

                return View(workDoneView);
            }
            else
            {
                return RedirectToAction("Index", "WorkDone");
            }
        }


        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(WorkDoneView workDoneView, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                WorkDone workDone = workDonesRepository.GetWorkDoneByPK((int)workDoneView.WorkDonePK);

                // set last user as entry creator
                if (workDone.UserFK == null)
                {
                    workDone.UserFK = SecurityHelper.GetUserPKFromCookie();
                }

                if (workDone.CreationDate == null)
                {
                    workDone.CreationDate = DateTime.Now;
                }

                workDoneView.ConvertTo(workDoneView, workDone);

                workDonesRepository.SaveChanges(sessionToken);

                TempData["message"] = LayoutHelper.GetMessage("UPDATE", workDone.WorkDonePK);

                int workDoneFK = workDone.WorkDonePK;

                foreach (var file in files) {
                    if (file != null && file.ContentLength > 0) {

                        // save attachment to local drive
                        string originalFileName = Path.GetFileName(file.FileName); 
                        string fileExtension = Path.GetExtension(originalFileName);
                        string newFileName = System.Guid.NewGuid().ToString();
                       
                        var path = Path.Combine(Path.GetDirectoryName(Server.MapPath("~")) + "\\Attachments", newFileName + fileExtension);

                        file.SaveAs(path);

                        // save attachment to database
                        IAttachmentsRepository attachmentsRepository = new AttachmentsRepository(db);

                        Attachment attachment = new Attachment();

                        attachment.Name = originalFileName;
                        attachment.Filename = newFileName;
                        attachment.Extension = fileExtension;
                        attachment.ContentType = "text/plain";

                        attachmentsRepository.Add(attachment);
                        attachmentsRepository.SaveChanges(sessionToken);

                        int attachmentFK = attachment.AttachmentPK;

                        // save attachment to work done attachments
                        IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);

                        WorkDoneAttachment workDoneAttachment = new WorkDoneAttachment();
 
                        workDoneAttachment.AttachmentFK = attachmentFK;
                        workDoneAttachment.WorkDoneFK = workDoneFK;

                        workDoneAttachmentsRepository.Add(workDoneAttachment);
                        workDoneAttachmentsRepository.SaveChanges(sessionToken);
                    }

                }

                return RedirectToAction("Index", "WorkDone");
            }
            else
            {
                //to do list ddl
                IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
                workDoneView.ToDoLists = new SelectList(toDoListsRepository.GetValid().ToList(), "ToDoListPK", "Name");

                //legalEntities ddl
                ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
                workDoneView.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name");

                //service type ddl
                IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
                workDoneView.ServiceTypes = new SelectList(serviceTypesRepository.GetValid().ToList(), "ServiceTypePK", "Name");

                //Work done ddl
                IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
                workDoneView.WorkTypes = new SelectList(workTypesRepository.GetValid().ToList(), "WorkTypePK", "Name");

                //worksubtypes ddl
                if (!String.IsNullOrWhiteSpace(form["WorkTypeFK"]))
                {
                    IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
                    workDoneView.WorkSubtypes = new SelectList(workSubtypesRepository.GetValidByWorkType(Convert.ToInt32(form["WorkTypeFK"])), "WorkSubtypePK", "Name", form["WorkSubtypeFK"]);
                }
                else
                {
                    workDoneView.WorkSubtypes = new SelectList(new List<County>(), "WorkSubtypePK", "Name");
                }

                //hours and minutes ddl
                workDoneView.Hours = GeneratorView.GenerateHours();
                workDoneView.Minutes = GeneratorView.GenerateMinutes();

                return View(workDoneView);
            }
        }

        #endregion

        #region Delete WorkDone
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? workDonePK)
        {
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            if (workDonePK != null)
            {
                WorkDone workDone = workDonesRepository.GetWorkDoneByPK((int)workDonePK);

                workDone.Deleted = true;

                workDonesRepository.SaveChanges();

                TempData["message"] = LayoutHelper.GetMessage("DELETE", workDone.WorkDonePK);
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

    }
}
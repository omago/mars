using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using PITFramework.Support;
using Mateus.Model.BussinesLogic.Views.WorkDoneAttachmentModel;
using Mateus.Support;
using PITFramework.Support.Grid;
using System.IO;
using PITFramework.Auditing;
using PITFramework.Support;

namespace Mateus.Controllers
{
    public class WorkDoneAttachmentController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public WorkDoneAttachmentController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        #region Index

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult Index()
        {
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
            IAttachmentsRepository attachmentsRepository = new AttachmentsRepository(db);
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);

            int page = !String.IsNullOrWhiteSpace(Request.QueryString["page"]) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            int pageSize = !String.IsNullOrWhiteSpace(Request.QueryString["pageSize"]) ? Convert.ToInt32(Request.QueryString["pageSize"]) : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResultsPerPage"]);
            string sortOrder = !String.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !String.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkDoneAttachmentPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IQueryable<WorkDoneAttachmentView> workDoneAttachments = WorkDoneAttachmentView.GetWorkDoneAttachmentView(workDoneAttachmentsRepository.GetValid(), attachmentsRepository.GetValid(), workDonesRepository.GetValid())
                                                                                            .OrderBy(ordering);

            //WorkDones ddl
            ViewBag.WorkDones = new SelectList(workDonesRepository.GetValid().OrderBy("comment").ToList(), "WorkDonePK", "Description", Request.QueryString["workDoneFK"]);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                workDoneAttachments = workDoneAttachments.Where(c => c.AttachmentName.Contains(searchString));
            }

            if (!String.IsNullOrWhiteSpace(Request.QueryString["workDoneFK"]))
            {
                int workDoneFK = Convert.ToInt32(Request.QueryString["workDoneFK"]);
                workDoneAttachments = workDoneAttachments.Where(c => c.WorkDoneFK == workDoneFK);
            }

            workDoneAttachments = workDoneAttachments.Page(page, pageSize);

            if (!String.IsNullOrWhiteSpace(Request.QueryString["searchString"]))
            {
                string searchString = Request.QueryString["searchString"].ToString();
                ViewData["numberOfRecords"] = workDoneAttachments.Where(c => c.AttachmentName.Contains(searchString)).Count();
            }
            else
            {
                ViewData["numberOfRecords"] = workDoneAttachments.Count();
            }

            int numberOfPages = ((int)ViewData["numberOfRecords"] + pageSize - 1) / pageSize;

            if (page > numberOfPages)
            {
                string url = LinkHelper.getQueryStringArray(new string[] { "page" });
                return Redirect("WorkDoneAttachment?" + url + "page=" + numberOfPages);
            }
            else
            {
                return View("Index", workDoneAttachments.ToList());
            }

        }

        #endregion

        #region Add new WorkDoneAttachment

        [PITAuthorize(Roles = "add")]
        [HttpGet]
        public ActionResult Add(int? workDoneFK)
        {
            WorkDoneAttachmentView workDoneAttachmentView = new WorkDoneAttachmentView();

            //WorkDones ddl
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            workDoneAttachmentView.WorkDones = new SelectList(workDonesRepository.GetValid().OrderBy("Description ASC").ToList(), "WorkDonePK", "Description");

            if(workDoneFK != null) {
                workDoneAttachmentView.WorkDoneFK = workDoneFK;
            }

            return View(workDoneAttachmentView);
        }

        [PITAuthorize(Roles = "add")]
        [HttpPost]
        public ActionResult Add(WorkDoneAttachmentView workDoneAttachmentView, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                int workDoneFK = (int)workDoneAttachmentView.WorkDoneFK;

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

                return RedirectToAction("Index", "WorkDoneAttachment");
            }
            else
            {
                //WorkDones ddl
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                workDoneAttachmentView.WorkDones = new SelectList(workDonesRepository.GetValid().OrderBy("Description ASC").ToList(), "WorkDonePK", "Description");

                return View(workDoneAttachmentView);
            }
        }

        #endregion

        #region Edit WorkDoneAttachment

        [PITAuthorize(Roles = "edit")]
        [HttpGet]
        public ActionResult Edit(int? workDoneAttachmentPK)
        {
            if (workDoneAttachmentPK != null)
            {
                IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
                WorkDoneAttachment workDoneAttachment = workDoneAttachmentsRepository.GetWorkDoneAttachmentByPK((int)workDoneAttachmentPK);
                WorkDoneAttachmentView workDoneAttachmentView = new WorkDoneAttachmentView();

                workDoneAttachmentView.ConvertFrom(workDoneAttachment, workDoneAttachmentView);

                //WorkDones ddl
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                workDoneAttachmentView.WorkDones = new SelectList(workDonesRepository.GetValid().OrderBy("Description ASC").ToList(), "WorkDonePK", "Description");

                return View(workDoneAttachmentView);
            }
            else
            {
                return RedirectToAction("Index", "WorkDoneAttachment");
            }
        }

        [PITAuthorize(Roles = "edit")]
        [HttpPost]
        public ActionResult Edit(WorkDoneAttachmentView workDoneAttachmentView, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                string sessionToken = Audit.GenerateNewSessionToken();

                int workDoneFK = (int)workDoneAttachmentView.WorkDoneFK;

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

                return RedirectToAction("Index", "WorkDoneAttachment");
            }
            else
            {
                //WorkDones ddl
                IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
                workDoneAttachmentView.WorkDones = new SelectList(workDonesRepository.GetValid().OrderBy("Description ASC").ToList(), "WorkDonePK", "Description");

                return View(workDoneAttachmentView);
            }
        }

        #endregion

        #region Delete WorkDone
        [PITAuthorize(Roles = "delete")]
        public ActionResult Delete(int? workDoneAttachmentPK)
        {
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
            if (workDoneAttachmentPK != null)
            {
                WorkDoneAttachment workDoneAttachment = workDoneAttachmentsRepository.GetWorkDoneAttachmentByPK((int)workDoneAttachmentPK);

                workDoneAttachment.Deleted = true;

                workDoneAttachmentsRepository.SaveChanges();
            }

            if (Request.IsAjaxRequest()) {
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

        }

        #endregion
    }
}
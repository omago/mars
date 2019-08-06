using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;

namespace Mateus.Model.BussinesLogic.Views.WorkDoneAttachmentModel
{
    public class WorkDoneAttachmentView
    {
        public int WorkDoneAttachmentPK { get; set; }

        [Required(ErrorMessage = "Izvršeni posao je obavezan.")]
        public int? WorkDoneFK { get; set; }
        public int? AttachmentFK { get; set; }
        
        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> WorkDones { get; set; }

        public string AttachmentName { get; set; }
        public string AttachmentFilename { get; set; }
        public string AttachmentExtension { get; set; }
        public string AttachmentContentType { get; set; }

        public string WorkDoneDescription { get; set; }

        public void ConvertFrom(WorkDoneAttachment workDoneAttachment, WorkDoneAttachmentView workDoneAttachmentView) 
        {
            workDoneAttachmentView.WorkDoneAttachmentPK = workDoneAttachment.WorkDoneAttachmentPK;
            workDoneAttachmentView.WorkDoneFK = workDoneAttachment.WorkDoneFK;
            workDoneAttachmentView.AttachmentFK = workDoneAttachment.AttachmentFK;
            workDoneAttachmentView.Deleted = workDoneAttachment.Deleted;
        }

        public void ConvertTo(WorkDoneAttachmentView workDoneAttachmentView, WorkDoneAttachment workDoneAttachment) 
        {
            workDoneAttachment.WorkDoneAttachmentPK = workDoneAttachmentView.WorkDoneAttachmentPK;
            workDoneAttachment.WorkDoneFK = workDoneAttachmentView.WorkDoneFK;
            workDoneAttachment.AttachmentFK = workDoneAttachmentView.AttachmentFK;
            workDoneAttachment.Deleted = workDoneAttachmentView.Deleted;
        }

        public static IQueryable<WorkDoneAttachmentView> GetWorkDoneAttachmentView(IQueryable<WorkDoneAttachment> workDoneAttachmentTable, IQueryable<Attachment> attachmentTable, IQueryable<WorkDone> workDoneTable) 
        {
            IQueryable<WorkDoneAttachmentView> WorkDoneAttachmentViewList = (from t1 in workDoneAttachmentTable
                                       join t2 in attachmentTable on t1.AttachmentFK equals t2.AttachmentPK
                                       join t3 in workDoneTable on t1.WorkDoneFK equals t3.WorkDonePK

                                       select new WorkDoneAttachmentView
                                       {
                                            WorkDoneAttachmentPK    = t1.WorkDoneAttachmentPK,
                                            WorkDoneFK              = t1.WorkDoneFK,
                                            WorkDoneDescription     = t3.Description,
                                            AttachmentFK            = t1.AttachmentFK,
                                            AttachmentName          = t2.Name,
                                            AttachmentFilename      = t2.Filename,
                                            AttachmentExtension     = t2.Extension,
                                            AttachmentContentType   = t2.ContentType
                                       }).AsQueryable<WorkDoneAttachmentView>();

            return WorkDoneAttachmentViewList;
        }
    }
}

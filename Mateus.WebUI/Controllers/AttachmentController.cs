using System.Web.Mvc;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Support;
using System.IO;

namespace Mateus.Controllers
{
    public class AttachmentController : Controller
    {
        #region Initalizers

        Mateus_wcEntities db = null;
        public AttachmentController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public FileStreamResult Download(int? attachmentPK)
        {
            if(attachmentPK != null) 
            {
                IAttachmentsRepository attachmentsRepository = new AttachmentsRepository(db);
                Attachment attachment = attachmentsRepository.GetAttachmentByPK((int)attachmentPK);

                string file = attachment.Filename + attachment.Extension;
                string name = attachment.Name;

                string filePath = Path.GetDirectoryName(Server.MapPath("~")) + "\\Attachments\\" + file;
                
                return File(new FileStream(filePath, FileMode.Open), attachment.ContentType, name);
            } 
            else 
            {
                return null;
            }
        }
    }
}
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Views.AttachmentModel
{
    public class AttachmentView
    {
        public int AttachmentPK { get; set; }

        public string Name { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }

        public bool? Deleted { get; set; }

        public void ConvertFrom(Attachment attachment, AttachmentView attachmentView) 
        {
            attachmentView.AttachmentPK = attachment.AttachmentPK;

            attachmentView.Name = attachment.Name;
            attachmentView.Filename = attachment.Filename;
            attachmentView.Extension = attachment.Extension;
            attachmentView.ContentType = attachment.ContentType;

            attachmentView.Deleted = attachment.Deleted;
        }

        public void ConvertTo(AttachmentView attachmentView, Attachment attachment) 
        {
            attachment.AttachmentPK = attachment.AttachmentPK;

            attachment.Name = attachmentView.Name;
            attachment.Filename = attachmentView.Filename;
            attachment.Extension = attachmentView.Extension;
            attachment.ContentType = attachmentView.ContentType;

            attachment.Deleted = attachmentView.Deleted;
        }
    }
}

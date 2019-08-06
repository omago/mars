using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AttachmentsRepository : Repository<Attachment>, IAttachmentsRepository
    {
        public AttachmentsRepository(ObjectContext context): base(context)
        {

        }

        public Attachment GetAttachmentByPK(int attachmentPK)
        {
            Attachment a = GetAll().Where(attachment => attachment.AttachmentPK == attachmentPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Attachment> GetValid()
        {
           return GetAll().Where(attachment => attachment.Deleted == false || attachment.Deleted == null);
        }
    }
}

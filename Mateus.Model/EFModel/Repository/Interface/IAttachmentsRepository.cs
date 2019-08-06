using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAttachmentsRepository : IRepository<Attachment>
    {
        Attachment GetAttachmentByPK(int attachmentPK);

        IQueryable<Attachment> GetValid();
    }
}

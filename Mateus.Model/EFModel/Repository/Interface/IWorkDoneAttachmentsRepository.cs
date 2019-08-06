using System.Linq;
using PITFramework.Repository;
using Mateus.Model.BussinesLogic.Views.WorkDoneModel;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IWorkDoneAttachmentsRepository : IRepository<WorkDoneAttachment>
    {
        WorkDoneAttachment GetWorkDoneAttachmentByPK(int workDoneAttachmentPK);

        IQueryable<WorkDoneAttachment> GetValid();

        IQueryable<WorkDoneView> GetWorkDoneAttachmentCountByWorkDone(int? workDoneFK);
    }
}

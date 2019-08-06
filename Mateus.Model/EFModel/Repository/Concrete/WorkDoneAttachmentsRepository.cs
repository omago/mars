using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;
using Mateus.Model.BussinesLogic.Views.WorkDoneModel;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class WorkDoneAttachmentsRepository : Repository<WorkDoneAttachment>, IWorkDoneAttachmentsRepository
    {
        IWorkDonesRepository workDonesRepository;
        public WorkDoneAttachmentsRepository(ObjectContext context): base(context)
        {
            workDonesRepository = new WorkDonesRepository(context);
        }

        public WorkDoneAttachment GetWorkDoneAttachmentByPK(int workDoneAttachmentPK)
        {
            WorkDoneAttachment a = GetValid().Where(workDoneAttachment => workDoneAttachment.WorkDoneAttachmentPK == workDoneAttachmentPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<WorkDoneAttachment> GetValid()
        {
            return GetAll().Where(workDoneAttachment => workDoneAttachment.Deleted == false || workDoneAttachment.Deleted == null);
        }

        public IQueryable<WorkDoneView> GetWorkDoneAttachmentCountByWorkDone(int? workDoneFK)
        { 
            var workDoneTable = workDonesRepository.GetValid();
            var workDoneAttachmentTable = GetValid();

            IQueryable<WorkDoneView> workDoneViewList = (from wd in workDoneTable
                                                           select new WorkDoneView
                                                           {
                                                                WorkDonePK                  = wd.WorkDonePK,
                                                                WorkDoneAttachmentsCount    = workDoneAttachmentTable.Where(c => c.WorkDoneFK == wd.WorkDonePK).Count(),
                                                           }).AsQueryable<WorkDoneView>();

            if(workDoneFK != null) workDoneViewList = workDoneViewList.Where(wd => wd.WorkDonePK == workDoneFK);

            return workDoneViewList;
        }

    }
}

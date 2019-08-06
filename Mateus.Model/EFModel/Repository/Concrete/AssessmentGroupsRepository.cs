using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AssessmentGroupsRepository : Repository<AssessmentGroup>, IAssessmentGroupsRepository
    {
        public AssessmentGroupsRepository(ObjectContext context)
            : base(context)
        {

        }

        public AssessmentGroup GetAssessmentGroupByPK(int assessmentGroupPK)
        {
            AssessmentGroup a = GetAll().Where(assessmentGroup => assessmentGroup.AssessmentGroupPK == assessmentGroupPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AssessmentGroup> GetValid()
        {
            return GetAll().Where(assessmentGroup => assessmentGroup.Deleted == false || assessmentGroup.Deleted == null);
        }

        public IQueryable<AssessmentGroup> GetAssessmentGroupsByType(int assessmentTypeFK)
        {
            return GetValid().Where(assessmentGroup => assessmentGroup.AssessmentTypeFK == assessmentTypeFK);
        }
    }
}

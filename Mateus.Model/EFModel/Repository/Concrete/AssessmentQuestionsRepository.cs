using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AssessmentQuestionsRepository : Repository<AssessmentQuestion>, IAssessmentQuestionsRepository
    {
        IAssessmentGroupsRepository assessmentGroupsRepository;
        public AssessmentQuestionsRepository(ObjectContext context)
            : base(context)
        {
            assessmentGroupsRepository = new AssessmentGroupsRepository(context);
        }

        public AssessmentQuestion GetAssessmentQuestionByPK(int assessmentQuestionPK)
        {
            AssessmentQuestion a = GetAll().Where(assessmentQuestion => assessmentQuestion.AssessmentQuestionPK == assessmentQuestionPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AssessmentQuestion> GetValid()
        {
            return GetAll().Where(assessmentQuestion => assessmentQuestion.Deleted == false || assessmentQuestion.Deleted == null);
        }

        public IQueryable<AssessmentQuestion> GetAssessmentQuestionsByGroup(int assessmentGroupFK)
        {
            return GetValid().Where(asessmentQuestion => asessmentQuestion.AssessmentGroupFK == assessmentGroupFK);
        }
    }
}

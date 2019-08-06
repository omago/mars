using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AssessmentAnswersRepository : Repository<AssessmentAnswers>, IAssessmentAnswersRepository
    {
        public AssessmentAnswersRepository(ObjectContext context)
            : base(context)
        {

        }

        public AssessmentAnswers GetAssessmentAnswersByPK(int assessmentAnswersPK)
        {
            AssessmentAnswers a = GetAll().Where(assessmentAnswers => assessmentAnswers.AssessmentAnswerPK == assessmentAnswersPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AssessmentAnswers> GetValid()
        {
            return GetAll().Where(assessmentAnswers => assessmentAnswers.Deleted == false || assessmentAnswers.Deleted == null);
        }

        public IQueryable<AssessmentAnswers> GetAssessmentAnswersByAssessment(int assessmentPK)
        {
            return GetValid().Where(assessmentAnswers => assessmentAnswers.AssessmentFK == assessmentPK);
        }
    }
}

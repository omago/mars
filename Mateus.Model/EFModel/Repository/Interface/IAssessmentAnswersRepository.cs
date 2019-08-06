using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAssessmentAnswersRepository : IRepository<AssessmentAnswers>
    {
        AssessmentAnswers GetAssessmentAnswersByPK(int assessmentAnswersPK);

        IQueryable<AssessmentAnswers> GetValid();

        IQueryable<AssessmentAnswers> GetAssessmentAnswersByAssessment(int assessmentPK);
    }
}

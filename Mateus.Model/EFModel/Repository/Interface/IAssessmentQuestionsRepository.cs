using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAssessmentQuestionsRepository : IRepository<AssessmentQuestion>
    {
        AssessmentQuestion GetAssessmentQuestionByPK(int assessmentQuestionPK);

        IQueryable<AssessmentQuestion> GetValid();

        IQueryable<AssessmentQuestion> GetAssessmentQuestionsByGroup(int assessmentGroupFK);
    }
}

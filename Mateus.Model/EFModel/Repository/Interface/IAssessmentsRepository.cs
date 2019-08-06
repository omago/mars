using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAssessmentsRepository : IRepository<Assessment>
    {
        Assessment GetAssessmentByPK(int assessmentPK);

        IQueryable<Assessment> GetValid(); 

        IQueryable<Assessment> GetAssessmentByLegalEntity(int legalEntityFK);
    }
}

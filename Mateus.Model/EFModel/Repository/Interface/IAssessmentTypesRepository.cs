using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAssessmentTypesRepository : IRepository<AssessmentType>
    {
        AssessmentType GetAssessmentTypeByPK(int assessmentTypePK);

        IQueryable<AssessmentType> GetValid();
    }
}

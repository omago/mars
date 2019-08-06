using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface IAssessmentGroupsRepository : IRepository<AssessmentGroup>
    {
        AssessmentGroup GetAssessmentGroupByPK(int assessmentGroupPK);

        IQueryable<AssessmentGroup> GetValid();

        IQueryable<AssessmentGroup> GetAssessmentGroupsByType(int assessmentTypeFK);
    }
}

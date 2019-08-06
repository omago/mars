using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AssessmentTypesRepository : Repository<AssessmentType>, IAssessmentTypesRepository
    {
        public AssessmentTypesRepository(ObjectContext context)
            : base(context)
        {

        }

        public AssessmentType GetAssessmentTypeByPK(int assessmentTypePK)
        {
            AssessmentType a = GetAll().Where(assessmentType => assessmentType.AssessmentTypePK == assessmentTypePK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<AssessmentType> GetValid()
        {
            return GetAll().Where(assessmentType => assessmentType.Deleted == false || assessmentType.Deleted == null);
        }
    }
}

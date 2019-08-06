using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class AssessmentsRepository : Repository<Assessment>, IAssessmentsRepository
    {
        public AssessmentsRepository(ObjectContext context): base(context)
        {

        }

        public Assessment GetAssessmentByPK(int assessmentPK)
        {
            Assessment a = GetAll().Where(assessment => assessment.AssessmentPK == assessmentPK).First();

            if (a != null)
            {
                return a;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Assessment> GetValid()
        {
           return GetAll().Where(assessment => assessment.Deleted == false || assessment.Deleted == null);
        }

        public IQueryable<Assessment> GetAssessmentByLegalEntity(int legalEntityFK)
        {
            return GetValid().Where(assessment => assessment.LegalEntityFK == legalEntityFK);
        }
    }
}

using System.Linq;
using PITFramework.Repository;
using Mateus.Model.EFModel.Repository.Interface;
using System.Data.Objects;

namespace Mateus.Model.EFModel.Repository.Concrete
{
    public class RisksRepository : Repository<Risk>, IRisksRepository
    {
        public RisksRepository(ObjectContext context): base(context)
        {

        }

        public Risk GetRiskByPK(int riskPK)
        {
            var r = GetAll().Where(risk => risk.RiskPK == riskPK);

            if (r != null && r.Count() > 0)
            {
                return r.First();
            }
            else
            {
                return null;
            }
        }


        public IQueryable<Risk> GetValid()
        {
           return GetAll().Where(risk => risk.Deleted == false || risk.Deleted == null);
        }

        public Risk GetRiskByName(string name)
        {
            var r = GetValid().Where(risk => risk.Name == name);

            if (r != null && r.Count() > 0)
            {
                return r.First();
            }
            else
            {
                return null;
            }

        }
    }
}

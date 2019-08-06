using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Linq;

namespace PITFramework.Auditing
{
    public interface IAuditing
    {
        void AuditRecord(string sessionToken, out List<int> entityPKMaster, out List<int> entityPKDetail, out string auditingOperation);
    }
}

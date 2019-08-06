using System;
using System.Collections.Generic;
using System.Linq;
using PITFramework.Auditing;

namespace PITFramework.Support
{
    public static class AuditingHelper
    {
        public static string checkString(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            string value = String.Empty;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value = auditingDetail.NewValue;
            }

            return value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Mateus.Model.EFModel;

namespace Mateus.Model.BussinesLogic.Support.ExtensionMethods
{
    public static class AuditingHelper
    {
        public static string checkString(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            string value = null;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value = auditingDetail.NewValue;
            }

            return value;
        }
        
        public static DateTime? checkDate(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            DateTime? value = null;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value =  Convert.ToDateTime(auditingDetail.NewValue, System.Globalization.CultureInfo.GetCultureInfo("hr-HR"));
            }

            return value;
        }

        public static int? checkInteger(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            int? value = null;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value =  Convert.ToInt32(auditingDetail.NewValue);
            }

            return value;
        }

        public static bool? checkBoolean(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            bool? value = null;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value =  Convert.ToBoolean(auditingDetail.NewValue);
            }

            return value;
        }

        public static decimal? checkDecimal(this List<AuditingDetail> record, string keyName)
        {
            AuditingDetail auditingDetail = record.Where(r => r.ColumnName == keyName).FirstOrDefault();

            decimal? value = null;

            if(auditingDetail != null && auditingDetail.NewValue != "")
            {
               value =  Convert.ToDecimal(auditingDetail.NewValue);
            }

            return value;
        }
    }
}

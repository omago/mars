using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Text.RegularExpressions;
using PITFramework.Repository;
using System.Web;
using System.Configuration;
using PITFramework.Security;

namespace PITFramework.Auditing
{
    public class Audit : IAuditing
    {
        #region Properties

        AuditingMaster auditingMaster;
        List<AuditingDetail> auditDetails;

        ObjectContext _context;
        AuditingMateus auditingMateus;

        #endregion

        #region Constructors

        public Audit(ObjectContext context) 
        {
            auditingMateus = new AuditingMateus();
            _context = context;   
        }

        #endregion

        #region Audit operations

        public void AuditRecord(string sessionToken, out List<int> entityPKMaster, out List<int> entityPKDetail, out string auditingOperation) 
        {
            IEnumerable<ObjectStateEntry> changes = _context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified);
            auditDetails = new List<AuditingDetail>();

            string serverName = HttpContext.Current.Server.MachineName;
            string userFK = "";
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null) 
            {
                userFK = SecurityHelper.GetUserPKFromCookie().ToString();
            }

            // default out assignments
            entityPKDetail = new List<int>();
            entityPKMaster = new List<int>();
            auditingOperation = AuditingOperation.INSERT.ToString();
            int counter = 0;

            foreach (ObjectStateEntry objectStateEntry in changes)
            {
                if (!objectStateEntry.IsRelationship && 
                    objectStateEntry.Entity != null && 
                    !(objectStateEntry.Entity is AuditingDetail) && 
                    !(objectStateEntry.Entity is AuditingMaster))
                {
                    auditingMaster = new AuditingMaster() {
                        UserFK = userFK,
                        DBName = _context.DBName(),
                        TableName = objectStateEntry.EntitySet.Name,
                        Date = DateTime.Now,
                        Operation = getAuditOperation(objectStateEntry),
                        ServerName = serverName,
                        SessionToken = sessionToken
                    };

                    auditingOperation = getAuditOperation(objectStateEntry);

                    auditingMateus.AuditingMasters.AddObject(auditingMaster);

                    auditDetails = GetAuditEntries(objectStateEntry);

                    if(auditDetails.Count > 0)
                    {
                        auditingMateus.SaveChanges();

                        auditDetails.ForEach(ad => ad.AuditingMasterFK = auditingMaster.AuditingMasterPK);

                        AuditingDetail auditingDetailEntityPK;

                        auditingDetailEntityPK = auditDetails.First();

                        if (objectStateEntry.State == EntityState.Added)
                        {                      
                            auditingMateus.AuditingDetails.AddObject(auditingDetailEntityPK);
                            auditingMateus.SaveChanges();
                        }

                        int i = 0;
                        foreach (var audit in auditDetails)
                        {
                            if (++i == 1 && objectStateEntry.State == EntityState.Added) continue;
                            auditingMateus.AuditingDetails.AddObject(audit);
                        }
                        auditingMateus.SaveChanges();

                        entityPKDetail.Insert(counter, auditingDetailEntityPK.AuditingDetailPK);
                        entityPKMaster.Insert(counter, auditingMaster.AuditingMasterPK);

                        counter++;
                    }
                }
            }

            return;
        }

        public List<AuditingDetail> GetAuditEntries(ObjectStateEntry objectStateEntry)
        {
            List<AuditingDetail> auditDetails = new List<AuditingDetail>();
            var allProperties = getAllProperties(objectStateEntry);
            var modifiedProperties = objectStateEntry.GetModifiedProperties();
            var currentValues = objectStateEntry.CurrentValues;

            if (objectStateEntry.State == EntityState.Added)
            {
                foreach (var propertyName in allProperties)
                {
                    // Add whole entry to database
                    auditDetails.Add(new AuditingDetail()
                    {
                        ColumnName = propertyName.Key.ToString(),
                        OldValue = null,
                        NewValue = currentValues[propertyName.Key.ToString()].ToString()
                    });
                }
            }
            else if (objectStateEntry.State == EntityState.Modified)
            {
                var originalValues = objectStateEntry.OriginalValues;

                // If object was in Modified state and property is Deleted => records has been Deleted
                // Record all of its properties to Audit
                if (currentValues[modifiedProperties.First()].ToString() == ("Deleted").ToLower())
                {
                    foreach (var propertyName in allProperties)
                    {
                        auditDetails.Add(new AuditingDetail()
                        {
                            ColumnName = propertyName.Key.ToString(),
                            OldValue = originalValues[propertyName.Key.ToString()].ToString(),
                            NewValue = null
                        });
                    }
                }
                else
                {
                    // Loop modified properties and make audit
                    foreach (var propertyName in modifiedProperties)
                    {
                        // write Decision date everytime
                        if (!currentValues[propertyName].Equals(originalValues[propertyName]) || propertyName == "ChangeDate")
                        {
                            string oldValue = originalValues[propertyName].ToString();
                            string newValue = currentValues[propertyName].ToString();

                            if(oldValue.Length > 0 && oldValue != "DELETED-VALUE" && newValue == "")
                            {
                                newValue = "DELETED-VALUE";
                            }

                            // Add only modified properties
                            auditDetails.Add(new AuditingDetail()
                            {
                                ColumnName = propertyName,
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }
                    }
                }
            }

            return auditDetails;
        }

        public void UpdateInsertedEntityPKDetail(List<int> entityPKDetails, List<ObjectStateEntry> entityPKs, string operation)
        {
            List<int> tmpADKeys = new List<int>();
            foreach (var item in entityPKs) 
            {
                tmpADKeys.Add(Convert.ToInt32(item.EntityKey.EntityKeyValues[0].Value));
            }

            int i = 0;
            var ads = auditingMateus.AuditingDetails.Where(ad => entityPKDetails.Contains(ad.AuditingDetailPK));

            foreach (var item in ads) 
            {
                var amTmp = auditingMateus.AuditingMasters.Where(am => am.AuditingMasterPK == item.AuditingMasterFK).FirstOrDefault();
                if (operation == "INSERT")
                {
                    item.NewValue = tmpADKeys[i].ToString();
                }
                amTmp.RelatedEntityPK = tmpADKeys[i];
                i++;
            }

            auditingMateus.SaveChanges();
        }

        #endregion

        #region Helpers

        private Dictionary<string, object> getAllProperties(ObjectStateEntry entry)
        {
            var keyValues = new Dictionary<string, object>();
            var currentValues = entry.CurrentValues;
            for (int i = 0; i < currentValues.FieldCount; i++)
            {
                keyValues.Add(currentValues.GetName(i), currentValues.GetValue(i));
            }
            return keyValues;
        }

        public static String GenerateNewSessionToken()
        {
            return GenerateNewSessionToken(32);
        }

        public static String GenerateNewSessionToken(int n)
        {
            String ret = "";
            Random rnd = new Random();
            const String chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789";

            for (int i = 0; i < n; ++i)
            {
                ret += chars[rnd.Next(chars.Length)];
            }
            return ret;
        }

        private string getAuditOperation(ObjectStateEntry objectStateEntry)
        {
            EntityState entityState = objectStateEntry.State;
            string operation = "UNKNOWN";
            switch (entityState)
            {
                case EntityState.Added:
                    operation = AuditingOperation.INSERT.ToString();
                    break;
                case EntityState.Modified:
                    if (objectStateEntry.GetModifiedProperties().First().ToLower() == ("Deleted").ToLower())
                    {
                        operation = operation = AuditingOperation.DELETE.ToString();
                    }
                    else
                    {
                        operation = AuditingOperation.UPDATE.ToString();
                    }
                    break;
                case EntityState.Deleted:
                    operation = AuditingOperation.DELETE.ToString();
                    break;
            }

            return operation;
        }

        #endregion
    }
}

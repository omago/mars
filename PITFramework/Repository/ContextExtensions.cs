using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Objects;
using System.Configuration;

namespace PITFramework.Repository
{
    public static class ContextExtensions
    {
        public static string DBName(this ObjectContext context)
        {
            string connString = ConfigurationManager.ConnectionStrings[context.Connection.ConnectionString.Split('=')[1]].ConnectionString;

            Regex regex = new Regex(@"Initial Catalog=(?<DBName>[A-Za-z0-9][^;]*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(connString);

            if (match.Success)
            { 
                return match.Groups["DBName"].Value; 
            }

            throw new ArgumentException("Unable to find database name.");
        }

        public static string TableNameFor(this ObjectContext context, ObjectStateEntry objectStateEntry)
        {
            var generic = context.GetType().GetProperties().ToList().First(p => p.Name == objectStateEntry.EntityKey.EntitySetName);
            var objectset = generic.GetValue(context, null);

            var method = objectset.GetType().GetMethod("ToTraceString");
            var sql = (String)method.Invoke(objectset, null);

            var match = Regex.Match(sql, @"FROM\s+\[dbo\]\.\[(?<TableName>[^\]]+)\]", RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups["TableName"].Value;
            }

            throw new ArgumentException("Unable to find table name.");
        } 
    }
}

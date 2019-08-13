using System;
using System.Linq;

namespace PITFramework.Support.Grid
{
    public class LinkHelper
    {
        public static string GetSortLink(string columnTitle, string columnName)
        {
            string url = LinkHelper.getQueryStringArray(new string[] { "sortColumn", "sortOrder" });
            string queryStringColumnName = System.Web.HttpContext.Current.Request.QueryString["sortColumn"];
            string queryStringSortOrder = System.Web.HttpContext.Current.Request.QueryString["SortOrder"];

            string content = "";

            if (!String.IsNullOrWhiteSpace(queryStringColumnName) && queryStringColumnName.ToLower() == columnName.ToLower())
            {
                if (queryStringSortOrder.ToLower().Contains("desc"))
                {
                    content = "<a class=\"desc\" href=\"?" + url + "sortColumn=" + columnName + "&sortOrder=asc\">" + columnTitle + "</a>";
                }
                else
                {
                    content = "<a class=\"asc\" href=\"?" + url + "sortColumn=" + columnName + "&sortOrder=desc\">" + columnTitle + "</a>";
                }
            }
            else
            {
                content = "<a href=\"?" + url + "sortColumn=" + columnName + "&sortOrder=asc\">" + columnTitle + "</a>";
            }

            return content;
        }

        public static string getIsSorted(string columnName)
        {
            string content = "";

            string queryStringColumnName = System.Web.HttpContext.Current.Request.QueryString["sortColumn"];
            if (!String.IsNullOrWhiteSpace(queryStringColumnName) && queryStringColumnName.ToLower() == columnName.ToLower())
            {
                content = "sorted";
            }
            return content;
        }

        public static string getQueryStringArray(string[] escapeParams = null)
        {
            string url = "";
            string queryString = System.Web.HttpContext.Current.Request.QueryString.ToString();
            string[] queryStringArray = queryString.Split('&');

            if (queryStringArray.Count() > 1)
            {
                foreach (string queryStringParam in queryStringArray)
                {
                    string[] queryStringParamsArray = queryStringParam.Split('=');

                    if (escapeParams != null && escapeParams.Contains(queryStringParamsArray[0]) == false && queryStringParamsArray.Length == 2)
                    {
                        url += queryStringParamsArray[0] + "=" + queryStringParamsArray[1] + "&";
                    }
                }
            } 
            else if(!String.IsNullOrWhiteSpace(queryString))
            {
                string[] queryStringParamsArray = queryString.Split('=');

                if (escapeParams != null && escapeParams.Contains(queryStringParamsArray[0]) == false && queryStringParamsArray.Length == 2)
                {
                    url += queryStringParamsArray[0] + "=" + queryStringParamsArray[1] + "&";
                }
            }

            return url;
        }

        public static bool isChecked(string checkboxValue)
        {
            bool isChecked = false;

            if (checkboxValue != null && checkboxValue != "false")
            {
                isChecked = true;
            }

            return isChecked;
        }

        public static string  calculateTimeSpent(int? timeSpent)
        {
            int timeSpentHours = timeSpent == null ? 0 : (int)timeSpent / 60;
            int timeSpentMinutes = (timeSpent == null ? 0 : (int)timeSpent) - (timeSpentHours * 60);

            return timeSpentHours.ToString() + "h " + timeSpentMinutes.ToString() + "min";
        }
    }
}

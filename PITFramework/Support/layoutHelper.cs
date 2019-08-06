using System.Configuration;

namespace PITFramework.Support
{
    public static class LayoutHelper
    {
        public static string GetTheme() 
        {
            return ConfigurationManager.AppSettings["Theme"];
        }

        public static string GetMessage(string type, int primaryKey) 
        {
            string message = null;

            switch(type) 
            {
                case "INSERT":
                    message = "Zapis (ID=<" + primaryKey + ">) uspješno kreiran.";
                    break;

                case "UPDATE":
                    message = "Zapis (ID=<" + primaryKey + ">) uspješno ažuriran.";
                    break;

                case "DELETE":
                    message = "Zapis (ID=<" + primaryKey + ">) uspješno obrisan.";
                    break;
            }
            
            return message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PITFramework.Support
{
    public static class StringHelper
    {
        public static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }

        public static DateTime ConvertStringToDate(string dateString, string dateFormat = "d.M.yyyy. H:mm:ss") 
        {
            DateTime date = DateTime.ParseExact(dateString, dateFormat, null);

            return date;
        }

        public static string ConvertNonASCIICharacters(this string word) 
        {
            foreach(string key in UnicodeCharacters.Keys) 
            {
                word = word.Replace(key, UnicodeCharacters[key]);
            }

            return word;
        }

        private static readonly Dictionary<string, string> UnicodeCharacters = new Dictionary<string, string>
        {
            {"Ć", "&#262;"},
            {"ć", "&#263;"},
            {"Č", "&#268;"},
            {"č", "&#269;"},
            {"Đ", "&#272;"},
            {"đ", "&#273;"},
            {"Š", "&#352;"},
            {"š", "&#353;"},
            {"Ž", "&#381;"},
            {"ž", "&#382;"},
        };
    }
}

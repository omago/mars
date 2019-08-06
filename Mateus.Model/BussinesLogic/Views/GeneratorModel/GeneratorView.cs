using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mateus.Model.BussinesLogic.Views.GeneratorModel
{
    public class GeneratorView
    {
        #region Helpers

        public static List<SelectListItem> GenerateMinutes(){

            var numbers = (from p in Enumerable.Range(0, 61)
                select new SelectListItem
                {
                    Text = p.ToString(),
                    Value = p.ToString()
                });
            return numbers.ToList();
        }

        public static List<SelectListItem> GenerateHours(){

            var numbers = (from p in Enumerable.Range(0, 25)
                select new SelectListItem
                {
                    Text = p.ToString(),
                    Value = p.ToString()
                });
            return numbers.ToList();
        }

        public static List<SelectListItem> GenerateFinishedStauses(){

            var finishedStauses = new [] {
                                            new { Text = "Sve", Value = "all" },
                                            new { Text = "Izvršene", Value = "finished" },
                                            new { Text = "Obaveze", Value = "obligations" }
            };

            var statuses = (from p in finishedStauses
                select new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                });

            return statuses.ToList();
        }

        public static List<SelectListItem> GenerateGenders(){

            var gendersList = new [] {
                                            new { Text = "Muški", Value = "M" },
                                            new { Text = "Ženski", Value = "F" }
            };

            var genders = (from p in gendersList
                select new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                });

            return genders.ToList();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mateus.Support
{
    public static class MVCExtensionMethods
    {
        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value)
        {
            return enumerable.ToSelectList(value, value, false);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, int selectedValue)
        {
            return enumerable.ToSelectList(value, value, new string[] { selectedValue.ToString() }.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, string selectedValue)
        {
            return enumerable.ToSelectList(value, value, new string[] { selectedValue }.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, int[] selectedValues)
        {
            if (selectedValues == null)
            {
                throw new ArgumentNullException("selectedValues");
            }
            return enumerable.ToSelectList(value, value, selectedValues.ToList().ConvertAll<string>(i => i.ToString()));
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, IList<int> selectedValues)
        {
            return enumerable.ToSelectList(value, value, selectedValues.Cast<string>().ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, string[] selectedValues)
        {
            return enumerable.ToSelectList(value, value, selectedValues.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, IList<string> selectedValues)
        {
            return enumerable.ToSelectList(value, value, selectedValues);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and the data text field.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text)
        {
            return enumerable.ToSelectList(value, text, false);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, int selectedValue)
        {
            return enumerable.ToSelectList(value, text, new string[] { selectedValue.ToString() }.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, string selectedValue)
        {
            return enumerable.ToSelectList(value, text, new string[] { selectedValue }.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and the selected values.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, int[] selectedValues)
        {
            if (selectedValues == null)
            {
                throw new ArgumentNullException("selectedValues");
            }
            return enumerable.ToSelectList(value, text, selectedValues.ToList().ConvertAll<string>(i => i.ToString()));
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and the selected values.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, IList<int> selectedValues)
        {
            return enumerable.ToSelectList(value, text, selectedValues.Cast<string>().ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and the selected values.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, string[] selectedValues)
        {
            if (selectedValues == null)
            {
                throw new ArgumentNullException("selectedValues");
            }
            return enumerable.ToSelectList(value, text, selectedValues.ToList());
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and the selected values.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, IList<string> selectedValues)
        {
            foreach (var f in enumerable)
            {
                yield return new SelectListItem()
                {
                    Value = value(f),
                    Text = text(f),
                    Selected = selectedValues.Contains(value(f))
                };
            }
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectAll">Whether all values are selected.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, bool selectAll)
        {
            return enumerable.ToSelectList(value, value, selectAll);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified checkboxes for data value field, the data text field, and the selected values.
        /// </summary>
        /// <param name="enumerable">The checkboxes.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectAll">Whether all values are selected.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, bool selectAll)
        {
            foreach (var f in enumerable)
            {
                yield return new SelectListItem()
                {
                    Value = value(f),
                    Text = text(f),
                    Selected = selectAll
                };
            }
        }


        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, string selectedValue)
        {
            return items.ToSelectListItems(name, value, selectedValue, false);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, string selectedValue, bool includeNotApplicable, string notApplicableValue = "", string notApplicableText = "(Not Applicable)")
        {
            return items.ToSelectListItems(name, value, x => value(x) == selectedValue, includeNotApplicable, notApplicableValue, notApplicableText);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected)
        {
            return items.ToSelectListItems(name, value, isSelected, false);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected, bool includeNotApplicable, string notApplicableValue = "", string notApplicableText = "(Not Applicable)")
        {
            if (includeNotApplicable)
            {
                var returnItems = new List<SelectListItem>
                          {
                            new SelectListItem
                              {
                                Text = notApplicableText,
                                Value = notApplicableValue,
                                Selected = false
                              }
                          };

                if (items != null)
                {
                    returnItems.AddRange(items.Select(item => new SelectListItem
                    {
                        Text = name(item),
                        Value = value(item),
                        Selected = isSelected(item)
                    }));
                }
                return returnItems;
            }

            if (items == null)
                return new List<SelectListItem>();

            return items.Select(item => new SelectListItem
            {
                Text = name(item),
                Value = value(item),
                Selected = isSelected(item)
            });
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<int> selectedValues)
        {
            if (selectedValues == null)
                selectedValues = new List<int>();
            return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<long> selectedValues)
        {
            if (selectedValues == null)
                selectedValues = new List<long>();
            return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<double> selectedValues)
        {
            if (selectedValues == null)
                selectedValues = new List<double>();
            return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<decimal> selectedValues)
        {
            if (selectedValues == null)
                selectedValues = new List<decimal>();
            return items.ToMultiSelectListItems(name, value, selectedValues.Select(x => x.ToString()));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, IEnumerable<string> selectedValues)
        {
            if (items == null)
                return new List<SelectListItem>();

            if (selectedValues == null)
                selectedValues = new List<string>();

            return items.ToMultiSelectListItems(name, value, x => selectedValues.Contains(value(x)));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, string> value, Func<T, bool> isSelected)
        {
            if (items == null)
                return new List<SelectListItem>();

            return items.Select(item => new SelectListItem
            {
                Text = name(item),
                Value = value(item),
                Selected = isSelected(item)
            });
        }
    }
}
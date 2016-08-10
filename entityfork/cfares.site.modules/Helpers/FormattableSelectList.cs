
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.Web.UI;
using System.Globalization;

#endregion

namespace cfares.site.modules.Helpers
{
	public class FormattableSelectList : SelectList
	{
		private readonly string _formatString;

		public FormattableSelectList(IEnumerable items, string dataValueField, string dataTextField, string formatString)
			: base(items, dataValueField, dataTextField)
		{
			_formatString = formatString;
		}

		public FormattableSelectList(IEnumerable items, string dataValueField, string dataTextField, string formatString, object selectedValue)
			: base(items, dataValueField, dataTextField, selectedValue)
		{
			_formatString = formatString;
		}

		public override IEnumerator<SelectListItem> GetEnumerator()
		{
			return ((!String.IsNullOrEmpty(DataValueField)) ?
				GetListItemsWithValueField() :
				GetListItemsWithoutValueField()).GetEnumerator();
		}

		private IList<SelectListItem> GetListItemsWithValueField()
		{
			HashSet<string> selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (SelectedValues != null)
				selectedValues.UnionWith(from object value in SelectedValues select Convert.ToString(value, CultureInfo.CurrentCulture));

			IEnumerable<SelectListItem> listItems = from object item in Items
					let value = Eval(item, DataValueField, null)
					select new SelectListItem
					{
						Value = value,
						Text = Eval(item, DataTextField, _formatString),
						Selected = selectedValues.Contains(value)
					};

			return listItems.ToList();
		}

		private IList<SelectListItem> GetListItemsWithoutValueField()
		{
			HashSet<object> selectedValues = new HashSet<object>();
			if (SelectedValues != null)
				selectedValues.UnionWith(SelectedValues.Cast<object>());

			IEnumerable<SelectListItem> listItems = from object item in Items
					select new SelectListItem
					{
						Text = Eval(item, DataTextField, _formatString),
						Selected = selectedValues.Contains(item)
					};

			return listItems.ToList();
		}

		private static string Eval(object container, string expression, string formatString)
		{
			object value = container;
			if (!String.IsNullOrEmpty(expression))
				value = DataBinder.Eval(container, expression);

			string stringValue;
			if (formatString == null)
				stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
			else
				stringValue = String.Format(formatString, value);

			return stringValue;
		}
	}
}

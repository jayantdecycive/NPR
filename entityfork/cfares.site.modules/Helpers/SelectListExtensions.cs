
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

#endregion

namespace cfares.site.modules.Helpers
{
	public static class SelectListExtensions
	{
		//public static SelectList ToSelectList<T>( this IEnumerable<T> enumerable, 
		//	Func<T, int> text, Func<T, int> value, string dataValueField, string dataTextField, 
		//	string selectedValue )
		//{
		//	var items = enumerable.Select(f => new SelectListItem { 
		//		Text = text(f).ToString( CultureInfo.InvariantCulture ), 
		//		Value = value(f).ToString( CultureInfo.InvariantCulture ) } ).ToList();
        
		//	items.Insert(0, new SelectListItem { Text = string.Empty, Value = "-1" } );
        
		//	return new SelectList( items, dataValueField, dataTextField, selectedValue );
		//}

		public static SelectList ToSelectList<T>( this IEnumerable<T> enumerable, 
			Func<T, string> text, Func<T, string> value, string selectedValue )
		{
			return ToSelectList( enumerable, text, value, string.Empty, selectedValue );
		}

		public static SelectList ToSelectList<T>( this IEnumerable<T> enumerable, 
			Func<T, string> text, Func<T, string> value, string defaultOption, string selectedValue )
		{
			var items = enumerable.Select(f => new SelectListItem
					{ Text = text(f), Value = value(f) } ).ToList();
        
			items.Insert(0, new SelectListItem { Text = defaultOption, Value = "-1" } );
        
			return new SelectList( items, "Value", "Text", selectedValue );
		}
	}
}

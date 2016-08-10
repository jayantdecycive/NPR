
#region Imports

using System;
using System.Web.Mvc;

#endregion

namespace cfacore.shared.modules.helpers
{
	public static class FormValidationHelpers
	{
		public static void ValidateDateTimeOffset( this FormCollection collection, 
			ModelStateDictionary modelState, string key, string fieldName )
		{
			string v = collection[ key ];
			if( string.IsNullOrWhiteSpace( v ) ) { 
				modelState.AddModelStateError(key, fieldName);
				return;
			}

			DateTimeOffset offset;
			if( ! DateTimeOffset.TryParse( v, out offset ) ) modelState.AddModelStateError(key, fieldName);
		}
	}
}

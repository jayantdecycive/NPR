
#region Imports

using System;
using System.Web.Mvc;

#endregion

namespace cfacore.shared.modules.helpers
{
	public static class MvcExtensions
	{
		public static ViewResult AddModelStateError( this ViewResult result, 
			ModelStateDictionary modelState, string message )
		{
			AddModelStateError( modelState, string.Empty, message );
			return result;
		}

		public static ViewResult AddModelStateError( this ViewResult result, 
			ModelStateDictionary modelState, string key, string message )
		{
			AddModelStateError( modelState, key, message );
			return result;
		}

		public static ViewResult AddModelStateRequiredFieldError( this ViewResult result, 
			ModelStateDictionary modelState, string key, string fieldName )
		{
			AddModelStateError( modelState, key, fieldName );
			return result;
		}

		public static void AddModelStateRequiredFieldError( this ModelStateDictionary modelState, string key, string fieldName )
		{
			modelState.AddModelError( key, String.Format( "{0} is required", fieldName ) );
		}

		public static void AddModelStateError( this ModelStateDictionary modelState, 
			string key, string message )
		{
			modelState.AddModelError( key, message );
		}

		public static void AddModelStateError( this ModelStateDictionary modelState, string message )
		{
			modelState.AddModelError( string.Empty, message );
		}
	}
}

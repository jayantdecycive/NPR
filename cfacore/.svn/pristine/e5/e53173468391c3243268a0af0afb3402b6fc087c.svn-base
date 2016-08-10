
#region Imports

using System;

#endregion

namespace cfacore.shared.service.Helpers
{
	public static class Reflection
	{
		public static T ToType<T>( this string value )
		{
			try
			{
				return (T) Convert.ChangeType( value, typeof( T ) );
			}
			catch (Exception)
			{
				return default(T);
			}

		}
	}
}

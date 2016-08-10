
#region Imports

using System;
using System.Data.Objects;
using System.Reflection;

#endregion

namespace cfacore.shared.modules.helpers
{
	public static class EntityExtensions
	{
		public static object GetWrappedEntity( this object proxiedEntity )
		{
			// If model is proxied, unproxify it
			Type entityType = ObjectContext.GetObjectType( proxiedEntity.GetType() );
			return GenericCast( proxiedEntity, entityType );
		}

		public static object GenericCast( this object proxiedEntity, Type targetType )
		{
			MethodInfo castMethod = typeof( EntityExtensions ).GetMethod( "Cast" ).MakeGenericMethod( targetType );
			return castMethod.Invoke(null, new[] { proxiedEntity });		
		}

		public static T Cast<T>( object o )
        {
            return (T) o;
        }
	}
}

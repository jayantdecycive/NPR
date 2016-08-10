
#region Imports

using System;
using ServiceStack.CacheAccess.Providers;

#endregion

namespace cfacore.shared.domain._base
{
	public class ExtendedCacheClient : MemoryCacheClient, IExtendedCacheClient
	{
		public T Resolve<T>(string key, Func<T> cacheEvaluationExpression, CacheProfile cacheProfile = CacheProfile.StaticCache )
		{
			// Since cached values may be null, maintain separate lookup of caching records
			bool t = Get<bool>( key + '~' );
			if( t ) return Get<T>( key );

			// Cache miss, store and return
			T n = cacheEvaluationExpression.Invoke();
			if( cacheProfile == CacheProfile.StaticCache )
			{
				Set( key, n );
				Set( key + '~', true );
				return n;
			}

			Set( key, n, GetCacheTime( cacheProfile ) );
			Set( key + '~', true, GetCacheTime( cacheProfile ) );
			return n;
		}

		private TimeSpan GetCacheTime( CacheProfile cacheProfile )
		{
			switch( cacheProfile ) {
				case CacheProfile.NearRealtime: return TimeSpan.FromMinutes( 1 ); // 1 minute
				case CacheProfile.UpdatedEveryFewMinutes: return TimeSpan.FromMinutes( 5 ); // 5 minutes
				case CacheProfile.PeriodicallyRefreshed: return TimeSpan.FromMinutes( 240 ); // 4 hours
				default: return TimeSpan.FromDays( 365 ); // Default cache period of 1 year
			}
		}

	}
}

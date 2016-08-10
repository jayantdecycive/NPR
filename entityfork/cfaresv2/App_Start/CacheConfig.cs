using System;
//using CacheStack;
using cfares.site.modules.com.application;

namespace cfaresv2
{
	public static class CacheConfig
    {
		public static void Initialize()
		{
			//CacheStackSettings.CacheClient = AppContext.Cache;
			
			//// All of our routes are unique and not shared, so we can use the route name instead of reflection to get a unique cache key
			//CacheStackSettings.UseRouteNameForCacheKey = true;

			//CacheStackSettings.CacheProfileDurations = profile => {
			//	switch ((CacheProfile)profile) {
			//		case CacheProfile.NearRealtime: return TimeSpan.FromMinutes( 1 ); // 1 minute
			//		case CacheProfile.PeriodicallyRefreshed: return TimeSpan.FromMinutes( 240 ); // 4 hours
			//		default: return TimeSpan.FromDays( 365 ); // Default cache period of 1 year
			//	}
			//};

			//// Share same objects between different cache keys
			//CacheStackSettings.CacheKeysForObject.Add(typeof(User), item => {
			//	var userItem = item as User;
			//	var keys = new List<string>
			//		{
			//			CacheKeys.Users.ById(userItem.Id), 
			//			CacheKeys.Users.ByUsername(userItem.Username)
			//		};
			//	return keys;
			//});
		}    
	}
}

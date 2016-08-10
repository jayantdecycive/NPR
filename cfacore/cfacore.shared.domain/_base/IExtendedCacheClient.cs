using System;
using ServiceStack.CacheAccess;

namespace cfacore.shared.domain._base
{
	public interface IExtendedCacheClient : ICacheClient
	{
		T Resolve<T>( string key, Func<T> cacheEvaluationExpression, CacheProfile cacheProfile = CacheProfile.StaticCache );
	}
}

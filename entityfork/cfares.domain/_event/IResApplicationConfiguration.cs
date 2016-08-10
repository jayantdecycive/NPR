
using System.Data.Entity;
using Ninject;

namespace cfares.domain._event
{
    public interface IResApplicationConfiguration
    {
        IKernel GetKernel(ReservationType type,IKernel k=null);
	    DbContext DbContext { get; }

		bool EnablePerformanceOptimizations { get; set; }
    }
}

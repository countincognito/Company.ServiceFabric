using Castle.DynamicProxy;

namespace Company.Utility.Logging.Serilog
{
    public class AsyncTrackingInterceptor
        : ProcessingAsyncInterceptor<object>
    {
        protected override object StartingInvocation(IInvocation invocation)
        {
            TrackingContext.NewCurrentIfEmpty();
            return base.StartingInvocation(invocation);
        }
    }
}

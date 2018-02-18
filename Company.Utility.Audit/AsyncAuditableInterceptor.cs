using Castle.DynamicProxy;

namespace Company.Utility.Audit
{
    public class AsyncAuditableInterceptor
        : ProcessingAsyncInterceptor<object>
    {
        protected override object StartingInvocation(IInvocation invocation)
        {
            AuditContext.NewCurrentIfEmpty();
            return base.StartingInvocation(invocation);
        }
    }
}

using Castle.DynamicProxy;

namespace IntegrationDemo.Interceptors
{
    internal class AsyncInterceptorAdaper<TAsyncInterceptor> : AsyncDeterminationInterceptor where TAsyncInterceptor : IAsyncInterceptor
    {
        TAsyncInterceptor _asyncInterceptor;

        public AsyncInterceptorAdaper(TAsyncInterceptor Interceptor) : base(Interceptor)
        {
            _asyncInterceptor = Interceptor;
        }

        public override void Intercept(IInvocation invocation)
        {
            _asyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}

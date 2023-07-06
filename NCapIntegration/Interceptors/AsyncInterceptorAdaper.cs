using Castle.DynamicProxy;

namespace NCapIntegration.Interceptors
{
    public class AsyncInterceptorAdaper<TAsyncInterceptor> : AsyncDeterminationInterceptor where TAsyncInterceptor : IAsyncInterceptor
    {
        TAsyncInterceptor _asyncInterceptor;

        public AsyncInterceptorAdaper(TAsyncInterceptor Interceptor) : base(Interceptor)
        {
            _asyncInterceptor = Interceptor;
        }
    }
}

using Castle.DynamicProxy;
using NCapIntegration.Interceptors.Attributes;
using NCapIntegration.Persistence.Uow;
using System.Reflection;

namespace NCapIntegration.Attributes
{
    public class UowAsyncInterceptor : IAsyncInterceptor
    {
        private readonly IUnitOfWork _unitOfWork;

        public UowAsyncInterceptor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //异步方法
        public void InterceptAsynchronous(IInvocation invocation)
        {
            var attribute = GetUowAttribute(invocation);
            if (attribute == null)
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                invocation.ReturnValue = task;
            }
            else
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation, attribute);
            }
        }

        public async Task InternalInterceptAsynchronous(IInvocation invocation, UowAttribute uowAttribute)
        {
            using (_unitOfWork)
            {
                try
                {
                    _unitOfWork.BeginTransaction(distributed: uowAttribute.Distribute);
                    invocation.Proceed();
                    var task = (Task)invocation.ReturnValue;
                    await task;

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
        }

        //异步方法带返回值
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var attribute = GetUowAttribute(invocation);
            if (attribute == null)
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                invocation.ReturnValue = task;
            }
            else
            {
                invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation, attribute);
            }
        }


        public async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation, UowAttribute uowAttribute)
        {
            TResult result;
            using (_unitOfWork)
            {
                try
                {
                    _unitOfWork.BeginTransaction(distributed: uowAttribute.Distribute);
                    invocation.Proceed();
                    var task = (Task<TResult>)invocation.ReturnValue;
                    result = await task;

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }

            return result;
        }

        //同步方法
        public void InterceptSynchronous(IInvocation invocation)
        {
            var attribute = GetUowAttribute(invocation);
            if (attribute == null)
                invocation.Proceed();
            else
                InternalInterceptSynchronous(invocation, attribute);
        }

        public void InternalInterceptSynchronous(IInvocation invocation, UowAttribute uowAttribute)
        {
            using (_unitOfWork)
            {
                try
                {
                    _unitOfWork.BeginTransaction(distributed: uowAttribute.Distribute);
                    invocation.Proceed();
                    _unitOfWork.CommitAsync().GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync().GetAwaiter().GetResult();
                    throw;
                }
            }
        }

        private UowAttribute GetUowAttribute(IInvocation invocation)
        {
            var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
            var attribute = methodInfo.GetCustomAttribute<UowAttribute>();

            return attribute;
        }
    }
}

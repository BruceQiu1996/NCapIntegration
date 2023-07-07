using Castle.DynamicProxy;
using NCapIntegration.Entities;
using NCapIntegration.Interceptors.Attributes;
using NCapIntegration.Pipeline;
using System.Reflection;

namespace NCapIntegration.Interceptors
{
    public class OperateLogAsyncInterceptor : IAsyncInterceptor
    {
        private readonly OperateLoggerChannel _loggerChannel;

        public OperateLogAsyncInterceptor(OperateLoggerChannel loggerChannel)
        {
            _loggerChannel = loggerChannel;
        }

        //异步方法
        public void InterceptAsynchronous(IInvocation invocation)
        {
            var attribute = GetOperateLogAttribute(invocation);
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

        public async Task InternalInterceptAsynchronous(IInvocation invocation, OperateLogAttribute OperateLogAttribute)
        {
            var log = CreateLog(invocation);
            try
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                await task;
                log.FinishTime = DateTime.Now;
                log.Result = "success";
            }
            catch (Exception)
            {
                log.FinishTime = DateTime.Now;
                log.Result = "faild";

                throw;
            }
            finally
            {
                await _loggerChannel.WriteLogAsync(log);
            }
        }

        //异步方法带返回值
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var attribute = GetOperateLogAttribute(invocation);
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


        public async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation, OperateLogAttribute OperateLogAttribute)
        {
            var log = CreateLog(invocation);
            TResult result;
            try
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                result = await task;

                log.FinishTime = DateTime.Now;
                log.Result = "success";
            }
            catch (Exception)
            {
                log.FinishTime = DateTime.Now;
                log.Result = "faild";

                throw;
            }
            finally
            {
                await _loggerChannel.WriteLogAsync(log);
            }

            return result;
        }

        //同步方法
        public void InterceptSynchronous(IInvocation invocation)
        {
            var attribute = GetOperateLogAttribute(invocation);
            if (attribute == null)
                invocation.Proceed();
            else
                InternalInterceptSynchronous(invocation, attribute);
        }

        public void InternalInterceptSynchronous(IInvocation invocation, OperateLogAttribute OperateLogAttribute)
        {
            var log = CreateLog(invocation);
            try
            {
                invocation.Proceed();
                log.FinishTime = DateTime.Now;
                log.Result = "success";
            }
            catch (Exception)
            {
                log.FinishTime = DateTime.Now;
                log.Result = "faild";

                throw;
            }
            finally
            {
                _loggerChannel.WriteLogAsync(log).GetAwaiter().GetResult();
            }
        }

        private OperateLog CreateLog(IInvocation invocation)
        {
            return new OperateLog()
            {
                Author = "bruce",
                CreateTime = DateTime.Now,
                Module = invocation.TargetType.FullName,
                Topic = invocation.Method.Name
            };
        }

        private OperateLogAttribute GetOperateLogAttribute(IInvocation invocation)
        {
            var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
            var attribute = methodInfo.GetCustomAttribute<OperateLogAttribute>();

            return attribute;
        }
    }
}

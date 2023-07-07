namespace NCapIntegration.Interceptors.Attributes
{
    /// <summary>
    /// 记录操作日志标记
    /// Inherited = true ,作用到派生类上
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class OperateLogAttribute : Attribute
    {
    }
}

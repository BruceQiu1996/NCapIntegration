namespace NCapIntegration.Interceptors.Attributes
{
    /// <summary>
    /// 工作单元标记
    /// Inherited = true ,作用到派生类上
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class UowAttribute : Attribute
    {
        public bool Distribute { get; set; }
    }
}

namespace IntegrationDemo.Interceptors.Attributes
{
    public class UowAttribute : Attribute
    {
        public bool Distribute { get; set; }
        public UowAttribute() { }
    }
}

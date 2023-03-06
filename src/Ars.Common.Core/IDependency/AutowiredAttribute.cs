namespace Ars.Common.Core.IDependency
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {
        public string ServiceName { get; }

        public AutowiredAttribute(string serviceName = null)
        {
            ServiceName = serviceName;
        }
    }
}

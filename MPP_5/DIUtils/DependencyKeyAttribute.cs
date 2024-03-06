namespace MPP_5.DIUtils
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        public string Key { get; }

        public DependencyKeyAttribute(string key)
        {
            Key = key;
        }
    }
}

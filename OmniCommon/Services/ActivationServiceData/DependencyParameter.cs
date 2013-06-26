namespace OmniCommon.Services.ActivationServiceData
{
    public class DependencyParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DependencyParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public DependencyParameter() : this(string.Empty, null)
        {
        }
    }
}
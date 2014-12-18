namespace OmniUI.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class UseViewAttribute : Attribute
    {
        private readonly string _viewName;

        public UseViewAttribute(string viewName)
        {
            _viewName = viewName;
        }

        public UseViewAttribute(Type viewType)
        {
            _viewName = viewType.AssemblyQualifiedName;
            IsFullyQualifiedName = true;
        }

        public string ViewName
        {
            get { return _viewName; }
        }

        public bool IsFullyQualifiedName { get; set; }
    }
}
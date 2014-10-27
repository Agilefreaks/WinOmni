namespace OmniUI.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class UseViewAttribute : Attribute
    {
        private readonly string viewName;

        public UseViewAttribute(string viewName)
        {
            this.viewName = viewName;
        }

        public string ViewName
        {
            get { return viewName; }
        }

        public bool IsFullyQualifiedName { get; set; }
    }
}
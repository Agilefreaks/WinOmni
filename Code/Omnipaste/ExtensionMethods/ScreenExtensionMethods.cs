namespace Omnipaste.ExtensionMethods
{
    using Caliburn.Micro;
    using OmniUI.Workspace;

    public static class ScreenExtensionMethods
    {
        #region Public Methods and Operators

        public static TParent GetParentOfType<TParent>(this IChild child) where TParent : class
        {
            TParent parent = null;
            while (child != null && parent == null)
            {
                parent = child.Parent as TParent;
                child = child.Parent as IChild;
            }

            return parent;
        }

        public static IWorkspace GetParentWorkspace(this IChild child)
        {
            return child.GetParentOfType<IWorkspace>();
        }

        #endregion
    }
}
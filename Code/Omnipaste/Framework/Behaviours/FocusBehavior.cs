namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class FocusBehavior
    {
        #region Static Fields

        public static readonly DependencyProperty FocusFirstProperty = DependencyProperty.RegisterAttached(
            "FocusFirst",
            typeof(bool),
            typeof(Control),
            new PropertyMetadata(false, OnFocusFirstPropertyChanged));

        #endregion

        #region Public Methods and Operators

        public static bool GetFocusFirst(Control control)
        {
            return (bool)control.GetValue(FocusFirstProperty);
        }

        public static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        #endregion

        #region Methods

        private static void OnFocusFirstPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = obj as Control;
            if (control == null || !(args.NewValue is bool))
            {
                return;
            }

            if ((bool)args.NewValue)
            {
                control.Loaded += (sender, e) => control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        #endregion
    }
}
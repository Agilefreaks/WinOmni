namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class FocusBehavior : Behavior<Control>
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, null));

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnAssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= OnAssociatedObjectOnLoaded;
        }

        private void OnAssociatedObjectOnLoaded(object o, RoutedEventArgs a)
        {
            if (IsFocused) AssociatedObject.Focus();
        }
    }
}

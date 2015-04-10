namespace OmniUI.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class DisposableBehavior<T> : Behavior<T>
        where T : FrameworkElement
    {
        protected override sealed void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.Unloaded += AssociatedObjectUnloaded;
        }

        protected override sealed void OnDetaching()
        {
            TearDown();
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            SetUp();
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            TearDown();
        }

        protected virtual void SetUp()
        {
        }

        protected virtual void TearDown()
        {
        }
    }
}
namespace OmniUI.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class DisposableBehavior<T> : Behavior<T>
        where T : FrameworkElement
    {
        private bool _isCleanedUp;

        protected override sealed void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.Unloaded += AssociatedObjectUnloaded;
        }

        protected override sealed void OnDetaching()
        {
            SafeTearDown();
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            SetUp();
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            SafeTearDown();
        }

        private void SafeTearDown()
        {
            if (_isCleanedUp)
            {
                return;
            }

            _isCleanedUp = true;
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
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
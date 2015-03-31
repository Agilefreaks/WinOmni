namespace OmniUI.Framework.Behaviors
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

    public class ScrollBehavior : DisposableBehavior<ScrollViewer>
    {
        public enum ScrollDirectionType
        {
            Bottom,

            Top
        }

        public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register(
            "Collection",
            typeof(INotifyCollectionChanged),
            typeof(ScrollBehavior),
            new PropertyMetadata(default(INotifyCollectionChanged)));

        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.Register(
                "ScrollDirection",
                typeof(ScrollDirectionType),
                typeof(ScrollBehavior),
                new PropertyMetadata(default(ScrollDirectionType)));

        public INotifyCollectionChanged Collection
        {
            get
            {
                return (INotifyCollectionChanged)GetValue(CollectionProperty);
            }
            set
            {
                SetValue(CollectionProperty, value);
            }
        }

        public ScrollDirectionType ScrollDirection
        {
            get
            {
                return (ScrollDirectionType)GetValue(ScrollDirectionProperty);
            }
            set
            {
                SetValue(ScrollDirectionProperty, value);
            }
        }

        protected override void SetUp()
        {
            HookCollection(Collection);
            if (ScrollDirection == ScrollDirectionType.Bottom)
            {
                AssociatedObject.ScrollToBottom();
            }
            else
            {
                AssociatedObject.ScrollToTop();
            }
        }

        protected override void TearDown()
        {
            UnhookCollection(Collection);
        }

        private void HookCollection(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void UnhookCollection(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged -= OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (ScrollDirection)
            {
                case ScrollDirectionType.Top:
                    AssociatedObject.ScrollToTop();
                    break;
                case ScrollDirectionType.Bottom:
                    AssociatedObject.ScrollToBottom();
                    break;
            }
        }
    }
}
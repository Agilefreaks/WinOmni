namespace OmniUI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class VisibleOnTruncationBehavior : DisposableBehavior<Control>
    {
        #region Static Fields

        public static readonly DependencyProperty TextBlockControlProperty =
            DependencyProperty.Register(
                "TextBlockControl",
                typeof(TextBlock),
                typeof(VisibleOnTruncationBehavior),
                new UIPropertyMetadata(OnTargetTextBlockChanged));

        #endregion

        #region Public Properties

        public TextBlock TextBlockControl
        {
            get
            {
                return (TextBlock)GetValue(TextBlockControlProperty);
            }
            set
            {
                SetValue(TextBlockControlProperty, value);
            }
        }

        #endregion

        #region Methods

        private static void OnTargetTextBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (VisibleOnTruncationBehavior)d;
            behavior.UnhookTextBlock(e.OldValue as TextBlock);
            behavior.HookTextBlock(e.NewValue as TextBlock);
            behavior.UpdateVisibility();
        }

        protected override void SetUp()
        {
            HookTextBlock(TextBlockControl);
            UpdateVisibility();
        }

        protected override void TearDown()
        {
            UnhookTextBlock(TextBlockControl);
        }

        private void HookTextBlock(TextBlock newValue)
        {
            if (newValue != null)
            {
                newValue.Loaded += OnTextBlockLoaded;
                newValue.SizeChanged += OnTextBlockSizeChanged;
            }
        }

        private void UnhookTextBlock(TextBlock oldValue)
        {
            if (oldValue != null)
            {
                oldValue.Loaded -= OnTextBlockLoaded;
                oldValue.SizeChanged -= OnTextBlockSizeChanged;
            }
        }

        private void OnTextBlockLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateVisibility();
        }

        private void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            var textBlock = TextBlockControl;
            AssociatedObject.Visibility = textBlock != null && IsTextBlockTruncated(textBlock)
                                              ? Visibility.Visible
                                              : Visibility.Hidden;
        }

        private bool IsTextBlockTruncated(TextBlock textBlock)
        {
            var result = false;
            if (textBlock != null)
            {
                textBlock.Measure(new Size(textBlock.ActualWidth, Double.PositiveInfinity));
                result = textBlock.DesiredSize.Height > textBlock.ActualHeight;
            }

            return result;
        }

        #endregion
    }
}
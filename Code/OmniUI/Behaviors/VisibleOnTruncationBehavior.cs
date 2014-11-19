namespace OmniUI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class VisibleOnTruncationBehavior : Behavior<Control>
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
                OnTextBlockChanging(TextBlockControl, value);
                SetValue(TextBlockControlProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool IsTextBlockTruncated(TextBlock textBlock)
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

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateVisibility();
        }

        protected override void OnDetaching()
        {
            var textBlockControl = TextBlockControl;
            if (textBlockControl != null)
            {
                textBlockControl.Loaded -= OnTextBlockLoaded;
                textBlockControl.SizeChanged -= OnTextBlockSizeChanged;
            }
        }

        private static void OnTargetTextBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var visibleOnTruncationBehavior = (VisibleOnTruncationBehavior)d;
            visibleOnTruncationBehavior.TextBlockControl = e.NewValue as TextBlock;
        }

        private void OnTextBlockChanging(TextBlock oldValue, TextBlock newValue)
        {
            if (oldValue != null)
            {
                oldValue.Loaded -= OnTextBlockLoaded;
                oldValue.SizeChanged -= OnTextBlockSizeChanged;
            }

            if (newValue != null)
            {
                newValue.Loaded += OnTextBlockLoaded;
                newValue.SizeChanged += OnTextBlockSizeChanged;
            }

            UpdateVisibility();
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

        #endregion
    }
}
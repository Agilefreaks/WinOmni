namespace Omnipaste.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name = "PART_ExpandButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(Border))]
    [TemplatePart(Name = "PART_CollapseButton", Type = typeof(Button))]
    public class ExpandableTextBlock : Control
    {
        #region Static Fields

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded",
            typeof(bool),
            typeof(ExpandableTextBlock),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty MaxCollapsedHeightProperty =
            DependencyProperty.Register(
                "MaxCollapsedHeight",
                typeof(double),
                typeof(ExpandableTextBlock),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ShowSeparatorProperty = DependencyProperty.Register(
            "ShowSeparator",
            typeof(bool),
            typeof(ExpandableTextBlock),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ExpandableTextBlock),
            new PropertyMetadata(default(string)));

        #endregion

        #region Fields

        private Button _collapseButton;

        private Border _contentContainer;

        private Button _expandButton;

        #endregion

        #region Constructors and Destructors

        static ExpandableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExpandableTextBlock),
                new FrameworkPropertyMetadata(typeof(ExpandableTextBlock)));
        }

        #endregion

        #region Public Properties

        public double ContentHeight
        {
            get
            {
                return IsExpanded ? double.MaxValue : MaxCollapsedHeight;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
                _contentContainer.MaxHeight = value ? double.PositiveInfinity : MaxCollapsedHeight;
            }
        }

        public double MaxCollapsedHeight
        {
            get
            {
                return (double)GetValue(MaxCollapsedHeightProperty);
            }
            set
            {
                SetValue(MaxCollapsedHeightProperty, value);
            }
        }

        public bool ShowSeparator
        {
            get
            {
                return (bool)GetValue(ShowSeparatorProperty);
            }
            set
            {
                SetValue(ShowSeparatorProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DetachFromVisualTree();
            AttachToVisualTree();
        }

        #endregion

        #region Methods

        private void AttachToVisualTree()
        {
            _expandButton = EnforceInstance<Button>("PART_ExpandButton");
            _expandButton.Click += OnExpandButtonClick;

            _collapseButton = EnforceInstance<Button>("PART_CollapseButton");
            _collapseButton.Click += OnCollapseButtonClick;

            _contentContainer = EnforceInstance<Border>("PART_ContentContainer");
            IsExpanded = false;
        }

        private void DetachFromVisualTree()
        {
            if (_expandButton != null)
            {
                _expandButton.Click -= OnExpandButtonClick;
                _expandButton = null;
            }

            if (_expandButton != null)
            {
                _expandButton.Click -= OnCollapseButtonClick;
                _expandButton = null;
            }

            _contentContainer = null;
        }

        private T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            return GetTemplateChild(partName) as T ?? new T();
        }

        private void OnCollapseButtonClick(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
        }

        private void OnExpandButtonClick(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
        }

        #endregion
    }
}
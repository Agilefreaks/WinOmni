namespace Omnipaste.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class ExpandableTextBlock : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ExpandableTextBlock),
            new PropertyMetadata(default(string)));

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

        static ExpandableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExpandableTextBlock),
                new FrameworkPropertyMetadata(typeof(ExpandableTextBlock)));
        }
    }
}
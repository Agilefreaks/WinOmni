namespace Omnipaste.Framework
{
    using System.Windows;

    public static class CheckboxHelper
    {
        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.RegisterAttached(
            "CheckedIcon",
            typeof(FrameworkElement),
            typeof(CheckboxHelper),
            new FrameworkPropertyMetadata(default(FrameworkElement)));

        public static readonly DependencyProperty UncheckedIconProperty = DependencyProperty.RegisterAttached(
            "UncheckedIcon",
            typeof(FrameworkElement),
            typeof(CheckboxHelper),
            new FrameworkPropertyMetadata(default(FrameworkElement)));

        public static FrameworkElement GetCheckedIcon(DependencyObject dp)
        {
            return (FrameworkElement)dp.GetValue(CheckedIconProperty);
        }

        public static void SetCheckedIcon(DependencyObject dp, FrameworkElement value)
        {
            dp.SetValue(CheckedIconProperty, value);
        }

        public static FrameworkElement GetUncheckedIcon(DependencyObject dp)
        {
            return (FrameworkElement)dp.GetValue(UncheckedIconProperty);
        }

        public static void SetUncheckedIcon(DependencyObject dp, FrameworkElement value)
        {
            dp.SetValue(UncheckedIconProperty, value);
        }
    }
}

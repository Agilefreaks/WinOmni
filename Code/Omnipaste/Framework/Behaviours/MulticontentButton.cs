namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;

    public static class MulticontentButton
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(MulticontentButton),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static string GetText(Button control)
        {
            return (string)control.GetValue(TextProperty);
        }

        public static void SetText(Button control, string value)
        {
            control.SetValue(TextProperty, value);
        }
    }
}
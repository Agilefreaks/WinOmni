namespace OmniUI.Framework.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Threading;

    public class TextBoxFocusBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(TextBoxFocusBehavior),
            new PropertyMetadata(default(bool), OnFocusChanged));

        public bool IsFocused
        {
            get
            {
                return (bool)GetValue(IsFocusedProperty);
            }
            set
            {
                SetValue(IsFocusedProperty, value);
            }
        }

        private static void OnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Focus(((TextBoxFocusBehavior)d).AssociatedObject);
        }

        private static void Focus(TextBox textBox)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(
                    () =>
                        {
                            textBox.CaretIndex = 0;
                            textBox.Focus();
                        }));
        }
    }
}
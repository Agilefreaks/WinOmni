namespace OmniUI.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Threading;

    public class TextBoxAutoFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                AssociatedObject.CaretIndex = 0;
                AssociatedObject.Focus();
            }));
        }
    }
}

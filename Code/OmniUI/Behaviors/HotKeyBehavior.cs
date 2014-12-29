namespace OmniUI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    public class HotKeyBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ActionKeyProperty = DependencyProperty.Register(
            "ActionKey",
            typeof(ModifierKeys),
            typeof(HotKeyBehavior),
            new PropertyMetadata(default(ModifierKeys)));

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(Key),
            typeof(HotKeyBehavior),
            new PropertyMetadata(default(Key)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(HotKeyBehavior), new PropertyMetadata(default(ICommand)));

        public ModifierKeys ActionKey
        {
            get
            {
                return (ModifierKeys)GetValue(ActionKeyProperty);
            }
            set
            {
                SetValue(ActionKeyProperty, value);
            }
        }

        public Key Key
        {
            get
            {
                return (Key)GetValue(KeyProperty);
            }
            set
            {
                SetValue(KeyProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ActionKey && e.Key == Key)
            {
                Command.Execute(null);
            }
        }
    }
}
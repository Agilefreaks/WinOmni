namespace OmniUI.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public abstract class HotKeyBehavior : DisposableBehavior<TextBox>
    {
        #region Static Fields

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            "Key",
            typeof(Key),
            typeof(HotKeyBehavior),
            new PropertyMetadata(default(Key)));

        public static readonly DependencyProperty ModifierKeyProperty = DependencyProperty.Register(
            "ModifierKey",
            typeof(ModifierKeys?),
            typeof(HotKeyBehavior),
            new PropertyMetadata(default(ModifierKeys?)));

        #endregion

        #region Public Properties

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

        public ModifierKeys? ModifierKey
        {
            get
            {
                return (ModifierKeys?)GetValue(ModifierKeyProperty);
            }
            set
            {
                SetValue(ModifierKeyProperty, value);
            }
        }

        #endregion

        #region Methods

        protected abstract void ExecuteAction();

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key && ModifierMatches())
            {
                e.Handled = true;
                ExecuteAction();
            }
        }

        protected override void SetUp()
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void TearDown()
        {
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private bool ModifierMatches()
        {
            return ((Keyboard.Modifiers == ModifierKeys.None && !ModifierKey.HasValue)
                    || Keyboard.Modifiers == ModifierKey);
        }

        #endregion
    }
}
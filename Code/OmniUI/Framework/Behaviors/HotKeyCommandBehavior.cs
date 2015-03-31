namespace OmniUI.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Input;

    public class HotKeyCommandBehavior : HotKeyBehavior
    {
        #region Static Fields

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(HotKeyCommandBehavior),
            new PropertyMetadata(default(ICommand)));

        #endregion

        #region Public Properties

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

        #endregion

        #region Methods

        protected override void ExecuteAction()
        {
            Command.Execute(null);
        }

        #endregion
    }
}
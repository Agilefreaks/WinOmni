namespace Omnipaste.Framework.Commands
{
    using System;
    using System.Windows.Input;

    public class Command : ICommand
    {
        #region Fields

        private readonly Action _action;

        private readonly Action<object> _parametrizedAction;

        private bool _canExecute;

        #endregion

        #region Constructors and Destructors

        public Command(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public Command(Action<object> parametrizedAction, bool canExecute = true)
        {
            _parametrizedAction = parametrizedAction;
            _canExecute = canExecute;
        }

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged;

        public event EventHandler<CommandEventArgs> Executed;

        public event EventHandler<CancelCommandEventArgs> Executing;

        #endregion

        #region Public Properties

        public bool CanExecute
        {
            get
            {
                return _canExecute;
            }
            set
            {
                if (_canExecute == value)
                {
                    return;
                }

                _canExecute = value;
                var canExecuteChanged = CanExecuteChanged;
                if (canExecuteChanged != null)
                {
                    canExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Execute(object parameter)
        {
            ExecuteCore(parameter);
        }

        #endregion

        #region Explicit Interface Methods

        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute;
        }

        #endregion

        #region Methods

        protected void InvokeAction(object param)
        {
            if (_action != null)
            {
                _action();
            }
            else if (_parametrizedAction != null)
            {
                _parametrizedAction(param);
            }
        }

        protected void InvokeExecuted(CommandEventArgs arguments)
        {
            var executed = Executed;
            if (executed != null)
            {
                executed(this, arguments);
            }
        }

        protected void InvokeExecuting(CancelCommandEventArgs arguments)
        {
            var executing = Executing;
            if (executing != null)
            {
                executing(this, arguments);
            }
        }

        private void ExecuteCore(object parameter)
        {
            var arguments = new CancelCommandEventArgs { Parameter = parameter, Cancel = false };
            InvokeExecuting(arguments);
            if (arguments.Cancel)
            {
                return;
            }

            InvokeAction(parameter);
            InvokeExecuted(new CommandEventArgs(parameter));
        }

        #endregion
    }
}
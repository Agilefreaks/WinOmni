namespace Omnipaste.Framework.Commands
{
    using System;

    public class CommandEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public CommandEventArgs()
        {
        }

        public CommandEventArgs(object parameter)
        {
            Parameter = parameter;
        }

        #endregion

        #region Public Properties

        public object Parameter { get; private set; }

        #endregion
    }
}
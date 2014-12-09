namespace Omnipaste.Framework.Commands
{
    public class CancelCommandEventArgs
    {
        #region Public Properties

        public bool Cancel { get; set; }

        public object Parameter { get; set; }

        #endregion
    }
}
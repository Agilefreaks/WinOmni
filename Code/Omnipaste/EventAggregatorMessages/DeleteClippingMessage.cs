namespace Omnipaste.EventAggregatorMessages
{
    public class DeleteClippingMessage
    {
        #region Fields

        private readonly string _clippingId;

        #endregion

        #region Constructors and Destructors

        public DeleteClippingMessage(string clippingId)
        {
            _clippingId = clippingId;
        }

        #endregion

        #region Public Properties

        public string ClippingId
        {
            get
            {
                return _clippingId;
            }
        }

        #endregion
    }
}
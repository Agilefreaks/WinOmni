namespace Omnipaste.EventAggregatorMessages
{
    using Omnipaste.Models;

    public class DeleteClippingMessage
    {
        #region Constructors and Destructors

        public DeleteClippingMessage(Activity backingModel)
        {
            Activity = backingModel;
        }

        #endregion

        #region Public Properties

        public Activity Activity { get; set; }

        #endregion
    }
}
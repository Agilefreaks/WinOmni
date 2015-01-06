namespace Omnipaste.ActivityDetails.Clipping
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ClippingDetailsViewModel : ActivityDetailsViewModel, IClippingDetailsViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public ClippingDetailsViewModel(
            IClippingDetailsHeaderViewModel headerViewModel,
            IClippingDetailsContentViewModel contentViewModel,
            IEventAggregator eventAggregator)
            : base(headerViewModel, contentViewModel)
        {
            _eventAggregator = eventAggregator;
        }

        protected override void OnDeactivate(bool close)
        {
            if (Model.MarkedForDeletion)
            {
                if (!close)
                {
                    var parentConductor = Parent as IConductor;
                    if (parentConductor != null)
                    {
                        parentConductor.DeactivateItem(this, true);
                    }
                }
                else
                {
                    _eventAggregator.PublishOnUIThread(new DeleteClippingMessage(Model.BackingModel));
                }
            }
            else
            {
                base.OnDeactivate(close);
            }
        }

    }
}
namespace Omnipaste.ActivityDetails.Message
{
    using OmniUI.Attributes;

    [UseView("Omnipaste.ActivityDetails.ActivityDetailsView", IsFullyQualifiedName = true)]
    public class MessageDetailsViewModel : ActivityDetailsViewModel, IMessageDetailsViewModel
    {
        #region Constructors and Destructors

        public MessageDetailsViewModel(
            IMessageDetailsHeaderViewModel headerViewModel,
            IMessageDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            HeaderViewModel.Activate();
            ContentViewModel.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            HeaderViewModel.Deactivate(close);
            ContentViewModel.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}
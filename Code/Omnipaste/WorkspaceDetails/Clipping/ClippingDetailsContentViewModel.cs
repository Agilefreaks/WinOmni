namespace Omnipaste.WorkspaceDetails.Clipping
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    public class ClippingDetailsContentViewModel : WorkspaceDetailsContentViewModel<ClippingPresenter>, IClippingDetailsContentViewModel
    {
        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        protected override void OnActivate()
        {
            if (Model != null && !Model.WasViewed)
            {
                Model.WasViewed = true;
                ClippingRepository.Save(Model.BackingModel).RunToCompletion();
                EventAggregator.PublishOnUIThread(new DismissNotification(Model.BackingModel.UniqueId));
            }
            
            base.OnActivate();
        }
    }
}
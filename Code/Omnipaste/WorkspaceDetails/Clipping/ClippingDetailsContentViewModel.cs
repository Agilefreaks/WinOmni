namespace Omnipaste.WorkspaceDetails.Clipping
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    public class ClippingDetailsContentViewModel : WorkspaceDetailsContentViewModel<ActivityPresenter>, IClippingDetailsContentViewModel
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
                ClippingRepository.Save(Model.BackingModel as ClippingModel).RunToCompletion();
                EventAggregator.PublishOnUIThread(new DismissNotification(Model.SourceId));
            }
            
            base.OnActivate();
        }
    }
}
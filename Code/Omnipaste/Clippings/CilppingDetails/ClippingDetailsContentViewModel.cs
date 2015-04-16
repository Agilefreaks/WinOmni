namespace Omnipaste.Clippings.CilppingDetails
{
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services.Repositories;
    using OmniUI.Details;

    public class ClippingDetailsContentViewModel : DetailsViewModelBase<ClippingModel>, IClippingDetailsContentViewModel
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
                ClippingRepository.Save(Model.BackingEntity).RunToCompletion();
                EventAggregator.PublishOnUIThread(new DismissNotification(Model.BackingEntity.UniqueId));
            }
            
            base.OnActivate();
        }
    }
}
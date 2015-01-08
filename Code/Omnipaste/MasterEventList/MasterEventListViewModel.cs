namespace Omnipaste.MasterEventList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterEventList.AllEventList;
    using Omnipaste.MasterEventList.IncomingCallEventList;
    using Omnipaste.MasterEventList.IncomingSmsEventList;
    using Omnipaste.Properties;
    using Omnipaste.SMSComposer;

    public sealed class MasterEventListViewModel : Conductor<IScreen>.Collection.AllActive, IMasterEventListViewModel
    {
        [Inject]
        public IModalSMSComposerViewModel SmsComposerViewModel { get; set; }

        [Inject]
        public IAllEventListViewModel AllEventListViewModel { get; set; }

        [Inject]
        public IIncomingCallEventListViewModel IncomingCallEventListViewModel { get; set; }

        [Inject]
        public IIncomingSmsEventListViewModel IncomingSmsEventListViewModel { get; set; }

        public MasterEventListViewModel()
        {
            DisplayName = Resources.MasterEventListDisplayName;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(AllEventListViewModel);
            ActivateItem(IncomingSmsEventListViewModel);
            ActivateItem(IncomingCallEventListViewModel);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(AllEventListViewModel, close);
            DeactivateItem(IncomingSmsEventListViewModel, close);
            DeactivateItem(IncomingCallEventListViewModel, close);
            base.OnDeactivate(close);
        }
    }
}
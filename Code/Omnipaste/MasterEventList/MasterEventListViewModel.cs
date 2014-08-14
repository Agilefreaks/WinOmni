namespace Omnipaste.MasterEventList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterEventList.AllEventList;
    using Omnipaste.MasterEventList.IncomingCallEventList;
    using Omnipaste.MasterEventList.IncomingSmsEventList;
    using Omnipaste.SendSms;

    public sealed class MasterEventListViewModel : Screen, IMasterEventListViewModel
    {
        [Inject]
        public ISendSmsViewModel SendSmsViewModel { get; set; }

        [Inject]
        public IAllEventListViewModel AllEventListViewModel { get; set; }

        [Inject]
        public IIncomingCallEventListViewModel IncomingCallEventListViewModel { get; set; }

        [Inject]
        public IIncomingSmsEventListViewModel IncomingSmsEventListViewModel { get; set; }

        public MasterEventListViewModel()
        {
            DisplayName = "Notifications";
        }
    }
}
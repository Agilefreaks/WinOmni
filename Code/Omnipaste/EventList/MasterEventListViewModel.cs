namespace Omnipaste.EventList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.SendSms;

    public sealed class MasterEventListViewModel : Screen, IMasterEventListViewModel
    {
        [Inject]
        public ISendSmsViewModel SendSmsViewModel { get; set; }

        public MasterEventListViewModel()
        {
            DisplayName = "Notifications";
        }
    }
}
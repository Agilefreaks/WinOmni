namespace Omnipaste.MasterEventList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterEventList.AllEventList;
    using Omnipaste.SendSms;

    public sealed class MasterEventListViewModel : Screen, IMasterEventListViewModel
    {
        [Inject]
        public ISendSmsViewModel SendSmsViewModel { get; set; }

        [Inject]
        public IAllEventListViewModel AllEventListViewModel { get; set; }

        public MasterEventListViewModel()
        {
            DisplayName = "Notifications";
        }
    }
}
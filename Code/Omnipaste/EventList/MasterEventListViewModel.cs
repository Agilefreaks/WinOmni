namespace Omnipaste.EventList
{
    using Caliburn.Micro;

    public sealed class MasterEventListViewModel : Screen, IMasterEventListViewModel
    {
        public MasterEventListViewModel()
        {
            DisplayName = "Notifications";
        }
    }
}
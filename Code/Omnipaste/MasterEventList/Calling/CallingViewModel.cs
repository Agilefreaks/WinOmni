namespace Omnipaste.MasterEventList.Calling
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Dialog;

    public class CallingViewModel : Screen, ICallingViewModel
    {
        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

    }
}
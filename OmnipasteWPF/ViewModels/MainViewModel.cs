namespace OmnipasteWPF.ViewModels
{
    using Cinch;
    using OmniCommon.Interfaces;

    public class MainViewModel : ViewModelBase
    {
        public IActivationService ActivationService { get; set; }

        public MainViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
            ActivationService = iocProvider.GetTypeFromContainer<IActivationService>();
        }

        public void StartActivationProcess()
        {
            ActivationService.Run();
        }

        protected override void OnWindowActivated()
        {
            base.OnWindowActivated();
            StartActivationProcess();
        }
    }
}
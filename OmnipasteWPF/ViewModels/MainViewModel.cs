namespace OmnipasteWPF.ViewModels
{
    using Cinch;
    using OmniCommon.Services;

    public class MainViewModel : ViewModelBase
    {
        public ActivationService ActivationService { get; set; }

        public MainViewModel()
        {
            ActivationService = new ActivationService();
        }
    }
}
namespace OmnipasteWPF.ViewModels
{
    using Cinch;

    public abstract class ViewModel : ViewModelBase
    {
        protected ViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }

        protected virtual void FetchServices()
        {
        }
    }
}
namespace OmnipasteWPF.ViewModels
{
    using Cinch;

    public abstract class ViewModelBase : Cinch.ViewModelBase
    {
        protected ViewModelBase(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }

        protected virtual void FetchServices()
        {
        }
    }
}
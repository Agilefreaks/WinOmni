namespace OmnipasteWPF.ViewModels
{
    using Cinch;

    public class GetTokenFromUserViewModel : ViewModelBase, IGetTokenFromUserViewModel
    {
        public string Token { get; private set; }

        public GetTokenFromUserViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }
    }
}
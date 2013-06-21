namespace OmnipasteWPF.ViewModels
{
    using System.ComponentModel;
    using Cinch;

    public class GetTokenFromUserViewModel : ViewModelBase, IGetTokenFromUserViewModel
    {
        private static readonly PropertyChangedEventArgs TokenChangeArgs =
            ObservableHelper.CreateArgs<GetTokenFromUserViewModel>(x => x.Token);

        private string _token;

        public string Token
        {
            get
            {
                return _token;
            }

            set
            {
                _token = value;
                NotifyPropertyChanged(TokenChangeArgs);
            }
        }

        public GetTokenFromUserViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }
    }
}
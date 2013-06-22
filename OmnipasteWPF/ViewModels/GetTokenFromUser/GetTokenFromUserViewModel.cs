namespace OmnipasteWPF.ViewModels.GetTokenFromUser
{
    using System.ComponentModel;
    using Cinch;
    using ViewModelBase = OmnipasteWPF.ViewModels.ViewModelBase;

    public class GetTokenFromUserViewModel : ViewModelBase, IGetTokenFromUserViewModel
    {
        private static readonly PropertyChangedEventArgs TokenChangeArgs =
            ObservableHelper.CreateArgs<GetTokenFromUserViewModel>(x => x.Token);

        private string _token;

        public string Token
        {
            get
            {
                return this._token;
            }

            set
            {
                this._token = value;
                this.NotifyPropertyChanged(TokenChangeArgs);
            }
        }

        public GetTokenFromUserViewModel(IIOCProvider iocProvider)
            : base(iocProvider)
        {
        }
    }
}
namespace Omnipaste.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class UserTokenViewModel : Screen, IUserTokenViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private TokenRequestResultMessage _tokenRequestResultMessage;

        public string Token { get; set; }

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Token";

            _eventAggregator = eventAggregator;
        }

        public void Ok()
        {
            _tokenRequestResultMessage = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, Token);
            TryClose();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _tokenRequestResultMessage = new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            
            _eventAggregator.Publish(_tokenRequestResultMessage);
        }
    }
}
namespace Omnipaste.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class UserTokenViewModel : Screen, IUserTokenViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public string Token { get; set; }

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Ok()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, Token));
        }

        public void Cancel()
        {
            this.Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled));
        }

        private void Publish(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _eventAggregator.Publish(tokenRequestResultMessage);
        }
    }
}
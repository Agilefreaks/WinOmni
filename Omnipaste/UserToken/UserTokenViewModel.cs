namespace Omnipaste.UserToken
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class UserTokenViewModel : Screen, IUserTokenViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public string ActivationCode { get; set; }

        public UserTokenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Ok()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful, ActivationCode));
        }

        public void Cancel()
        {
            Publish(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Canceled));
        }

        private void Publish(TokenRequestResultMessage tokenRequestResultMessage)
        {
            _eventAggregator.Publish(tokenRequestResultMessage);
        }
    }
}
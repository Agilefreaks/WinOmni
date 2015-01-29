namespace Omnipaste.Conversation
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Presenters;

    public class ConversationInfoViewModelFactory : IConversationInfoViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ConversationInfoViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IConversationInfoViewModel Create(ConversationPresenter presenter)
        {
            var viewModel = _serviceLocator.GetInstance<IConversationInfoViewModel>();
            viewModel.Model = presenter;

            return viewModel;
        }
    }
}
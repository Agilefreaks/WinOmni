namespace Omnipaste.Presenters
{
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public interface IConversationPresenter : IPresenter
    {
        ContactInfoPresenter ContactInfoPresenter { get; }

        SourceType Source { get; }

        string Content { get; }

        IConversationPresenter SetContactInfoPresenter(ContactInfoPresenter contactInfoPresenter);
    }
}
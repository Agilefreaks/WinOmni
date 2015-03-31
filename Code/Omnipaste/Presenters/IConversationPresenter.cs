namespace Omnipaste.Presenters
{
    using Omnipaste.Entities;
    using OmniUI.Presenters;

    public interface IConversationPresenter : IPresenter
    {
        ContactInfoPresenter ContactInfoPresenter { get; }

        SourceType Source { get; }

        string Content { get; }

        IConversationPresenter SetContactInfoPresenter(ContactInfoPresenter contactInfoPresenter);
    }
}
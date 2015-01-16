namespace Omnipaste.Event
{
    using System;
    using System.Threading.Tasks;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;

    public interface IEventViewModel : IDetailsViewModel<IConversationItem>
    {
        #region Public Properties

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        Task CallBack();

        void SendSms();

        #endregion
    }
}
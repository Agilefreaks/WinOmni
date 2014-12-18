namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class MessageDetailsViewModel :
        DetailsViewModelWithHeaderBase<IMessageDetailsHeaderViewModel, IMessageDetailsContentViewModel>,
        IMessageDetailsViewModel
    {
        #region Constructors and Destructors

        public MessageDetailsViewModel(
            IMessageDetailsHeaderViewModel headerViewModel,
            IMessageDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion
    }
}
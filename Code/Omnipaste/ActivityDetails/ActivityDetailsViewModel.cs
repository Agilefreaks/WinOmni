namespace Omnipaste.ActivityDetails
{
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView("OmniUI.Details.DetailsViewWithHeader", IsFullyQualifiedName = true)]
    public class ActivityDetailsViewModel :
        DetailsViewModelWithHeaderBase<IActivityDetailsHeaderViewModel, IActivityDetailsContentViewModel>,
        IActivityDetailsViewModel
    {
        #region Constructors and Destructors

        public ActivityDetailsViewModel(
            IActivityDetailsHeaderViewModel headerViewModel,
            IActivityDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion
    }
}
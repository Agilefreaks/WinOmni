namespace Omnipaste.ActivityDetails
{
    using Caliburn.Micro;

    public class ActivityDetailsViewModel : Screen, IActivityDetailsViewModel
    {
        #region Constructors and Destructors

        public ActivityDetailsViewModel(
            IActivityDetailsHeaderViewModel headerViewModel,
            IActivityDetailsContentViewModel contentViewModel)
        {
            HeaderViewModel = headerViewModel;
            ContentViewModel = contentViewModel;
        }

        #endregion

        #region Public Properties

        public IActivityDetailsContentViewModel ContentViewModel { get; private set; }

        public IActivityDetailsHeaderViewModel HeaderViewModel { get; private set; }

        #endregion
    }
}
namespace Omnipaste.Activity
{
    using Omnipaste.Activity.Models;
    using Omnipaste.Services;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        private readonly IUiRefreshService _uiRefreshService;

        public ActivityViewModelFactory(IUiRefreshService uiRefreshService)
        {
            _uiRefreshService = uiRefreshService;
        }

        #region Public Methods and Operators

        public IActivityViewModel Create(Models.Activity activity)
        {
            IActivityViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Call:
                case ActivityTypeEnum.Message:
                    result = new ContactRelatedActivityViewModel(_uiRefreshService);
                    break;
                default:
                    result = new ActivityViewModel(_uiRefreshService);
                    break;
            }

            result.Model = activity;

            return result;
        }

        #endregion
    }
}
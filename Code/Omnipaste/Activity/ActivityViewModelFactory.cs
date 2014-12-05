namespace Omnipaste.Activity
{
    using Omnipaste.Activity.Models;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        #region Public Methods and Operators

        public IActivityViewModel Create(Models.Activity activity)
        {
            IActivityViewModel result;
            switch (activity.Type)
            {
                case ActivityTypeEnum.Call:
                case ActivityTypeEnum.Message:
                    result = new ContactRelatedActivityViewModel();
                    break;
                default:
                    result = new ActivityViewModel();
                    break;
            }

            result.Model = activity;

            return result;
        }

        #endregion
    }
}
namespace Omnipaste.ActivityList
{
    using Ninject;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Models;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public IActivityViewModel Create(ActivityModel activityModel)
        {
            IActivityViewModel result;
            switch (activityModel.Type)
            {
                case ActivityTypeEnum.Call:
                case ActivityTypeEnum.Message:
                    result = Kernel.Get<IContactRelatedActivityViewModel>();
                    break;
                case ActivityTypeEnum.Version:
                    result = Kernel.Get<IVersionActivityViewModel>();
                    break;
                default:
                    result = Kernel.Get<IActivityViewModel>();
                    break;
            }

            result.Model = activityModel;

            return result;
        }

        #endregion
    }
}
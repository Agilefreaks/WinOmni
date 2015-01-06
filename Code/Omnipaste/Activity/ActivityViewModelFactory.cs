﻿namespace Omnipaste.Activity
{
    using Ninject;
    using Omnipaste.Models;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public IActivityViewModel Create(Models.Activity activity)
        {
            IActivityViewModel result;
            switch (activity.Type)
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

            result.Model = activity;

            return result;
        }

        #endregion
    }
}
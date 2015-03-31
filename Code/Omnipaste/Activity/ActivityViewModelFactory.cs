﻿namespace Omnipaste.Activity
{
    using Ninject;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    public class ActivityViewModelFactory : IActivityViewModelFactory
    {
        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public IActivityViewModel Create(ActivityPresenter activityPresenter)
        {
            IActivityViewModel result;
            switch (activityPresenter.Type)
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

            result.Model = activityPresenter;

            return result;
        }

        #endregion
    }
}
namespace Omnipaste.WorkspaceDetails
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.WorkspaceDetails.Clipping;
    using Omnipaste.WorkspaceDetails.Conversation;
    using Omnipaste.WorkspaceDetails.GroupMessage;
    using Omnipaste.WorkspaceDetails.Version;

    public class WorkspaceDetailsViewModelFactory : IWorkspaceDetailsViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public WorkspaceDetailsViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        #region IWorkspaceDetailsViewModelFactory Members

        public IWorkspaceDetailsViewModel Create(ActivityPresenter activityPresenter)
        {
            IWorkspaceDetailsViewModel result;
            switch (activityPresenter.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = Create(activityPresenter.BackingModel as ClippingModel);
                    break;
                case ActivityTypeEnum.Message:
                case ActivityTypeEnum.Call:
                    result = Create(activityPresenter.ContactInfo);
                    break;
                case ActivityTypeEnum.Version:
                    result = Create(activityPresenter.BackingModel as UpdateInfo);
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ContactInfo contactInfo)
        {
            var result = _serviceLocator.GetInstance<IConversationViewModel>();
            result.Model = new ContactInfoPresenter(contactInfo);

            return result;
        }

        public IWorkspaceDetailsViewModel Create(IEnumerable<ContactInfoPresenter> contactInfoPresenterList)
        {
            var result = _serviceLocator.GetInstance<IGroupMessageDetailsViewModel>();
            result.Recipients = (ObservableCollection<ContactInfoPresenter>)contactInfoPresenterList;

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ClippingModel clippingModel)
        {
            var result = _serviceLocator.GetInstance<IClippingDetailsViewModel>();
            result.Model = new ClippingPresenter(clippingModel);

            return result;
        }

        public IWorkspaceDetailsViewModel Create(UpdateInfo updateInfo)
        {
            var result = _serviceLocator.GetInstance<IVersionDetailsViewModel>();
            result.Model = new UpdateInfoPresenter(updateInfo);

            return result;
        }

        #endregion
    }
}
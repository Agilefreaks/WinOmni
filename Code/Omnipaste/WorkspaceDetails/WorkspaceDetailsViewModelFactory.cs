namespace Omnipaste.WorkspaceDetails
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.WorkspaceDetails.Clipping;
    using Omnipaste.WorkspaceDetails.Conversation;
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
                    result = Create(activityPresenter.BackingModel as ClippingEntity);
                    break;
                case ActivityTypeEnum.Message:
                case ActivityTypeEnum.Call:
                    result = Create(activityPresenter.ContactEntity);
                    break;
                case ActivityTypeEnum.Version:
                    result = Create(activityPresenter.BackingModel as UpdateInfo);
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ContactEntity contactEntity)
        {
            var contactInfoPresenter = new ContactInfoPresenter(contactEntity);

            return Create(new ObservableCollection<ContactInfoPresenter> { contactInfoPresenter });
        }

        public IWorkspaceDetailsViewModel Create(IEnumerable<ContactInfoPresenter> contactInfoPresenterList)
        {
            var result = _serviceLocator.GetInstance<IConversationViewModel>();
            result.Recipients = (ObservableCollection<ContactInfoPresenter>)contactInfoPresenterList;
            result.Model = contactInfoPresenterList.First();

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ClippingEntity clippingEntity)
        {
            var result = _serviceLocator.GetInstance<IClippingDetailsViewModel>();
            result.Model = new ClippingPresenter(clippingEntity);

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
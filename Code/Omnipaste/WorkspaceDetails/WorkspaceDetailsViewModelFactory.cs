namespace Omnipaste.WorkspaceDetails
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.WorkspaceDetails.GroupMessage;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
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

        public IWorkspaceDetailsViewModel Create(ActivityPresenter activityPresenter)
        {
            IWorkspaceDetailsViewModel result;
            switch (activityPresenter.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = Create(activityPresenter.BackingModel as ClippingPresenter);
                    break;
                case ActivityTypeEnum.Message:
                case ActivityTypeEnum.Call:
                    result = Create(activityPresenter.ExtraData.ContactInfo as ContactInfoPresenter);
                    break;
                case ActivityTypeEnum.Version:
                    result = _serviceLocator.GetInstance<IVersionDetailsViewModel>();
                    result.Model = activityPresenter.BackingModel;
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            return result;
        }

        public IWorkspaceDetailsViewModel Create(IContactInfoPresenter contactInfoPresenter)
        {
            var result = _serviceLocator.GetInstance<IConversationViewModel>();
            result.Model = contactInfoPresenter;

            return result;
        }

        public IWorkspaceDetailsViewModel Create(IEnumerable<ContactInfoPresenter> contactInfoPresenterList)
        {
            var result = _serviceLocator.GetInstance<IGroupMessageDetailsViewModel>();
            result.Recipients = (ObservableCollection<ContactInfoPresenter>)contactInfoPresenterList;

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ClippingPresenter clippingPresenter)
        {
            var result = _serviceLocator.GetInstance<IClippingDetailsViewModel>();
            result.Model = clippingPresenter;

            return result;
        }
    }
}
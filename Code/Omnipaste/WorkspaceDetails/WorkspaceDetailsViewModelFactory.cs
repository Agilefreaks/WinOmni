namespace Omnipaste.WorkspaceDetails
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Entities;
    using Omnipaste.Models;
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

        public IWorkspaceDetailsViewModel Create(ActivityModel activityModel)
        {
            IWorkspaceDetailsViewModel result;
            switch (activityModel.Type)
            {
                case ActivityTypeEnum.Clipping:
                    result = Create(activityModel.BackingEntity as ClippingEntity);
                    break;
                case ActivityTypeEnum.Message:
                case ActivityTypeEnum.Call:
                    result = Create(activityModel.ContactEntity);
                    break;
                case ActivityTypeEnum.Version:
                    result = Create(activityModel.BackingEntity as UpdateEntity);
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ContactEntity contactEntity)
        {
            var contactModel = new ContactModel(contactEntity);

            return Create(new ObservableCollection<ContactModel> { contactModel });
        }

        public IWorkspaceDetailsViewModel Create(IEnumerable<ContactModel> contactModelList)
        {
            var result = _serviceLocator.GetInstance<IConversationViewModel>();
            result.Recipients = (ObservableCollection<ContactModel>)contactModelList;
            result.Model = contactModelList.First();

            return result;
        }

        public IWorkspaceDetailsViewModel Create(ClippingEntity clippingEntity)
        {
            var result = _serviceLocator.GetInstance<IClippingDetailsViewModel>();
            result.Model = new ClippingModel(clippingEntity);

            return result;
        }

        public IWorkspaceDetailsViewModel Create(UpdateEntity updateEntity)
        {
            var result = _serviceLocator.GetInstance<IVersionDetailsViewModel>();
            result.Model = new UpdateModel(updateEntity);

            return result;
        }

        #endregion
    }
}
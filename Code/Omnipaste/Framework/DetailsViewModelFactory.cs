namespace Omnipaste.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Activities.ActivityDetails.Version;
    using Omnipaste.Activities.ActivityList.Activity;
    using Omnipaste.Clippings.CilppingDetails;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Details;

    public class DetailsViewModelFactory : IDetailsViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public DetailsViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        #region IDetailsViewModelFactory Members

        public IDetailsViewModelWithHeader Create(ActivityModel activityModel)
        {
            IDetailsViewModelWithHeader result;
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

        public IDetailsViewModelWithHeader Create(ContactEntity contactEntity)
        {
            var contactModel = new ContactModel(contactEntity);

            return Create(new ObservableCollection<ContactModel> { contactModel });
        }

        public IDetailsViewModelWithHeader Create(IEnumerable<ContactModel> contactModelList)
        {
            var result = _serviceLocator.GetInstance<IConversationViewModel>();
            result.Recipients = (ObservableCollection<ContactModel>)contactModelList;
            result.Model = contactModelList.FirstOrDefault();

            return result;
        }

        public IDetailsViewModelWithHeader Create(ClippingEntity clippingEntity)
        {
            var result = _serviceLocator.GetInstance<IClippingDetailsViewModel>();
            result.Model = new ClippingModel(clippingEntity);

            return result;
        }

        public IDetailsViewModelWithHeader Create(UpdateEntity updateEntity)
        {
            var result = _serviceLocator.GetInstance<IVersionDetailsViewModel>();
            result.Model = new UpdateModel(updateEntity);

            return result;
        }

        #endregion
    }
}
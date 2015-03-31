﻿namespace Omnipaste.DetailsViewModel
{
    using OmniCommon.Interfaces;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public abstract class ConversationItemViewModel<TModel> : DetailsViewModelWithAutoRefresh<TModel>
        where TModel : class, IConversationModel
    {
        private readonly ContactModel _currentUser;

        private ContactModel _contact;

        protected ConversationItemViewModel(
            IUiRefreshService uiRefreshService,
            IConfigurationService configurationService)
            : base(uiRefreshService)
        {
            _currentUser = new ContactModel(new UserContactEntity(configurationService.UserInfo));
        }

        public ContactModel Contact
        {
            get
            {
                return _contact;
            }
            set
            {
                if (Equals(value, _contact))
                {
                    return;
                }
                _contact = value;
                NotifyOfPropertyChange();
            }
        }

        public override TModel Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                Contact = value.Source == SourceType.Local
                                  ? _currentUser
                                  : value.ContactModel;
            }
        }
    }
}
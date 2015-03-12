﻿namespace Omnipaste.DetailsViewModel
{
    using OmniCommon.Interfaces;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;

    public abstract class ConversationItemViewModel<TModel> : DetailsViewModelWithAutoRefresh<TModel>
        where TModel : class, IConversationPresenter
    {
        private readonly ContactInfoPresenter _currentUserInfo;

        private ContactInfoPresenter _contactInfo;

        protected ConversationItemViewModel(
            IUiRefreshService uiRefreshService,
            IConfigurationService configurationService)
            : base(uiRefreshService)
        {
            _currentUserInfo = new ContactInfoPresenter(new UserContactInfo(configurationService.UserInfo));
        }

        public ContactInfoPresenter ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
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
                ContactInfo = value.Source == SourceType.Local
                                  ? _currentUserInfo
                                  : value.ContactInfoPresenter;
            }
        }
    }
}
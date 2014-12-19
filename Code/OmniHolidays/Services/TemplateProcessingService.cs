namespace OmniHolidays.Services
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniUI.Models;

    public class TemplateProcessingService : ITemplateProcessingService
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly Dictionary<string, Func<dynamic, string>> _tagReplacementList;

        private UserInfo _userInfo;

        #endregion

        #region Constructors and Destructors

        public TemplateProcessingService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _tagReplacementList = new Dictionary<string, Func<dynamic, string>>
                                      {
                                          { "%UserFirstName%", (context => context.User.FirstName) },
                                          { "%UserLastName%", (context => context.User.LastName) },
                                          { "%ContactFirstName%", (context => context.Contact.FirstName) },
                                          { "%ContactLastName%", (context => context.Contact.LastName) },
                                      };
        }

        #endregion

        #region Properties

        private UserInfo UserInfo
        {
            get
            {
                return _userInfo ?? (_userInfo = _configurationService.UserInfo);
            }
        }

        #endregion

        #region Public Methods and Operators

        public string Process(string template, IContactInfo contactInfo)
        {
            template = template ?? string.Empty;
            dynamic context = new ExpandoObject();
            context.User = UserInfo;
            context.Contact = contactInfo;
            return _tagReplacementList.Aggregate(
                template,
                (result, currentEntry) => result.Replace(currentEntry.Key, currentEntry.Value(context)));
        }

        #endregion
    }
}
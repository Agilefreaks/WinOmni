namespace OmniHolidays
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniHolidays.MessagesWorkspace;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniHolidays.MessagesWorkspace.MessageDetails.MessageCategory;
    using OmniHolidays.MessagesWorkspace.MessageDetails.MessageList;
    using OmniHolidays.Providers;
    using OmniUI;
    using OmniUI.MainMenuEntry;
    using OmniUI.Presenters;

    public class HolidaysModule : ModuleBase
    {
        #region Methods

        protected override IEnumerable<Type> GenerateSingletonTypesList()
        {
            return new[] { typeof(MessagesWorkspace.MessagesWorkspace) };
        }

        protected override void LoadCore()
        {
            Kernel.Bind<IMessageDefinitionProvider>().To<GreetingDefinitionProvider>().InSingletonScope();

            Kernel.Bind<IMainMenuEntryViewModel>().To<MessagesMainMenuEntryViewModel>().InSingletonScope();
            Kernel.Bind<IMessageStepViewModel>().To<MessageCategoryViewModel>();
            Kernel.Bind<IMessageStepViewModel>().To<MessageListViewModel>();
            Kernel.Bind<IObservable<IContactInfoPresenter>>()
                .ToMethod(
                    context =>
                    (Enumerable.Range(0, 20)).Select(
                        _ => new ContactInfoPresenter { Identifier = Path.GetRandomFileName() }).ToObservable());
        }

        #endregion
    }
}
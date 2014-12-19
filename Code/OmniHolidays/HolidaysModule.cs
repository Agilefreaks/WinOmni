namespace OmniHolidays
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using OmniHolidays.MessagesWorkspace;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniHolidays.MessagesWorkspace.MessageDetails.MessageCategory;
    using OmniHolidays.MessagesWorkspace.MessageDetails.MessageList;
    using OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage;
    using OmniHolidays.Providers;
    using OmniUI;
    using OmniUI.MainMenuEntry;

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
            Kernel.Bind<IMessageStepViewModel>().ToMethod(context => context.Kernel.Get<ISendingMessageViewModel>());
        }

        #endregion
    }
}
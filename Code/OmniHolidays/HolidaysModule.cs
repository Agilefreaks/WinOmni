namespace OmniHolidays
{
    using System;
    using System.Collections.Generic;
    using OmniHolidays.MessagesWorkspace;
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
            Kernel.Bind<IMainMenuEntryViewModel>().To<MessagesMainMenuEntryViewModel>().InSingletonScope();
        }

        #endregion
    }
}
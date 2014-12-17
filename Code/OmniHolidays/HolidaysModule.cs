namespace OmniHolidays
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniHolidays.MessagesWorkspace;
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
            Kernel.Bind<IMainMenuEntryViewModel>().To<MessagesMainMenuEntryViewModel>().InSingletonScope();
            Kernel.Bind<IObservable<IContactInfoPresenter>>()
                .ToMethod(
                    context =>
                    (Enumerable.Range(0, 20)).Select(
                        _ => new ContactInfoPresenter { Identifier = Path.GetRandomFileName() }).ToObservable());
        }

        #endregion
    }
}
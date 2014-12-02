namespace Omnipaste.ActivityList
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Omnipaste.Activity;
    using Omnipaste.Framework;
    using Omnipaste.Properties;

    public class ActivityListViewModel : ListViewModelBase<Activity, ActivityViewModel>, IActivityListViewModel
    {
        #region Constructors and Destructors

        public ActivityListViewModel(IClipboardHandler clipboardHandler, IEventsHandler eventsHandler)
            : base(GetActivityObservable(clipboardHandler, eventsHandler))
        {
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.Activity;
            }
        }

        #endregion

        #region Methods

        protected override ActivityViewModel CreateViewModel(Activity entity)
        {
            return new ActivityViewModel { Model = entity };
        }

        private static IObservable<Activity> GetActivityObservable(
            IClipboardHandler clipboardHandler,
            IEventsHandler eventsHandler)
        {
            return
                clipboardHandler.Select(clipping => new Activity(clipping))
                    .Merge(eventsHandler.Select(@event => new Activity(@event)));
        }

        #endregion
    }
}
﻿namespace OmniUI.Framework.Behaviors
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;

    public class TimeProgressUpdateBehavior : DisposableBehavior<ProgressBar>
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration",
            typeof(TimeSpan),
            typeof(TimeProgressUpdateBehavior),
            new PropertyMetadata(default(TimeSpan)));

        public const int UpdateIntervalMilliseconds = 42;

        private IDisposable _progressUpdaterSubscription;

        public TimeSpan Duration
        {
            get
            {
                return (TimeSpan)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        protected override void SetUp()
        {
            ResetProgress();
        }

        protected override void TearDown()
        {
            DisposeProgressUpdaterSubscription();
        }

        private void ResetProgress()
        {
            AssociatedObject.Value = 0;
            BeginProgressUpdate();
        }

        private void BeginProgressUpdate()
        {
            var totalProgressUpdates = (int)Duration.TotalMilliseconds / UpdateIntervalMilliseconds + 1;
            var progressIncrement = (double)100 / totalProgressUpdates;
            var updateInterval = TimeSpan.FromMilliseconds(UpdateIntervalMilliseconds);

            DisposeProgressUpdaterSubscription();
            _progressUpdaterSubscription =
                Enumerable.Range(0, totalProgressUpdates)
                    .ToSequentialDelayedObservable(updateInterval)
                    .Take(totalProgressUpdates)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeOn(SchedulerProvider.Default)
                    .Subscribe(_ => { AssociatedObject.Value += progressIncrement; }, _ => { });
        }

        private void DisposeProgressUpdaterSubscription()
        {
            if (_progressUpdaterSubscription == null)
            {
                return;
            }

            _progressUpdaterSubscription.Dispose();
            _progressUpdaterSubscription = null;
        }
    }
}
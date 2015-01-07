namespace OmniUI.Behaviors
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

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled",
            typeof(bool),
            typeof(TimeProgressUpdateBehavior),
            new PropertyMetadata(default(bool), OnStateChanged));

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

        public bool IsEnabled
        {
            get
            {
                return (bool)GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimeProgressUpdateBehavior)d).ResetProgress();
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
            if (IsEnabled)
            {
                AssociatedObject.Value = 0;
                BeginProgressUpdate();
            }
            else
            {
                AssociatedObject.Value = 100;
            }
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
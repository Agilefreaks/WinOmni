namespace OmniHolidaysTests.Services
{
    using System;
    using System.Reactive.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniHolidays.Services;

    [TestFixture]
    public class ProgressUpdaterFactoryTests
    {
        private ProgressUpdaterFactory _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new ProgressUpdaterFactory();
        }

        [Test]
        public void Create_Always_ReturnsAnObservableThatInvokesActionInIntervals()
        {
            double totalProgress = 0;
            var progressObservable = _subject.Create(2000, d => totalProgress += d);

            progressObservable.Wait();

            (Math.Ceiling(totalProgress)).Should().Be(100);
        }

        [Test]
        public void Create_Always_InvokesCallbackEveryInterval()
        {
            int callCount = 0;
            var totalDuration = 2000d;
            var progressObservable = _subject.Create(totalDuration, d => callCount++);

            progressObservable.Wait();

            callCount.Should().Be((int)Math.Ceiling(totalDuration / ProgressUpdaterFactory.UpdateIntervalMilliseconds));
        }
    }
}

namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Helpers;

    [TestFixture]
    public class SecurePermanentRepositoryTest : BaseRepositoryTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _subject = new TestModelRepository();
        }

        [Test]
        public void Save_WillSetItemsToExpireInOneDay()
        {
            var testableObserver = TestScheduler.CreateObserver<TestModel>();
            TestScheduler.Schedule(() => _subject.Save(new TestModel { UniqueId = "42" }));
            TestScheduler.Schedule(new TimeSpan(0, 23, 59, 0), () => _subject.Get("42").Subscribe(testableObserver));
            TestScheduler.Schedule(new TimeSpan(1, 0, 0, 1), () => _subject.Get("42").Subscribe(testableObserver));

            TestScheduler.Start();

            testableObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnError);
        }

        public class TestModelRepository : SecurePermanentRepository<TestModel>
        {
            public TestModelRepository()
                : base("test")
            {
            }
        }
    }
}
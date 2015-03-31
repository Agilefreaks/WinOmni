namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Services.Repositories;
    using OmnipasteTests.Framework.Helpers;

    [TestFixture]
    public class SecurePermanentRepositoryTest : BaseRepositoryTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            Subject = new TestModelRepository();
        }

        [Test]
        public void Save_WillSetItemsToExpireInOneDay()
        {
            var testableObserver = TestScheduler.CreateObserver<TestModel>();
            TestScheduler.Schedule(() => Subject.Save(new TestModel { UniqueId = "42" }));
            TestScheduler.Schedule(new TimeSpan(0, 23, 59, 0), () => Subject.Get("42").Subscribe(testableObserver));
            TestScheduler.Schedule(new TimeSpan(1, 0, 0, 1), () => Subject.Get("42").Subscribe(testableObserver));

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
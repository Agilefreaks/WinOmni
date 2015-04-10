namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Framework.Helpers;

    public abstract class BaseRepositoryTests
    {
        protected BaseRepository<TestModel> Subject;

        protected TestScheduler TestScheduler;

        [SetUp]
        public virtual void SetUp()
        {
            TestScheduler = new TestScheduler();
            SchedulerProvider.Default = TestScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Save_Always_SavesItem()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable =
                TestScheduler.Start(() => Subject.Save(testModel).Select(_ => Subject.GetAll()).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.First().UniqueId.Should().Be(testModel.UniqueId);
        }

        [Test]
        public void Save_Always_ReturnsRepositoryOperationObservable()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable = TestScheduler.Start(() => Subject.Save(testModel));

            testObservable.Messages.Should().HaveCount(2);
            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
        }

        [Test]
        public void GetWithFunc_WhenItemExists_ReturnsItem()
        {
            const string UniqueId = "42";
            var testModel = new TestModel { UniqueId = UniqueId };

            var testObservable =
                TestScheduler.Start(
                    () => Subject.Save(testModel).Select(_ => Subject.Get(c => c.UniqueId == UniqueId)).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.UniqueId.Should().Be(testModel.UniqueId);
        }

        [Test]
        public void GetWithFunc_WhenItemDoesNotExist_ReturnsError()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable =
                TestScheduler.Start(() => Subject.Save(testModel).Select(_ => Subject.Get(c => false)).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnError);
        }

        [Test]
        public void Delete_WhenItemExists_DeletesItem()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObserver = TestScheduler.CreateObserver<IEnumerable<TestModel>>();

            TestScheduler.Schedule(() => Subject.Save(testModel));
            TestScheduler.Schedule(() => Subject.Delete(testModel.UniqueId).Subscribe());
            TestScheduler.Schedule(() => Subject.GetAll().Subscribe(testObserver));

            TestScheduler.Start();

            testObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObserver.Messages[0].Value.Value.Should().BeEmpty();
        }

        [Test]
        public void Delete_WhenItemExists_NotifiesOperation()
        {
            var testModel = new TestModel { UniqueId = "42" };
            var results = new List<RepositoryOperation<TestModel>>();
            Subject.GetOperationObservable().Subscribe(ro => results.Add(ro));

            TestScheduler.Schedule(() => Subject.Save(testModel));
            TestScheduler.Schedule(() => Subject.Delete(testModel.UniqueId).Subscribe());

            TestScheduler.Start();

            results[1].RepositoryMethod.Should().Be(RepositoryMethodEnum.Delete);
            results[1].GetItem<TestModel>().UniqueId.Should().Be(testModel.UniqueId);
        }

        [Test]
        public void Get_WithExistingId_WillReturnTheModel()
        {
            var testModel = new TestModel { UniqueId = "42" };
            
            var testableObserver = TestScheduler.Start(() => Subject.Save(testModel).Select(_ => Subject.Get("42")).Switch());

            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.UniqueId.Should().Be("42");
        }
    }
}
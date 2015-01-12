namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class InMemoryRepositoryTests
    {
        private InMemoryRepository<TestModel> _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _subject = new TestModelRepository();
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
                _testScheduler.Start(() => _subject.Save(testModel).Select(_ => _subject.GetAll()).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.Should().Contain(testModel);
        }

        [Test]
        public void Save_Always_ReturnsRepositoryOperationObservable()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable = _testScheduler.Start(() => _subject.Save(testModel));

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.RepositoryMethod.Should().Be(RepositoryMethodEnum.Save);
        }

        [Test]
        public void Save_Always_NotifiesOperation()
        {
            var testModel = new TestModel { UniqueId = "42" };
            var results = new List<RepositoryOperation<TestModel>>();
            _subject.OperationObservable.Subscribe(ro => results.Add(ro));

            _testScheduler.Start(() => _subject.Save(testModel));

            results[0].RepositoryMethod.Should().Be(RepositoryMethodEnum.Save);
        }

        [Test]
        public void Delete_WhenItemExists_DeletesItem()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable =
                _testScheduler.Start(
                    () =>
                    _subject.Save(testModel)
                        .Select(_ => _subject.Delete(testModel.UniqueId))
                        .Switch()
                        .Select(_ => _subject.GetAll())
                        .Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.Should().BeEmpty();
        }

        [Test]
        public void Delete_WhenItemExists_NotifiesOperation()
        {
            var testModel = new TestModel { UniqueId = "42" };
            var results = new List<RepositoryOperation<TestModel>>();
            _subject.OperationObservable.Subscribe(ro => results.Add(ro));

            _testScheduler.Start(() => _subject.Save(testModel).Select(_ => _subject.Delete(testModel.UniqueId)).Switch());

            results[1].RepositoryMethod.Should().Be(RepositoryMethodEnum.Delete);
        }

        public class TestModel : BaseModel
        {
        }

        public class TestModelRepository : InMemoryRepository<TestModel>
        {
        }
    }
}
namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services.Repositories;
    using OmnipasteTests.Helpers;

    public abstract class BaseRepositoryTest
    {
        protected BaseRepository<TestModel> _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public virtual void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
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
            testObservable.Messages[0].Value.Value.First().UniqueId.Should().Be(testModel.UniqueId);
        }

        [Test]
        public void Save_Always_ReturnsRepositoryOperationObservable()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable = _testScheduler.Start(() => _subject.Save(testModel));

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.RepositoryMethod.Should().Be(RepositoryMethodEnum.Create);
        }

        [Test]
        public void Save_Always_NotifiesOperation()
        {
            var testModel = new TestModel { UniqueId = "42" };
            var results = new List<RepositoryOperation<TestModel>>();
            _subject.OperationObservable.Subscribe(ro => results.Add(ro));

            _testScheduler.Start(() => _subject.Save(testModel));

            results[0].RepositoryMethod.Should().Be(RepositoryMethodEnum.Create);
        }

        [Test]
        public void Save_WhenItemWasPreviouslySaved_NotifiesUpdateOperation()
        {
            var testModel = new TestModel { UniqueId = "42" };
            var sameTestModel = new TestModel { UniqueId = "42", IsDeleted = true };
            var results = new List<RepositoryOperation<TestModel>>();
            _subject.OperationObservable.Subscribe(ro => results.Add(ro));

            _testScheduler.Start(() => _subject.Save(testModel).Select(_ => _subject.Save(sameTestModel)).Switch());

            results[1].RepositoryMethod.Should().Be(RepositoryMethodEnum.Update);
        }

        [Test]
        public void GetWithFunc_WhenItemExists_ReturnsItem()
        {
            const string UniqueId = "42";
            var testModel = new TestModel { UniqueId = UniqueId };

            var testObservable =
                _testScheduler.Start(
                    () => _subject.Save(testModel).Select(_ => _subject.Get(c => c.UniqueId == UniqueId)).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.UniqueId.Should().Be(testModel.UniqueId);
        }

        [Test]
        public void GetWithFunc_WhenItemDoesNotExist_ReturnsNull()
        {
            var testModel = new TestModel { UniqueId = "42" };

            var testObservable =
                _testScheduler.Start(() => _subject.Save(testModel).Select(_ => _subject.Get(c => false)).Switch());

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.Should().Be(null);
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

            _testScheduler.Start(
                () => _subject.Save(testModel).Select(_ => _subject.Delete(testModel.UniqueId)).Switch());

            results[1].RepositoryMethod.Should().Be(RepositoryMethodEnum.Delete);
        }
    }
}
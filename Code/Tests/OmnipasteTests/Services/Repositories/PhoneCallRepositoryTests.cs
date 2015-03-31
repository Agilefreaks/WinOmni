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
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class PhoneCallRepositoryTests
    {
        private PhoneCallRepository _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            _subject = new PhoneCallRepository();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            _testScheduler.Stop();
        }

        [Test]
        public void Save_SavesMultipleObjectTypes()
        {
            var localPhoneCall = BuildLocalPhoneCall();
            var remotePhoneCall = BuildRemotePhoneCall();
            var testObservable = _testScheduler.CreateObserver<IEnumerable<PhoneCallEntity>>();

            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));
            _testScheduler.Schedule(() => _subject.GetAll().Subscribe(testObservable));
            
            _testScheduler.Start();

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.Should().ContainSingle(pc => pc.UniqueId == "42" && pc.ContactInfoUniqueId == "123");
            testObservable.Messages[0].Value.Value.Should().ContainSingle(pc => pc.UniqueId == "43" && pc.ContactInfoUniqueId == "123");
            testObservable.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Test]
        public void Save_WithMultipleTypes_NotifiesRepositoryOperations()
        {
            var localPhoneCall = BuildLocalPhoneCall();
            var remotePhoneCall = BuildRemotePhoneCall();

            var results = new List<RepositoryOperation<PhoneCallEntity>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));

            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));

            _testScheduler.Start();

            results.Should().HaveCount(2);
            results.First().GetItem<LocalPhoneCallEntity>().UniqueId.Should().Be("42");
            results.First().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
            results.Last().GetItem<RemotePhoneCallEntity>().UniqueId.Should().Be("43");
            results.Last().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
        }

        [Test]
        public void Delete_WithExistingIdOfALocalPhoneCall_DeletesItAndNotifiesRepositoryOperations()
        {
            var localPhoneCall = BuildLocalPhoneCall();

            var results = new List<RepositoryOperation<PhoneCallEntity>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));
            var testableObserver = _testScheduler.CreateObserver<IEnumerable<PhoneCallEntity>>();

            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.Delete("42").Subscribe());
            _testScheduler.Schedule(() => _subject.GetAll().Subscribe(testableObserver));

            _testScheduler.Start();

            results.Should().HaveCount(2);
            results.First().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
            results.Last().RepositoryMethod.Should().Be(RepositoryMethodEnum.Delete);
            testableObserver.Messages.First().Value.Value.Should().HaveCount(0);
        }

        [Test]
        public void Delete_WithExistingIdOfARemotePhoneCall_DeletesItAndNotifiesRepositoryOperations()
        {
            var remotePhoneCall = BuildRemotePhoneCall();

            var results = new List<RepositoryOperation<PhoneCallEntity>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));
            var testableObserver = _testScheduler.CreateObserver<IEnumerable<PhoneCallEntity>>();

            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));
            _testScheduler.Schedule(() => _subject.Delete("43").Subscribe());
            _testScheduler.Schedule(() => _subject.GetAll().Subscribe(testableObserver));

            _testScheduler.Start();

            results.Should().HaveCount(2);
            results.First().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
            results.Last().RepositoryMethod.Should().Be(RepositoryMethodEnum.Delete);
            testableObserver.Messages.First().Value.Value.Should().HaveCount(0);
        }

        [Test]
        public void GetOperationObservable_WithRemotePhoneCall_ReturnsRemoteCallOperation()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var testObservable = _testScheduler.CreateObserver<RepositoryOperation>();
            _subject.GetOperationObservable<RemotePhoneCallEntity>().Subscribe(testObservable);

            _testScheduler.Start(() => _subject.Save(remotePhoneCall));

            testObservable.Messages.Should().HaveCount(1);
            testObservable.Messages.First().Value.Value.GetItem<PhoneCallEntity>().UniqueId.Should().Be(remotePhoneCall.UniqueId);
        }

        [Test]
        public void GetForContact_WhenThereArePhoneCallsForThatContact_ReturnsTheContacts()
        {
            var remotePhoneCall = BuildRemotePhoneCall();
            var localPhoneCall = BuildLocalPhoneCall();
            var testObservable = _testScheduler.CreateObserver<IEnumerable<PhoneCallEntity>>();

            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));
            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.GetForContact(new ContactEntity { UniqueId = "123" }).Subscribe(testObservable));

            _testScheduler.Start();

            testObservable.Messages.First().Value.Value.Should().HaveCount(2);
            testObservable.Messages.First().Value.Value.Should().ContainSingle(pc => pc.UniqueId == "42");
            testObservable.Messages.First().Value.Value.Should().ContainSingle(pc => pc.UniqueId == "43");
        }

        [Test]
        public void ForContact_WithRepositoryOperation_ReturnsPhoneCall()
        {
            var remotePhoneCall = BuildRemotePhoneCall();
            var localPhoneCall = BuildLocalPhoneCall();
            var testObservable = _testScheduler.CreateObserver<RepositoryOperation<PhoneCallEntity>>();

            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));
            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));

            _subject.GetOperationObservable()
                .OnMethod(RepositoryMethodEnum.Changed)
                .ForContact(new ContactEntity { UniqueId = "123" })
                .Subscribe(testObservable);

            _testScheduler.Start();

            testObservable.Messages.Should().HaveCount(2);
            testObservable.Messages.First().Value.Value.Item.UniqueId.Should().Be("43");
            testObservable.Messages.Last().Value.Value.Item.UniqueId.Should().Be("42");
        }

        private static RemotePhoneCallEntity BuildRemotePhoneCall()
        {
            return new RemotePhoneCallEntity { UniqueId = "43", ContactInfoUniqueId = "123" };
        }

        private static LocalPhoneCallEntity BuildLocalPhoneCall()
        {
            return new LocalPhoneCallEntity { UniqueId = "42", ContactInfoUniqueId = "123" };
        }
    }
}
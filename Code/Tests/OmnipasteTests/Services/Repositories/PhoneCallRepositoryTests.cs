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
            var testObservable = _testScheduler.CreateObserver<IEnumerable<PhoneCall>>();

            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));
            _testScheduler.Schedule(() => _subject.GetAll().Subscribe(testObservable));
            
            _testScheduler.Start();

            testObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[0].Value.Value.First().UniqueId.Should().Be("42");
            testObservable.Messages[0].Value.Value.First()
                .ContactInfoUniqueId.Should()
                .Be("123");
            testObservable.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testObservable.Messages[1].Value.Value.First().UniqueId.Should().Be("43");
        }

        [Test]
        public void Save_WithMultipleTypes_NotifiesRepositoryOperations()
        {
            var localPhoneCall = BuildLocalPhoneCall();
            var remotePhoneCall = BuildRemotePhoneCall();

            var results = new List<RepositoryOperation<PhoneCall>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));

            _testScheduler.Schedule(() => _subject.Save(localPhoneCall));
            _testScheduler.Schedule(() => _subject.Save(remotePhoneCall));

            _testScheduler.Start();

            results.Should().HaveCount(2);
            results.First().GetItem<LocalPhoneCall>().UniqueId.Should().Be("42");
            results.First().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
            results.Last().GetItem<RemotePhoneCall>().UniqueId.Should().Be("43");
            results.Last().RepositoryMethod.Should().Be(RepositoryMethodEnum.Changed);
        }

        [Test]
        public void Delete_WithExistingIdOfALocalPhoneCall_DeletesItAndNotifiesRepositoryOperations()
        {
            var localPhoneCall = BuildLocalPhoneCall();

            var results = new List<RepositoryOperation<PhoneCall>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));
            var testableObserver = _testScheduler.CreateObserver<IEnumerable<PhoneCall>>();

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

            var results = new List<RepositoryOperation<PhoneCall>>();
            _subject.GetOperationObservable().Subscribe(ro => results.Add(ro));
            var testableObserver = _testScheduler.CreateObserver<IEnumerable<PhoneCall>>();

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
        public void GetByContact_Always_ReturnsCallsForContact()
        {
            var contactInfo = new ContactInfo { UniqueId = "43" };
            var call1 = new LocalPhoneCall { UniqueId = "42", ContactInfoUniqueId = "43" };
            var call2 = new LocalPhoneCall { UniqueId = "1", ContactInfoUniqueId = "2" };
            var observable =
                _subject.Save(call1)
                    .Select(_ => _subject.Save(call2))
                    .Select(_ => _subject.GetByContact<LocalPhoneCall>(contactInfo))
                    .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Count().Should().Be(1);
            result.Messages.First().Value.Value.First().UniqueId.Should().Be("42");
        }

        [Test]
        public void GetOperationObservable_WithRemotePhoneCall_ReturnsRemoteCallOperation()
        {
            var remotePhoneCall = new RemotePhoneCall();
            var testObservable = _testScheduler.CreateObserver<RepositoryOperation>();
            _subject.GetOperationObservable<RemotePhoneCall>().Subscribe(testObservable);

            _testScheduler.Start(() => _subject.Save(remotePhoneCall));

            testObservable.Messages.Should().HaveCount(1);
            testObservable.Messages.First().Value.Value.GetItem<PhoneCall>().UniqueId.Should().Be(remotePhoneCall.UniqueId);
        }

        private static RemotePhoneCall BuildRemotePhoneCall()
        {
            return new RemotePhoneCall { UniqueId = "43" };
        }

        private static LocalPhoneCall BuildLocalPhoneCall()
        {
            return new LocalPhoneCall { UniqueId = "42", ContactInfoUniqueId = "123" };
        }
    }
}
namespace OmnipasteTests.Framework.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Activities.ActivityList.Activity;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    [TestFixture]
    public class ActivityModelTests
    {
        private ActivityModel _subject;

        [SetUp]
        public void Setup()
        {
            var remoteSmsMessageEntity = new RemoteSmsMessageEntity
                                                                {
                                                                    Content = "test message",
                                                                    Time = new DateTime(2000, 1, 1)
                                                                };
            var contactEntity = new ContactEntity { FirstName = "first", LastName = "last" };
            _subject = ActivityModel.BeginBuild(remoteSmsMessageEntity)
                    .WithType(ActivityTypeEnum.Message)
                    .WithContent(remoteSmsMessageEntity.Content)
                    .WithDevice(remoteSmsMessageEntity)
                    .WithContact(contactEntity)
                    .Build();
        }

        [Test]
        public void ToString_Always_RetursnACorrectlyFormattedString()
        {
            var result = _subject.ToString();

            result.Should().Be("From Cloud SMS from:  test message");
        }
    }
}
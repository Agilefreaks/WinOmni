﻿namespace OmnipasteTests.Entities
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using SMS.Dto;

    [TestFixture]
    public class RemoteSmsMessageEntityTests
    {
        [Test]
        public void Source_Always_ReturnsRemote()
        {
            new RemoteSmsMessageEntity(new SmsMessageDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
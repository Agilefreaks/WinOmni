﻿using System;
using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Services;
using Moq;
using NUnit.Framework;

namespace CloudClipboardTests.Services
{
    [TestFixture]
    public class ClickOnceActivationDataProviderTests
    {
        ClickOnceActivationDataProvider _subject;
        private Mock<IApplicationDeploymentInfo> _mockActivationInfoProvider;

        [SetUp]
        public void Setup()
        {
            _mockActivationInfoProvider = new Mock<IApplicationDeploymentInfo>();
            _subject = new ClickOnceActivationDataProvider(_mockActivationInfoProvider.Object);
        }

        [Test]
        public void GetCommunicationChannel_ApplicationDeploymentInfoHasValidActivationUri_CallsApplicationDeploymentInfoActivationUri()
        {
            _mockActivationInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            _mockActivationInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://test.com"));

            _subject.GetActivationData();

            _mockActivationInfoProvider.Verify(x => x.ActivationUri);
        }
    }
}

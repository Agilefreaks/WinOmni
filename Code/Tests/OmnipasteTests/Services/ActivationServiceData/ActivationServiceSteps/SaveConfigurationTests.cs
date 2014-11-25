﻿namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class SaveConfigurationTests
    {
        private SaveConfiguration _subject;

        private Mock<IConfigurationService> _configurationService;

        [SetUp]
        public void SetUp()
        {
            _configurationService = new Mock<IConfigurationService>();

            _subject = new SaveConfiguration(_configurationService.Object);
        }

        [Test]
        public void Execute_Always_SaveTheConfiguration()
        {
            _subject.Parameter = new DependencyParameter(null, new Token("access token", "refresh token"));

            var autoResetEvent = new AutoResetEvent(false);
            SchedulerProvider.Default = new NewThreadScheduler();

            Task.Factory.StartNew(
                () =>
                {
                    _subject.Execute().Wait();
                    autoResetEvent.Set();
                });

            autoResetEvent.WaitOne();

            _configurationService.Verify(m => m.SaveAuthSettings("access token", "refresh token"));
        }
    }
}
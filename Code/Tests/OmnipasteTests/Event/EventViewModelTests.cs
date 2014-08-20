﻿namespace OmnipasteTests.Event
{
    using Caliburn.Micro;
    using Events.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Event;
    using Omnipaste.EventAggregatorMessages;

    [TestFixture]
    public class EventViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IEventViewModel _subject;

        private Mock<IEventAggregator> _mockEventAggregator;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _kernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);
            _subject = _kernel.Get<EventViewModel>();
            _subject.Model = new Event { PhoneNumber = "phone_number" };
        }

        [Test]
        public void SendSms_SendsShowSmsMessage()
        {
            _subject.SendSms();

            _mockEventAggregator.Verify(ea => ea.Publish(It.Is<SendSmsMessage>(m => m.Recipient == "phone_number"), It.IsAny<System.Action<System.Action>>()));
        }
    }
}
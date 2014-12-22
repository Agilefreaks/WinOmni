namespace OmniHolidaysTests.MessagesWorkspace.MessageDetails
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniHolidays.MessagesWorkspace.MessageDetails.MessageCategory;
    using OmniHolidays.Providers;

    [TestFixture]
    public class MessageCategoryViewModelTests
    {
        private MessageCategoryViewModel _subject;

        private Mock<IMessageDefinitionProvider> _mockMessageDefinitionProvider;

        private TestScheduler _testScheduler;

        private CultureInfo _initialCultureInfo;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _mockMessageDefinitionProvider = new Mock<IMessageDefinitionProvider>();
            _subject = new MessageCategoryViewModel(_mockMessageDefinitionProvider.Object);

            _initialCultureInfo = CultureInfo.CurrentCulture;
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = _initialCultureInfo;
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Activate_WhenLanguagesAreNotPresent_SetsDistinctLanguagesFromMessageDefinitions()
        {
            _mockMessageDefinitionProvider.Setup(m => m.Get())
                .Returns(
                    Observable.Return(
                        new List<MessageDefinition>
                            {
                                new MessageDefinition { Language = "English" },
                                new MessageDefinition { Language = "Romanian" },
                                new MessageDefinition { Language = "Romanian" }
                            },
                        _testScheduler));

            ((IActivate)_subject).Activate();
            _testScheduler.Start(() => WaitForProperty("Languages"));
            
            _subject.Languages.Count.Should().Be(2);
            _subject.Languages.Any(l => l.Value == "English").Should().BeTrue();
            _subject.Languages.Any(l => l.Value == "Romanian").Should().BeTrue();
        }

        [Test]
        public void Activate_WhenThereAreMessagesForCurrentCultureLanguage_SetsSelectedLanguageToCurrentCultureLanguage()
        {
            _mockMessageDefinitionProvider.Setup(m => m.Get())
                .Returns(
                    Observable.Return(
                        new List<MessageDefinition>
                            {
                                new MessageDefinition { Language = "English" },
                                new MessageDefinition { Language = "Romanian" },
                                new MessageDefinition { Language = "Romanian" }
                            },
                        _testScheduler));
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ro-RO");

            ((IActivate)_subject).Activate();
            _testScheduler.Start(() => WaitForProperty("SelectedLanguage"));

            _subject.SelectedLanguage.Should().Be("Romanian");
        }

        [Test]
        public void Activate_WhenThereAreNoMessagesForCurrentCultureLanguage_SetsSelectedLanguageToFirstMessageLanguage()
        {
            _mockMessageDefinitionProvider.Setup(m => m.Get())
                .Returns(
                    Observable.Return(
                        new List<MessageDefinition>
                            {
                                new MessageDefinition { Language = "English" },
                                new MessageDefinition { Language = "Romanian" },
                                new MessageDefinition { Language = "Romanian" }
                            },
                        _testScheduler));
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("hu-HU");

            ((IActivate)_subject).Activate();
            _testScheduler.Start(() => WaitForProperty("SelectedLanguage"));

            _subject.SelectedLanguage.Should().Be("English");
        }

        private IObservable<EventPattern<object>> WaitForProperty(string propertyName)
        {
            return
                Observable.FromEventPattern(_subject, "PropertyChanged", _testScheduler)
                    .Where(e => ((PropertyChangedEventArgs)e.EventArgs).PropertyName == propertyName)
                    .Take(1);
        }
    }
}

namespace OmnipasteTests.DataProviders
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Framework.DataProviders;

    public class ArgumentsDataProviderTests
    {
        private Func<ArgumentsDataProvider> _subject;
        private Mock<IArgumentsProvider> _mockArgumentsProvider;

        [SetUp]
        public void Setup()
        {
            _mockArgumentsProvider = new Mock<IArgumentsProvider>();
            _subject = () => new ArgumentsDataProvider(_mockArgumentsProvider.Object);
        }

        [Test]
        public void AuthorizationKey_AnAuthorizationKeyIsGivenInTheArgumentsList_SetsTheValueOnTheInstance()
        {
            _mockArgumentsProvider.Setup(x => x.GetCommandLineArgs())
                .Returns(new List<string>() { "C:\\somePath.exe", "-authorizationKey=123456" });

            var dataProvider = _subject();

            dataProvider.AuthorizationKey.Should().Be("123456");
        }
    }
}
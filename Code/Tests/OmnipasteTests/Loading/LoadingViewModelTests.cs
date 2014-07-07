﻿namespace OmnipasteTests.Loading
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Dialog;
    using Omnipaste.Loading;
    using Omnipaste.Loading.ActivationFailed;
    using Omnipaste.Loading.UserToken;

    [TestFixture]
    public class LoadingViewModelTests
    {
        private ILoadingViewModel _subject;

        private IEventAggregator _eventAggregator;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private Mock<IActivationFailedViewModel> _mockActivationFailedViewModel;

        [SetUp]
        public void SetUp()
        {
            _eventAggregator = new EventAggregator();
            _mockUserTokenViewModel = new Mock<IUserTokenViewModel>();
            _mockActivationFailedViewModel = new Mock<IActivationFailedViewModel>();
            _subject = new LoadingViewModel(_eventAggregator)
                           {
                               UserTokenViewModel = _mockUserTokenViewModel.Object,
                               ActivationFailedViewModel = _mockActivationFailedViewModel.Object
                           };
            var parent = new DialogViewModel();
            parent.ActivateItem(_subject);
        }

        [Test]
        public void NewInstance_IsInLoadingState()
        {
            _subject.State.Should().Be(LoadingViewModelStateEnum.Loading);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsStateToOther()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.Other);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsTheMessageOnTheUserTokenViewModel()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage { Message = "message" });

            _mockUserTokenViewModel.VerifySet(vm => vm.Message = "message");
        }

        [Test]
        public void Handle_ActivationFailed_SetsStateToOtherAndActiveItem()
        {
            _eventAggregator.PublishOnCurrentThread(new ActivationFailedMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.Other);
            _subject.ActiveItem.Should().Be(_mockActivationFailedViewModel.Object);
        }

        [Test]
        public void StateChange_Always_TriggersNotifyOfPropertyChanged()
        {
            string changedProperty = string.Empty;
            _subject.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;

            _subject.State = LoadingViewModelStateEnum.Other;

            changedProperty.Should().Be("State");
        }
    }
}
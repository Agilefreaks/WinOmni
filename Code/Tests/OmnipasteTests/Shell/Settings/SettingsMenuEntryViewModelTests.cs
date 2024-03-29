﻿﻿namespace OmnipasteTests.Shell.Settings
 {
     using Moq;
     using NUnit.Framework;
     using Omnipaste.Shell.Settings;

     [TestFixture]
     public class SettingsMenuEntryViewModelTests
     {
         private SettingsItemViewModel _settingsHeaderViewModel;

         private Mock<ISettingsViewModel> _mockSettingsViewModel;

         [SetUp]
         public void SetUp()
         {
             _mockSettingsViewModel = new Mock<ISettingsViewModel>();
             _settingsHeaderViewModel = new SettingsItemViewModel(_mockSettingsViewModel.Object);
         }

         [Test]
         public void PerformAction_ChangesSettingsViewModelIsOpenProperty()
         {
             _settingsHeaderViewModel.PerformAction();

             _mockSettingsViewModel.VerifySet(svm => svm.IsOpen = true, Times.Once());
         }

         [Test]
         public void PerformAction_WhenSettingsIsOpen_WillCloseTheFlyout()
         {
             _mockSettingsViewModel.SetupGet(svm => svm.IsOpen).Returns(true);

             _settingsHeaderViewModel.PerformAction();

             _mockSettingsViewModel.VerifySet(svm => svm.IsOpen = false, Times.Once());
         }
     }
 }
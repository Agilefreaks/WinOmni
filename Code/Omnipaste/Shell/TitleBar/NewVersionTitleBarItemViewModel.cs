﻿namespace Omnipaste.Shell.TitleBar
{
    using System;
    using Caliburn.Micro;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Entities;
    using Omnipaste.Services;
    using OmniUI.Attributes;
    using OmniUI.TitleBarItem;
    using Omnipaste.Properties;

    [UseView(typeof(TitleBarItemView))]
    public class NewVersionTitleBarItemViewModel : Screen, ITitleBarItemViewModel
    {
        private readonly IUpdaterService _updaterService;

        private bool _canPerformAction;

        private IDisposable _updateAvailableSubscription;

        public string Icon
        {
            get
            {
                return OmniUI.Resources.IconNames.RestartIcon;
            }
        }

        public string Tag
        {
            get
            {
                return Resources.NewVersionLabel;
            }
        }

        public bool CanPerformAction
        {
            get
            {
                return _canPerformAction;
            }
            set
            {
                if (value.Equals(_canPerformAction))
                {
                    return;
                }
                _canPerformAction = value;
                NotifyOfPropertyChange();
            }
        }

        public NewVersionTitleBarItemViewModel(IUpdaterService updaterService)
        {
            _updaterService = updaterService;
            _updateAvailableSubscription = _updaterService.UpdateObservable
                .SubscribeAndHandleErrors(OnUpdateAvailable);
        }

        public void PerformAction()
        {
            _updaterService.InstallNewVersion();
        }

        public void Dispose()
        {
            DisposeUpdateAvailableSubscription();
        }

        private void OnUpdateAvailable(UpdateEntity updateEntity)
        {
            CanPerformAction = !updateEntity.WasInstalled;
        }

        private void DisposeUpdateAvailableSubscription()
        {
            if (_updateAvailableSubscription == null)
            {
                return;
            }
            _updateAvailableSubscription.Dispose();
            _updateAvailableSubscription = null;
        }
    }
}
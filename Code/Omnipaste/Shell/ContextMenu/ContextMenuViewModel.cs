﻿namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework.Behaviours;
    using OmniSync;

    public class ContextMenuViewModel : Screen, IContextMenuViewModel
    {
        #region Fields

        private BalloonNotificationInfo _balloonInfo;

        private string _iconSource;

        #endregion
        
        #region Constructors and Destructors

        public ContextMenuViewModel(IOmniService omniService)
        {
            OmniService = omniService;
            OmniService.StatusChangedObservable.Subscribe(
                status => {
                              IconSource = status == ServiceStatusEnum.Started 
                                  ? "/Connected.ico" 
                                  : "/Disconnected.ico";
                },
                exception => { });

            IconSource = "/Disconnected.ico";
        }

        #endregion

        #region Public Properties

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        public bool AutoStart { get; set; }

        public BalloonNotificationInfo BalloonInfo
        {
            get
            {
                return _balloonInfo;
            }
            set
            {
                _balloonInfo = value;
                NotifyOfPropertyChange(() => BalloonInfo);
            }
        }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public string IconSource
        {
            get
            {
                return _iconSource;
            }
            set
            {
                _iconSource = value;
                NotifyOfPropertyChange(() => IconSource);
            }
        }

        public bool IsStopped { get; set; }

        public IOmniService OmniService { get; set; }

        public string TooltipText
        {
            get
            {
                return "Omnipaste " + ApplicationService.Version;
                
            }
        }

        public Visibility Visibility { get; set; }

        #endregion

        #region Public Methods and Operators
        
        public void Exit()
        {
            ApplicationService.ShutDown();
        }

        public void Show()
        {
            EventAggregator.PublishOnUIThread(new ShowShellMessage());
        }

        public void ShowBalloon(string balloonTitle, string balloonMessage)
        {
            BalloonInfo = new BalloonNotificationInfo { Title = balloonTitle, Message = balloonMessage };
        }

        public void ToggleAutoStart()
        {            
        }

        public void ToggleSync()
        {
            if (IsStopped)
            {
                OmniService.Stop();
            }
            else
            {
                OmniService.Start().Subscribe();
            }
        }

        #endregion
    }
}
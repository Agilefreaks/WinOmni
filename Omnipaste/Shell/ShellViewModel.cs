namespace Omnipaste.Shell
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Configuration;
    using Omnipaste.ContextMenu;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Properties;
    using Omnipaste.UserToken;

    public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShellViewModel
    {
        private Window _view;

        [Inject]
        public IWindowManager WindowManager { get; set; }

        [Inject]
        public IUserTokenViewModel UserTokenViewModel { get; set; }

        [Inject]
        public IContextMenuViewModel ContextMenuViewModel { get; set; }

        public IConfigurationViewModel ConfigurationViewModel { get; set; }

        public ShellViewModel(IConfigurationViewModel configurationViewModel, IEventAggregator eventAggregator)
        {
            ConfigurationViewModel = configurationViewModel;

            DisplayName = Resources.AplicationName;

            eventAggregator.Subscribe(this);
        }

        public void Handle(GetTokenFromUserMessage message)
        {
            ActiveItem = UserTokenViewModel;
        }

        public void Handle(TokenRequestResultMessage message)
        {
            ActiveItem = ConfigurationViewModel;
        }

        public void Handle(ConfigurationCompletedMessage message)
        {
            ActiveItem = ContextMenuViewModel;
            ContextMenuViewModel.Start();

            Hide();
        }

        public IntPtr GetHandle()
        {
            var handle = new IntPtr();
            Execute.OnUIThread(() =>
                {
                    var windowInteropHelper = new WindowInteropHelper(_view);
                    handle = windowInteropHelper.Handle;
                });
            
            return handle;
        }

        public void Hide()
        {
            if (_view != null)
            {
                _view.Visibility = Visibility.Hidden;
                _view.ShowInTaskbar = false;
            }
        }

        public void Show()
        {
            if (_view != null)
            {
                _view.Visibility = Visibility.Visible;
                _view.ShowInTaskbar = true;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (Window)view;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ActiveItem = ConfigurationViewModel;
            ConfigurationViewModel.Start();
        }
    }
}
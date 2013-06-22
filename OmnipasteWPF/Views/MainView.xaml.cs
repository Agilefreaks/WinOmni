namespace OmnipasteWPF.Views
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using OmnipasteWPF.ViewModels;
    using OmnipasteWPF.ViewModels.MainView;
    using WindowsImports;
    using ViewModelBase = Cinch.ViewModelBase;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            ViewModelBase.SetupVisualizer = x => x.Register("GetTokenFromUser", typeof(GetTokenFromUserView));
            DataContext = new MainViewModel(new MainViewModelIOCProvider());

            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HideWindowFromAltTab();
        }

        private void HideWindowFromAltTab()
        {
            var handle = new WindowInteropHelper(this).Handle;
            var extendedStyle = (int)User32.GetWindowLong(handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            extendedStyle |= ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WindowHelper.SetWindowLong(handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)extendedStyle);
        }
    }
}

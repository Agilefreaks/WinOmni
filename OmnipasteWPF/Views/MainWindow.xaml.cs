namespace OmnipasteWPF.Views
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using OmnipasteWPF.ViewModels;
    using WindowsImports;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();

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

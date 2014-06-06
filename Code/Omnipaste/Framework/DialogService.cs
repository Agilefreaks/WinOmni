namespace Omnipaste.Framework
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Ninject;
    using Omnipaste.Dialog;

    public class DialogService : IDialogService
    {
        private BaseMetroDialog _dialog;

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        public MetroWindow MainWindow
        {
            get
            {
                return Application.Current.Windows.OfType<MetroWindow>().First();
            }
        }
        
        public void Start()
        {
            DialogViewModel.ActivationProcessed += OnDialogViewModelActivationProcessed;
            DialogViewModel.Closed += OnDialogViewModelClosed;
        }

        public void Stop()
        {
            DialogViewModel.ActivationProcessed -= OnDialogViewModelActivationProcessed;
            DialogViewModel.Closed -= OnDialogViewModelClosed;
        }

        private void OnDialogViewModelActivationProcessed(object sender, ActivationProcessedEventArgs e)
        {
            var dialogViewModel = (IDialogViewModel)sender;

            if (dialogViewModel.IsOpen)
            {
                return;
            }

            Execute.OnUIThread(async () =>
            {
                _dialog = _dialog ?? ViewLocator.GetViewForViewModel(dialogViewModel) as BaseMetroDialog;
                await MainWindow.ShowMetroDialogAsync(_dialog);
                DialogViewModel.IsOpen = true;
            });
        }

        private async void OnDialogViewModelClosed(object sender, DialogClosedEventArgs args)
        {
            var dialogViewModel = (IDialogViewModel)sender;
            
            await Execute.OnUIThreadAsync(async () =>
            {
                await MainWindow.HideMetroDialogAsync(_dialog);
                dialogViewModel.IsOpen = false;
            });
                
        }
    }
}
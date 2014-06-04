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

        private object _view;

        public async Task ShowDialog(IScreen viewModel)
        {
            if (_dialog == null)
            {
                _view = ViewLocator.GetViewForViewModel(DialogViewModel);
                _dialog = _view as BaseMetroDialog;
            }

            DialogViewModel.ActivateItem(viewModel);

            if (!DialogViewModel.IsOpen)
            {
                await MainWindow.ShowMetroDialogAsync(_dialog);
                DialogViewModel.IsOpen = true;
            }
        }

        public async Task CloseDialog()
        {
            await MainWindow.HideMetroDialogAsync(_dialog);
        }
    }
}
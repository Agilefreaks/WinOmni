namespace Omnipaste.Framework
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Omnipaste.Dialog;

    public class DialogService : IDialogService
    {
        private BaseMetroDialog _dialog;

        private IDialogViewModel _dialogViewModel;

        private object _view;

        public async Task ShowDialog(IScreen viewModel)
        {
            if (_dialogViewModel == null)
            {
                _dialogViewModel = IoC.Get<IDialogViewModel>();
            }

            if (_dialog == null)
            {
                _view = ViewLocator.GetViewForViewModel(_dialogViewModel);
                _dialog = _view as BaseMetroDialog;
            }

            if (_dialog == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        "The view {0} belonging to view model {1} does not inherit from {2}",
                        _view.GetType(),
                        viewModel.GetType(),
                        typeof(BaseMetroDialog)));
            }

            var shellView = Application.Current.Windows.OfType<MetroWindow>().First();
            viewModel.Deactivated += ViewModelDeactivated;
            _dialogViewModel.ActivateItem(viewModel);

            if (!_dialogViewModel.IsOpen)
            {
                await shellView.ShowMetroDialogAsync(_dialog);
                _dialogViewModel.IsOpen = true;
            }
        }

        private void ViewModelDeactivated(object sender, DeactivationEventArgs e)
        {
            var screen = (IDeactivate)sender;
            screen.Deactivated -= ViewModelDeactivated;
            _dialogViewModel.DeactivateItem(screen, true);
            var firstMetroWindow = Application.Current.Windows.OfType<MetroWindow>().First();

            Execute.OnUIThread(async () =>
            {
                await firstMetroWindow.HideMetroDialogAsync(_dialog);
                _dialogViewModel.IsOpen = false;
            });
        }
    }
}
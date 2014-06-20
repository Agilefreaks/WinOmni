namespace Omnipaste.Framework
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Omnipaste.Dialog;

    public class DialogService : IDialogService
    {
        #region Fields

        private BaseMetroDialog _dialog;

        private IDialogViewModel _dialogViewModel;

        private readonly AutoResetEvent _dialogOpened = new AutoResetEvent(false);

        #endregion

        #region Constructors and Destructors

        public DialogService(IDialogViewModel dialogViewModel)
        {
            DialogViewModel = dialogViewModel;
        }

        #endregion

        #region Public Properties

        public IDialogViewModel DialogViewModel
        {
            get
            {
                return _dialogViewModel;
            }
            set
            {
                if (_dialogViewModel != null)
                {
                    Unsubscribe();
                }

                _dialogViewModel = value;

                Subscribe();
            }
        }

        public MetroWindow MainWindow
        {
            get
            {
                return Application.Current.Windows.OfType<MetroWindow>().First();
            }
        }

        public bool IsBusyOpeningDialog { get; set; }

        #endregion

        #region Methods

        private async void OnCloseDialog(object sender, EventArgs args)
        {
            if (!IsBusyOpeningDialog)
            {
                await MainWindow.HideMetroDialogAsync(_dialog);
            }
            else
            {
                //this happens when the dialog is closed almost instantly after the 
                // it was required to be opened. Since they are async, and come from 
                // different event handlers, the close action will finish before 
                // the open action.
                var backroundWorker = new BackgroundWorker();
                backroundWorker.DoWork += delegate
                {
                    //go on a separate thread and wait for the Dialog to be shown
                    _dialogOpened.WaitOne();
                    //go back
                    Execute.OnUIThread(
                        async () =>
                        {
                            await MainWindow.HideMetroDialogAsync(_dialog);
                        });
                };

                backroundWorker.RunWorkerAsync();
            }
        }

        private async void OnShowDialog(object sender, ActivationProcessedEventArgs e)
        {
            IsBusyOpeningDialog = true;
            
            var dialogViewModel = (IDialogViewModel)sender;
            
            _dialog = _dialog ?? ViewLocator.GetViewForViewModel(dialogViewModel) as BaseMetroDialog;

            await MainWindow.ShowMetroDialogAsync(_dialog);
            
            IsBusyOpeningDialog = false;
            _dialogOpened.Set();
        }

        private void Subscribe()
        {
            _dialogViewModel.ActivationProcessed += OnShowDialog;
            _dialogViewModel.Closed += OnCloseDialog;
        }

        private void Unsubscribe()
        {
            _dialogViewModel.ActivationProcessed -= OnShowDialog;
            _dialogViewModel.Closed -= OnCloseDialog;
        }

        #endregion
    }
}
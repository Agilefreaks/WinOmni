using System;
using Caliburn.Micro;
using WebSocket4Net.Command;

namespace Omnipaste.Dialog
{
    public class DialogClosedEventArgs : EventArgs
    {
        public object ClosedItem { get; set; }
    }

    public class DialogViewModel : Conductor<IScreen>, IDialogViewModel
    {
        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        public event EventHandler<DialogClosedEventArgs> Closed;

        private IScreen _content;

        public IScreen Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                ActivateItem(_content);
            }
        }

        public void ActivateItem(object item)
        {
            ActiveItem = item as IScreen;

            var child = ActiveItem as IChild;
            if (child != null)
                child.Parent = this;

            if (ActiveItem != null)
                ActiveItem.Activate();

            NotifyOfPropertyChange(() => ActiveItem);
            ActivationProcessed(this, new ActivationProcessedEventArgs { Item = ActiveItem, Success = true });
        }

        public void DeactivateItem(object item, bool close)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(result =>
                {
                    if (result)
                        CloseActiveItemCore();
                });
            }
            else
            {
                CloseActiveItemCore();
            }
        }

        private void CloseActiveItemCore()
        {
            var oldItem = ActiveItem;
            ActivateItem(null);
            oldItem.Deactivate(true);
            OnClosed(oldItem);
        }

        private void OnClosed(object closedItem)
        {
            if (Closed != null)
            {
                Closed(this, new DialogClosedEventArgs
                {
                    ClosedItem = closedItem
                });
            }
        }
    }
}
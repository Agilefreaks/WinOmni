namespace Omnipaste.Workspaces
{
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.Micro;
    using OmniUI.Workspace;

    public class DetailsConductorViewModel : Conductor<IScreen>, IDetailsConductorViewModel
    {
        #region Public Properties

        public Point AnimationOriginPoint
        {
            get
            {
                var result = new Point();
                var frameworkElement = Application.Current.MainWindow;
                if (frameworkElement != null)
                {
                    var mousePosition = Mouse.GetPosition(frameworkElement);
                    result.Y = mousePosition.Y / (frameworkElement.ActualHeight);
                    result.X = 0;
                }

                return result;
            }
        }

        public override void ActivateItem(IScreen item)
        {
            NotifyOfPropertyChange(() => AnimationOriginPoint);
            base.ActivateItem(item);
        }

        #endregion
    }
}
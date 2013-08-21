namespace Omnipaste.History
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;

    public interface IHistoryViewModel : IWorkspace, IHandle<IClipboardData>
    {
        ObservableCollection<string> RecentClippings { get; }
    }
}

using System.Collections.ObjectModel;
using Caliburn.Micro;
using OmniCommon.Interfaces;
using Omnipaste.Framework;

namespace Omnipaste.History
{
    public interface IHistoryViewModel : IWorkspace, IHandle<IClipboardData>
    {
        ObservableCollection<string> Clippings { get; }
    }
}

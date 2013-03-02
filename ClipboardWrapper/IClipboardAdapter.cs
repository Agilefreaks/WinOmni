using System.Windows.Forms;

namespace ClipboardWrapper
{
    public interface IClipboardAdapter
    {
        IDataObject GetDataObject();
    }
}
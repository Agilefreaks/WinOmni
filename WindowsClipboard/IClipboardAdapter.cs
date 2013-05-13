using System.Windows.Forms;

namespace WindowsClipboard
{
    public interface IClipboardAdapter
    {
        IDataObject GetDataObject();

        void SetData(string data);
    }
}
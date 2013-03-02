using System.Windows.Forms;

namespace ClipboardWrapper
{
    public class ClipboardAdapter : IClipboardAdapter
    {
        public IDataObject GetDataObject()
        {
            return Clipboard.GetDataObject();
        }
    }
}
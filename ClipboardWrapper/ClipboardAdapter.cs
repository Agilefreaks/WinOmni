using System.Windows.Forms;

namespace ClipboardWrapper
{
    public class ClipboardAdapter : IClipboardAdapter
    {
        public IDataObject GetDataObject()
        {
            return Clipboard.GetDataObject();
        }

        public void SetData(string data)
        {
            Clipboard.SetData(DataFormats.Text, data);
        }
    }
}
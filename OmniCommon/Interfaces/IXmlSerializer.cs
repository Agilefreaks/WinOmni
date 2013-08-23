namespace OmniCommon.Interfaces
{
    using System.IO;

    public interface IXmlSerializer
    {
        T Deserialize<T>(Stream stream);

        void Serialize(Stream stream, object instance);
    }
}
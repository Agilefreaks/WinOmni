namespace OmniCommon.Interfaces
{
    using System.IO;
    using System.Xml.Serialization;

    public class SystemXmlSerializer : IXmlSerializer
    {
        public T Deserialize<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(stream);
        }

        public void Serialize(Stream stream, object instance)
        {
            if (instance == null)
            {
                return;
            }

            var serializer = new XmlSerializer(instance.GetType());
            serializer.Serialize(stream, instance);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }
}
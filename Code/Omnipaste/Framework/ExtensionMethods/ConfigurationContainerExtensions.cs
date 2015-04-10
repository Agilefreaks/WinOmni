namespace Omnipaste.Framework.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using OmniCommon.Helpers;
    using OmniCommon.Settings;

    public static class ConfigurationContainerExtensions
    {
        public static TValue GetObject<TValue>(this IConfigurationContainer configurationContainer, string key)
            where TValue : class, new()
        {
            TValue deserializedObject = null;

            try
            {
                var serializedData = configurationContainer.GetValue(key);
                if (!string.IsNullOrWhiteSpace(serializedData))
                {
                    var xmlSerializer = new XmlSerializer(typeof(TValue));
                    var stringReader = new StringReader(serializedData);
                    var xmlReader = XmlReader.Create(stringReader);
                    if (xmlSerializer.CanDeserialize(xmlReader))
                    {
                        deserializedObject = (TValue)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }

            deserializedObject = deserializedObject ?? new TValue();

            return deserializedObject;
        }

        public static void SetObject<TValue>(this IConfigurationContainer configurationContainer, string key, TValue data)
            where TValue : class, new()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(TValue));
                var stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, data);

                configurationContainer.SetValue(key, stringWriter.ToString());
            }
            catch (Exception exception)
            {
                ExceptionReporter.Instance.Report(exception);
            }
        }
    }
}

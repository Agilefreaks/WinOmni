using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ClipboardWatcher.Core.Services
{
    public class DPAPIConfigurationProvider : IConfigurationProvider
    {
        private const string FileName = "settings.cfg";
        private readonly byte[] _entropy = Encoding.UTF8.GetBytes("ExtraEntropyToBeMoreSafe");

        public string SettingsFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmniPaste"); }
        }

        public string FullSettingsFilePath
        {
            get
            {
                return Path.Combine(SettingsFolder, FileName);
            }
        }

        public string GetValue(string key)
        {
            string value = null;
            try
            {
                if (File.Exists(FullSettingsFilePath))
                {
                    var xDocument = LoadData();
                    var element = GetElementForKey(xDocument, key);
                    value = element.Descendants("Value").First().Value;
                }
            }
            catch (Exception)
            {
                value = null;
            }

            return value;
        }

        public bool SetValue(string key, string value)
        {
            bool saved;
            try
            {
                var document = File.Exists(FullSettingsFilePath)
                                         ? UpdateExistingDocument(key, value)
                                         : InitializeNewSettingsDocument(key, value);
                SaveToFile(GetProtectedData(document));
                saved = true;
            }
            catch (Exception)
            {
                saved = false;
            }

            return saved;
        }

        private byte[] GetProtectedData(XDocument document)
        {
            byte[] protectedData;
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    document.Save(xmlWriter);
                }

                memoryStream.Flush();
                protectedData = ProtectedData.Protect(memoryStream.ToArray(), _entropy,
                                                      DataProtectionScope.CurrentUser);
            }

            return protectedData;
        }

        private void SaveToFile(byte[] protectedData)
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            using (var file = File.Open(FullSettingsFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.Write(protectedData, 0, protectedData.Count());
            }
        }

        private static XDocument InitializeNewSettingsDocument(string key, string value)
        {
            var xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var root = new XElement("Settings");
            xDocument.Add(root);
            root.Add(new XElement("Entry", new XElement("Name", key), new XElement("Value", value)));

            return xDocument;
        }

        private static XElement GetElementForKey(XNode xDocument, string key)
        {
            return xDocument.XPathSelectElement(string.Format("/Settings/Entry[Name='{0}']", key));
        }

        private XDocument LoadData()
        {
            var readAllBytes = File.ReadAllBytes(FullSettingsFilePath);
            var data = ProtectedData.Unprotect(readAllBytes, _entropy, DataProtectionScope.CurrentUser);
            XDocument xDocument;
            using (var memoryStream = new MemoryStream(data))
            {
                xDocument = XDocument.Load(XmlReader.Create(memoryStream));
            }

            return xDocument;
        }

        private XDocument UpdateExistingDocument(string key, string value)
        {
            var xDocument = LoadData();
            var element = GetElementForKey(xDocument, key);
            if (element != null)
            {
                element.Descendants("Value").First().Value = value;
            }
            else
            {
                Debug.Assert(xDocument.Root != null, "The configuration file should have a root");
                xDocument.Root.Add(new XElement("Entry", new XElement("Name", key), new XElement("Value", value)));
            }

            return xDocument;
        }
    }
}

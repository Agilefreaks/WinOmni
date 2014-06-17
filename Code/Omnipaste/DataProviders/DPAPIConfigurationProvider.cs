namespace Omnipaste.DataProviders
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using OmniCommon.DataProviders;
    using OmniCommon.ExtensionMethods;

    public class DPAPIConfigurationProvider : IConfigurationProvider
    {
        #region Constants

        private const string FileName = "settings.cfg";

        #endregion

        #region Fields

        private readonly byte[] _entropy = Encoding.UTF8.GetBytes("ExtraEntropyToBeMoreSafe");

        private string _settingFilePath;

        private string _settingsFolder;

        #endregion

        #region Public Properties

        public string FullSettingsFilePath
        {
            get
            {
                return _settingFilePath ?? (_settingFilePath = Path.Combine(SettingsFolder, FileName));
            }
        }

        public string SettingsFolder
        {
            get
            {
                return _settingsFolder ?? (_settingsFolder = GetSettingsFolder());
            }
        }

        #endregion

        #region Public Indexers

        public string this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
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
            catch (Exception exception)
            {
                value = null;
                this.Log(exception);
            }

            return value;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            var value = GetValue(key);

            return value != null ? (T)(object)value : defaultValue;
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
            catch (Exception exception)
            {
                saved = false;
                this.Log(exception);
            }

            return saved;
        }

        #endregion

        #region Methods

        private static XElement GetElementForKey(XNode document, string key)
        {
            return document.XPathSelectElement(string.Format("/Settings/Entry[Name='{0}']", key));
        }

        private static string GetSettingsFolder()
        {
            return
                Path.Combine(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        ApplicationInfoFactory.PublisherName),
                    ApplicationInfoFactory.ApplicationName);
        }

        private static XDocument InitializeNewSettingsDocument(string key, string value)
        {
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var root = new XElement("Settings");
            document.Add(root);
            root.Add(new XElement("Entry", new XElement("Name", key), new XElement("Value", value)));

            return document;
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
                protectedData = ProtectedData.Protect(memoryStream.ToArray(), _entropy, DataProtectionScope.CurrentUser);
            }

            return protectedData;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
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
                Debug.Assert(xDocument.Root != null, "xDocument.Root != null");
                xDocument.Root.Add(new XElement("Entry", new XElement("Name", key), new XElement("Value", value)));
            }

            return xDocument;
        }

        #endregion
    }
}
namespace OmniCommon.Services
{
    using System;
    using System.Linq;
    using System.IO;
    using OmniCommon.Interfaces;
    using System.Collections.Generic;
    using OmniCommon.Domain;

    public class XmlClippingRepository : IClippingRepository
    {
        private List<Clipping> _allClippings; 

        public IFileService FileService { get; set; }

        public IXmlSerializer Serializer { get; set; }

        public IDateTimeService DateTimeService { get; set; }

        public string FilePath
        {
            get
            {
                return Path.Combine(FileService.AppDataDir, "clippings.xml");
            }
        }

        private static readonly object Lock = new object();

        public XmlClippingRepository(IFileService fileService, IXmlSerializer serializer, IDateTimeService dateTimeService)
        {
            FileService = fileService;
            Serializer = serializer;
            DateTimeService = dateTimeService;
        }

        public IList<Clipping> GetForLast24Hours()
        {
            var now = DateTimeService.UtcNow;
            return GetAll()
                .Where(c => c.DateCreated > now.Subtract(TimeSpan.FromHours(24)))
                .OrderByDescending(c => c.DateCreated)
                .ToList();
        }

        public void Save(Clipping clip)
        {
            clip.DateCreated = DateTimeService.UtcNow;

            var items = GetAll();
            items.Add(clip);

            Save(items);
        }

        private IList<Clipping> GetAll()
        {
            if (_allClippings != null)
            {
                return _allClippings;
            }

            lock (Lock)
            {
                Stream stream = null;
                try
                {
                    if (!FileService.Exists(FilePath))
                    {
                        stream = FileService.Open(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        Serializer.Serialize(stream, new List<Clipping>());
                    }
                    else
                    {
                        stream = FileService.Open(FilePath, FileMode.Open, FileAccess.Read);
                    }

                    _allClippings = Serializer.Deserialize<List<Clipping>>(stream);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }

            }


            return _allClippings;
        }

        private void Save(IList<Clipping> clippings)
        {
            lock (Lock)
            {
                Stream stream = null;
                try
                {
                    stream = FileService.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Write);

                    Serializer.Serialize(stream, clippings);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
            }
        }
    }
}

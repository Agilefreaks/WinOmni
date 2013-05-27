﻿namespace OmniCommon.Services
{
    using OmniCommon.Interfaces;

    public class ClipboardData : IClipboardData
    {
        private readonly object _sender;
        private readonly string _data;

        public ClipboardData(object sender, string data)
        {
            _sender = sender;
            _data = data;
        }

        public object GetSender()
        {
            return _sender;
        }

        public string GetData()
        {
            return _data;
        }
    }
}
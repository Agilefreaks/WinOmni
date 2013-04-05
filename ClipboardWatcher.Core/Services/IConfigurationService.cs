﻿namespace ClipboardWatcher.Core.Services
{
    public interface IConfigurationService : IStartupTask
    {
        CommunicationSettings CommunicationSettings { get; }

        void LoadCommunicationSettings();
    }
}
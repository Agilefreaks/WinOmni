namespace Omnipaste.Shell.SessionInfo
{
    using System.Collections.Generic;
    using Properties;

    public enum ConnectionStateEnum
    {
        Connected,

        Disconnected
    }

    public static class ConnectionStateEnumExtensions
    {
        private static readonly Dictionary<ConnectionStateEnum, string> StatusTexts =
            new Dictionary<ConnectionStateEnum, string>
                {
                    { ConnectionStateEnum.Connected, Resources.Connected },
                    { ConnectionStateEnum.Disconnected, Resources.Disconnected }
                };

        public static string ToResourceString(this ConnectionStateEnum connectionState)
        {
            return StatusTexts[connectionState];
        }
    }
}
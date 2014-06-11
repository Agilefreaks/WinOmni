namespace OmniSync
{
    using System;

    public class ServiceStatusEventArgs : EventArgs
    {
        public ServiceStatusEnum Status { get; set; }

        public ServiceStatusEventArgs(ServiceStatusEnum status)
        {
            Status = status;
        }
    }
}
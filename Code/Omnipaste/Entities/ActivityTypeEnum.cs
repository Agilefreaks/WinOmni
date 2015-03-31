namespace Omnipaste.Entities
{
    using System;

    [Flags]
    public enum ActivityTypeEnum
    {
        None = 0,

        Clipping = 1,

        Call = 2,

        Message = 4,

        Version = 8,

        All = Call | Clipping | Message | Version
    }
}
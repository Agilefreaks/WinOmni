namespace Omnipaste.Activity
{
    using System;

    [Flags]
    public enum ActivityTypeEnum
    {
        None = 0,

        Clipping = 1,

        Call = 2,

        Message = 4,

        All = Call | Clipping | Message
    }
}
namespace Omnipaste.ClippingList
{
    using System;

    [Flags]
    public enum ClippingFilterTypeEnum
    {
        None = 0,

        Local = 1,

        Cloud = 2,

        All = Local | Cloud
    }
}

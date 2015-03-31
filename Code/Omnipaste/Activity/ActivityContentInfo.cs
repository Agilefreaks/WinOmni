namespace Omnipaste.Activity
{
    using System;

    public enum ContentStateEnum
    {
        Viewing,

        Viewed,

        NotViewed
    }

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

    public class ActivityContentInfo
    {
        public ContentStateEnum ContentState { get; set; }

        public ActivityTypeEnum ContentType { get; set; }
    }
}
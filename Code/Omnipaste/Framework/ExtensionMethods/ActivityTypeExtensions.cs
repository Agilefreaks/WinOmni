namespace Omnipaste.Framework.ExtensionMethods
{
    using Omnipaste.Activities.ActivityList.Activity;

    public static class ActivityTypeExtensions
    {
        const string CallBrush = "CallBrush";

        const string ClippingBrush = "ClippingBrush";

        const string MessageBrush = "MessageBrush";

        const string VersionBrush = "VersionBrush";
        
        public static string GetBrushName(this ActivityTypeEnum activityType)
        {
            string brushName = null;

            switch (activityType)
            {
                case ActivityTypeEnum.Clipping:
                    brushName = ClippingBrush;
                    break;
                case ActivityTypeEnum.Message:
                    brushName = MessageBrush;
                    break;
                case ActivityTypeEnum.Call:
                    brushName = CallBrush;
                    break;
                case ActivityTypeEnum.Version:
                    brushName = VersionBrush;
                    break;
            }

            return brushName;
        }
    }
}

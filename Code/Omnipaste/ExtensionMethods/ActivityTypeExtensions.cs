namespace Omnipaste.ExtensionMethods
{
    using Omnipaste.Models;

    public static class ActivityTypeExtensions
    {
        public static string GetBrushName(this ActivityTypeEnum activityType)
        {
            const string CallBrush = "CallBrush";

            const string ClippingBrush = "ClippingBrush";

            const string MessageBrush = "MessageBrush";

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
            }

            return brushName;
        }
    }
}

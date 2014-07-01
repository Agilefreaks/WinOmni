namespace OmniApi.Support.Converters
{
    using System;
    using Newtonsoft.Json;

    public class SnakeCaseStringEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string finalValue = reader.Value.ToString().Replace("_", string.Empty);
            return Enum.Parse(objectType, finalValue, true);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
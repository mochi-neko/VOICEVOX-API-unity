#nullable enable
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API
{
    [JsonObject]
    internal sealed class APIError
    {
        [JsonProperty("detail"), JsonRequired] 
        public ValidationError[] Detail { get; private set; }

        [JsonObject]
        public sealed class ValidationError
        {
            [JsonProperty("loc"), JsonRequired] 
            public string[] Location { get; private set; }

            [JsonProperty("msg"), JsonRequired] 
            public string Message { get; private set; }

            [JsonProperty("type"), JsonRequired] 
            public string ErrorType { get; private set; }
        }

        public string ToJson()
            => JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

        public static APIError? FromJson(string json)
            => JsonConvert.DeserializeObject<APIError>(json);
    }
}
#nullable enable
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// NOTICE: Does not respond error from VOICEVOX Core ver 0.14.3.
    /// </summary>
    [JsonObject]
    internal sealed class APIErrorResponseBody
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

        public static APIErrorResponseBody? FromJson(string json)
            => JsonConvert.DeserializeObject<APIErrorResponseBody>(json);
    }
}
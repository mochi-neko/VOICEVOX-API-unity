#nullable enable
using System;
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// NOTICE: Does not respond error from VOICEVOX Core ver 0.14.3.
    /// </summary>
    [JsonObject]
    internal sealed class HTTPValidationError
    {
        [JsonProperty("detail"), JsonRequired]
        public ValidationError[] Detail { get; private set; } = Array.Empty<ValidationError>();

        [JsonObject]
        public sealed class ValidationError
        {
            [JsonProperty("loc"), JsonRequired]
            public string[] Location { get; private set; } = Array.Empty<string>();

            [JsonProperty("msg"), JsonRequired] 
            public string Message { get; private set; } = string.Empty;

            [JsonProperty("type"), JsonRequired] 
            public string ErrorType { get; private set; } = string.Empty;
        }

        public string ToJson()
            => JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

        public static HTTPValidationError? FromJson(string json)
            => JsonConvert.DeserializeObject<HTTPValidationError>(json);
    }
}
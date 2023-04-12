#nullable enable
namespace Mochineko.VOICEVOX_API
{
    /// <summary>
    /// Shares base URL of a VOICEVOX API server.
    /// </summary>
    public static class VoiceVoxBaseURL
    {
        private const string DefaultBaseURL = "http://127.0.0.1:50021";
        public static string BaseURL { get; set; } = DefaultBaseURL;
    }
}
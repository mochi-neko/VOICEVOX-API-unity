#nullable enable
namespace Mochineko.VOICEVOX_API
{
    public static class VoiceVoxBaseURL
    {
        private const string DefaultBaseURL = "http://localhost:50021";
        public static string BaseURL { get; set; } = DefaultBaseURL;
    }
}
#nullable enable
using System;
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API.QueryCreation
{
    /// <summary>
    /// Audio query created from text and used to synthesis.
    /// </summary>
    [JsonObject]
    public sealed class AudioQuery
    {
        [JsonProperty("accent_phrases"), JsonRequired]
        public AccentPhase[] AccentPhases { get; private set; } = Array.Empty<AccentPhase>();

        [JsonProperty("speedScale"), JsonRequired]
        public float SpeedScale { get; private set; }

        [JsonProperty("pitchScale"), JsonRequired]
        public float PitchScale { get; private set; }

        [JsonProperty("intonationScale"), JsonRequired]
        public float IntonationScale { get; private set; }

        [JsonProperty("volumeScale"), JsonRequired]
        public float VolumeScale { get; private set; }

        [JsonProperty("prePhonemeLength"), JsonRequired]
        public float PrePhonemeLength { get; private set; }

        [JsonProperty("postPhonemeLength"), JsonRequired]
        public float PostPhonemeLength { get; private set; }

        [JsonProperty("outputSamplingRate"), JsonRequired]
        public int OutputSamplingRate { get; private set; }

        [JsonProperty("outputStereo"), JsonRequired]
        public bool OutputStereo { get; private set; }

        [JsonProperty("kana")] 
        public string? Kana { get; private set; }

        [JsonObject]
        public sealed class AccentPhase
        {
            [JsonProperty("moras"), JsonRequired] 
            public Mora[] Moras { get; private set; }

            [JsonProperty("accent"), JsonRequired] 
            public int Accent { get; private set; }

            [JsonProperty("pause_mora")] 
            public Mora? PauseMora { get; private set; }

            [JsonProperty("is_interrogative")] // Defaults to false
            public bool? IsInterrogative { get; private set; }
            
            public AccentPhase(Mora[] moras, int accent)
            {
                Moras = moras;
                Accent = accent;
            }

            [JsonObject]
            public sealed class Mora
            {
                [JsonProperty("text"), JsonRequired]
                public string Text { get; private set; } = string.Empty;

                [JsonProperty("consonant")] 
                public string? Consonant { get; private set; }

                [JsonProperty("consonant_length")]
                public float? ConsonantLength { get; private set; }

                [JsonProperty("vowel"), JsonRequired]
                public string Vowel { get; private set; } = string.Empty;

                [JsonProperty("vowel_length"), JsonRequired]
                public float VowelLength { get; private set; }

                [JsonProperty("pitch"), JsonRequired]
                public float Pitch { get; private set; }
            }
        }
        
        public string ToJson()
            => JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

        public static AudioQuery? FromJson(string json)
            => JsonConvert.DeserializeObject<AudioQuery>(json);
    }
}
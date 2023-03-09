#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mochineko.VOICEVOX_API.QueryCreation
{
    public sealed class VoiceVoxQueryCreation
    {
        private const string AudioQueryEndPoint = "/audio_query";
        private const string AudioQueryFromPreset = "/audio_query_from_preset";

        private static HttpClient HttpClient
            => HttpClientPool.PooledClient;

        public static async Task<AudioQuery> CreateQueryAsync(
            string text,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("text", text));
            queryParameters.Add(("speaker", speaker.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] HttpResponseMessage is null.");
            }

            var responseText = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseText))
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] Response JSON is null or empty.");
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var audioQuery = AudioQuery.FromJson(responseText);
                if (audioQuery != null)
                {
                    return audioQuery;
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.QueryCreation] Response AudioQuery is null.");
                }
            }
            else
            {
                var errorResponse = APIError.FromJson(responseText);
                if (errorResponse != null)
                {
                    throw new APIException(errorResponse);
                }
                else
                {
                    responseMessage.EnsureSuccessStatusCode();
                
                    throw new Exception($"[VOICEVOX_API.QueryCreation] System error.");   
                }
            }
        }
        
        public static async Task<AudioQuery> CreateQueryFromPresetAsync(
            string text,
            int presetId,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("text", text));
            queryParameters.Add(("preset_id", presetId.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryFromPreset
                      + QueryParametersBuilder.Build(queryParameters);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] HttpResponseMessage is null.");
            }

            var responseText = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseText))
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] Response JSON is null or empty.");
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var audioQuery = AudioQuery.FromJson(responseText);
                if (audioQuery != null)
                {
                    return audioQuery;
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.QueryCreation] Response AudioQuery is null.");
                }
            }
            else
            {
                // TODO: Handles error response
                
                responseMessage.EnsureSuccessStatusCode();
                
                throw new Exception($"[VOICEVOX_API.QueryCreation] System error.");
            }
        }
    }
}
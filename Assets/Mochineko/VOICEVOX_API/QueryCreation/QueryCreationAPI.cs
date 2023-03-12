#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API.QueryCreation
{
    /// <summary>
    /// VOICEVOX query creation API.
    /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90
    /// </summary>
    public static class QueryCreationAPI
    {
        private const string AudioQueryEndPoint = "/audio_query";
        private const string AudioQueryFromPreset = "/audio_query_from_preset";

        private static HttpClient HttpClient
            => HttpClientPool.PooledClient;

        /// <summary>
        /// Creates an audio query.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90/operation/audio_query_audio_query_post
        /// </summary>
        /// <param name="text">[Required] Text input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio query from text</returns>
        /// <exception cref="Exception">System errors</exception>
        /// <exception cref="APIException">VOICEVOX API errors</exception>
        /// <exception cref="OperationCanceledException">Cancelled before operation</exception>
        /// <exception cref="HttpRequestException">Request errors</exception>
        /// <exception cref="TaskCanceledException">Cancelled by user or timeout</exception>
        /// <exception cref="JsonSerializationException">JSON error</exception>
        public static async Task<AudioQuery> CreateQueryAsync(
            string text,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Build query parameters
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("text", text));
            queryParameters.Add(("speaker", speaker.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            // Send request and receive response
            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] HttpResponseMessage is null.");
            }

            // Decode response
            var responseText = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseText))
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] Response JSON is null or empty.");
            }

            // Succeeded
            if (responseMessage.StatusCode == HttpStatusCode.OK)
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
            // Failed
            else
            {
                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw new APIException(responseText, responseMessage.StatusCode, e);
                }
                
                throw new Exception($"[VOICEVOX_API.QueryCreation] System error.");
            }
        }
        
        /// <summary>
        /// Creates an audio query from a preset.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90/operation/audio_query_from_preset_audio_query_from_preset_post
        /// </summary>
        /// <param name="text">[Required] Text input</param>
        /// <param name="presetId">[Required] Preset ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio query from text</returns>
        /// <exception cref="Exception">System errors</exception>
        /// <exception cref="APIException">VOICEVOX API errors</exception>
        /// <exception cref="OperationCanceledException">Cancelled before operation</exception>
        /// <exception cref="HttpRequestException">Request errors</exception>
        /// <exception cref="TaskCanceledException">Cancelled by user or timeout</exception>
        /// <exception cref="JsonSerializationException">JSON error</exception>
        public static async Task<AudioQuery> CreateQueryFromPresetAsync(
            string text,
            int presetId,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Build query parameters
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("text", text));
            queryParameters.Add(("preset_id", presetId.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryFromPreset
                      + QueryParametersBuilder.Build(queryParameters);

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            // Send request and receive response
            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] HttpResponseMessage is null.");
            }

            // Decode response
            var responseText = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseText))
            {
                throw new Exception($"[VOICEVOX_API.QueryCreation] Response JSON is null or empty.");
            }

            // Succeeded
            if (responseMessage.StatusCode == HttpStatusCode.OK)
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
            // Failed
            else
            {
                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw new APIException(responseText, responseMessage.StatusCode, e);
                }
                
                throw new Exception($"[VOICEVOX_API.QueryCreation] System error.");
            }
        }
    }
}
#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mochineko.VOICEVOX_API.QueryCreation;
using Newtonsoft.Json;

namespace Mochineko.VOICEVOX_API.Synthesis
{
    /// <summary>
    /// VOICEVOX speech synthesis API.
    /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90
    /// </summary>
    public static class SynthesisAPI
    {
        private const string SynthesisEndPoint = "/synthesis";
        private const string CancellableSynthesisEndPoint = "/cancellable_synthesis";

        private static HttpClient HttpClient
            => HttpClientPool.PooledClient;

        /// <summary>
        /// Synthesizes speech from audio query.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90/operation/synthesis_synthesis_post
        /// </summary>
        /// <param name="query">[Required] Audio query input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="enableInterrogativeUpspeak">[Optional](Defaults to false) Enable interrogative upspeak or not</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio stream as WAV format data</returns>
        /// <exception cref="Exception">System errors</exception>
        /// <exception cref="APIException">VOICEVOX API errors</exception>
        /// <exception cref="OperationCanceledException">Cancelled before operation</exception>
        /// <exception cref="HttpRequestException">Request errors</exception>
        /// <exception cref="TaskCanceledException">Cancelled by user or timeout</exception>
        /// <exception cref="JsonSerializationException">JSON error</exception>
        public static async Task<Stream> SynthesizeAsync(
            AudioQuery query,
            int speaker,
            bool? enableInterrogativeUpspeak,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Build query parameters
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("speaker", speaker.ToString()));
            if (enableInterrogativeUpspeak != null)
            {
                queryParameters.Add(("enable_interrogative_upspeak", enableInterrogativeUpspeak.ToString()));
            }
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + SynthesisEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: query.ToJson(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            // Send request and receive response
            var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.Synthesis] HttpResponseMessage is null.");
            }

            // Succeeded
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                if (responseStream != null)
                {
                    return responseStream;
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Response Stream is null.");
                }
            }
            // Failed
            else
            {
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (responseText == null)
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response text is null.");
                }

                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw new APIException(responseText, responseMessage.StatusCode, e);
                }

                throw new Exception($"[VOICEVOX_API.Synthesis] System error.");
            }
        }

        /// <summary>
        /// Synthesizes speech from audio query with cancellable API.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90/operation/cancellable_synthesis_cancellable_synthesis_post
        /// </summary>
        /// <param name="query">[Required] Audio query input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio stream as WAV format data</returns>
        /// <exception cref="Exception">System errors</exception>
        /// <exception cref="APIException">VOICEVOX API errors</exception>
        /// <exception cref="OperationCanceledException">Cancelled before operation</exception>
        /// <exception cref="HttpRequestException">Request errors</exception>
        /// <exception cref="TaskCanceledException">Cancelled by user or timeout</exception>
        /// <exception cref="JsonSerializationException">JSON error</exception>
        public static async Task<Stream> CancellableSynthesizeAsync(
            AudioQuery query,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Build query parameters
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("speaker", speaker.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + CancellableSynthesisEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: query.ToJson(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            // Send request and receive response
            var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.Synthesis] HttpResponseMessage is null.");
            }

            // Succeeded
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                if (responseStream != null)
                {
                    return responseStream;
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Response Stream is null.");
                }
            }
            // Failed
            else
            {
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (responseText == null)
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response text is null.");
                }

                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw new APIException(responseText, responseMessage.StatusCode, e);
                }

                throw new Exception($"[VOICEVOX_API.Synthesis] System error.");
            }
        }
    }
}
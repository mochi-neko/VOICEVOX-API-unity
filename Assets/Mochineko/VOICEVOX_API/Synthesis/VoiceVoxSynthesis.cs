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

namespace Mochineko.VOICEVOX_API.Synthesis
{
    public sealed class VoiceVoxSynthesis
    {
        private const string SynthesisEndPoint = "/synthesis";
        private const string CancellableSynthesisEndPoint = "/cancellable_synthesis";

        private static HttpClient HttpClient
            => HttpClientPool.PooledClient;

        public static async Task<Stream> SynthesisAsync(
            AudioQuery query,
            int speaker,
            bool? enableInterrogativeUpspeak,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
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

            var url = VoiceVoxBaseURL.BaseURL
                      + SynthesisEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: query.ToJson(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.Synthesis] HttpResponseMessage is null.");
            }

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
            else if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity) // Error
            {
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (responseText == null)
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response text is null.");
                }

                var errorResponse = APIError.FromJson(responseText);
                if (errorResponse != null)
                {
                    // Handle API error
                    throw new APIException(errorResponse);
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response JSON is null.");
                }
            }
            else // Undefined errors
            {
                responseMessage.EnsureSuccessStatusCode();

                throw new Exception($"[VOICEVOX_API.Synthesis] System error.");
            }
        }

        public static async Task<Stream> CancellableSynthesisAsync(
            AudioQuery query,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var queryParameters = new List<(string key, string value)>();
            queryParameters.Add(("speaker", speaker.ToString()));
            if (coreVersion != null)
            {
                queryParameters.Add(("core_version", coreVersion));
            }

            var url = VoiceVoxBaseURL.BaseURL
                      + SynthesisEndPoint
                      + QueryParametersBuilder.Build(queryParameters);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: query.ToJson(),
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            using var responseMessage = await HttpClient.SendAsync(requestMessage, cancellationToken);
            if (responseMessage == null)
            {
                throw new Exception($"[VOICEVOX_API.Synthesis] HttpResponseMessage is null.");
            }

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
            else if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (responseText == null)
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response text is null.");
                }

                var errorResponse = APIError.FromJson(responseText);
                if (errorResponse != null)
                {
                    // Handle API error
                    throw new APIException(errorResponse);
                }
                else
                {
                    throw new Exception($"[VOICEVOX_API.Synthesis] Error response JSON is null.");
                }
            }
            else // Undefined errors
            {
                responseMessage.EnsureSuccessStatusCode();

                throw new Exception($"[VOICEVOX_API.Synthesis] System error.");
            }
        }
    }
}
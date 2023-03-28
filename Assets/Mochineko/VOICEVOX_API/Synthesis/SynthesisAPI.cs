#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mochineko.Relent.Result;
using Mochineko.Relent.UncertainResult;
using Mochineko.VOICEVOX_API.QueryCreation;

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

        /// <summary>
        /// Synthesizes a speech from an audio query.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90/operation/synthesis_synthesis_post
        /// </summary>
        /// <param name="httpClient">HttpClient instance</param>
        /// <param name="audioQuery">[Required] Audio query input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="enableInterrogativeUpspeak">[Optional](Defaults to false) Enable interrogative upspeak or not</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio stream as WAV format data</returns>
        /// <exception cref="ResultPatternMatchException"></exception>
        public static async UniTask<IUncertainResult<Stream>> SynthesizeAsync(
            HttpClient httpClient,
            AudioQuery audioQuery,
            int speaker,
            bool? enableInterrogativeUpspeak,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return UncertainResultFactory.Retry<Stream>(
                    "Retryable because cancellation has been already requested.");
            }
            
            // Build query parameters
            var queryParameters = new Dictionary<string, string>();
            queryParameters["speaker"] = speaker.ToString();
            if (enableInterrogativeUpspeak != null)
            {
                queryParameters["enable_interrogative_upspeak"] = enableInterrogativeUpspeak.ToString();
            }
            if (coreVersion != null)
            {
                queryParameters["core_version"] = coreVersion;
            }
            using var query = new FormUrlEncodedContent(queryParameters);
            var queryPath = await query.ReadAsStringAsync();

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + SynthesisEndPoint
                      + "?" + queryPath;

            string audioQueryJson;
            var serializeResult = JsonSerializer.Serialize(audioQuery);
            if (serializeResult is ISuccessResult<string> serializeSuccess)
            {
                audioQueryJson = serializeSuccess.Result;
            }
            else if (serializeResult is IFailureResult<string> serializeFailure)
            {
                return UncertainResultFactory.Fail<Stream>(
                    $"Failed to serialize {nameof(AudioQuery)} because -> {serializeFailure.Message}");
            }
            else
            {
                throw new ResultPatternMatchException(nameof(serializeResult));
            }

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: audioQueryJson,
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            try
            {
                // Send request and receive response
                var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
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
                        return UncertainResultFactory.Succeed(responseStream);
                    }
                    else
                    {
                        return UncertainResultFactory.Fail<Stream>(
                            $"Failed because response stream is null.");
                    }
                }
                // Retryable
                else if (responseMessage.StatusCode is HttpStatusCode.TooManyRequests
                         || (int)responseMessage.StatusCode is >= 500 and <= 599)
                {
                    var responseText = await responseMessage.Content.ReadAsStringAsync();
                    return UncertainResultFactory.Retry<Stream>(
                        $"Retryable because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}.");
                }
                // Response error
                else
                {
                    var responseText = await responseMessage.Content.ReadAsStringAsync();
                    return UncertainResultFactory.Fail<Stream>(
                        $"Failed because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}."
                    );
                }
            }
            // Request error
            catch (HttpRequestException exception)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Retryable because {nameof(HttpRequestException)} was thrown during calling the API -> {exception}.");
            }
            // Task cancellation
            catch (TaskCanceledException exception)
                when (exception.CancellationToken == cancellationToken)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Failed because task was canceled by user during call to the API -> {exception}.");
            }
            // Operation cancellation 
            catch (OperationCanceledException exception)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Retryable because operation was cancelled during calling the API -> {exception}.");
            }
            // Unhandled error
            catch (Exception exception)
            {
                return UncertainResultFactory.Fail<Stream>(
                    $"Failed because an unhandled exception was thrown when calling the API -> {exception}.");
            }
        }

        /// <summary>
        /// Synthesizes speech from audio query with cancellable API.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90/operation/cancellable_synthesis_cancellable_synthesis_post
        /// </summary>
        /// <param name="httpClient">HttpClient instance</param>
        /// <param name="audioQuery">[Required] Audio query input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio stream as WAV format data</returns>
        /// <exception cref="ResultPatternMatchException"></exception>
        public static async UniTask<IUncertainResult<Stream>> CancellableSynthesizeAsync(
            HttpClient httpClient,
            AudioQuery audioQuery,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return UncertainResultFactory.Retry<Stream>(
                    "Retryable because cancellation has been already requested.");
            }
            
            // Build query parameters
            var queryParameters = new Dictionary<string, string>();
            queryParameters["speaker"] = speaker.ToString();
            if (coreVersion != null)
            {
                queryParameters["core_version"] = coreVersion;
            }
            using var query = new FormUrlEncodedContent(queryParameters);
            var queryPath = await query.ReadAsStringAsync();

            // Build URL
            var url = VoiceVoxBaseURL.BaseURL
                      + CancellableSynthesisEndPoint
                      + "?" + queryPath;

            string audioQueryJson;
            var serializeResult = JsonSerializer.Serialize(audioQuery);
            if (serializeResult is ISuccessResult<string> serializeSuccess)
            {
                audioQueryJson = serializeSuccess.Result;
            }
            else if (serializeResult is IFailureResult<string> serializeFailure)
            {
                return UncertainResultFactory.Fail<Stream>(
                    $"Failed to serialize {nameof(AudioQuery)} because -> {serializeFailure.Message}");
            }
            else
            {
                throw new ResultPatternMatchException(nameof(serializeResult));
            }

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new StringContent(
                content: audioQueryJson,
                encoding: Encoding.UTF8,
                mediaType: "application/json");
            requestMessage.Content = content;

            try
            {
                // Send request and receive response
                var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
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
                        return UncertainResultFactory.Succeed(responseStream);
                    }
                    else
                    {
                        return UncertainResultFactory.Fail<Stream>(
                            $"Failed because response stream is null.");
                    }
                }
                // Retryable
                else if (responseMessage.StatusCode is HttpStatusCode.TooManyRequests
                         || (int)responseMessage.StatusCode is >= 500 and <= 599)
                {
                    var responseText = await responseMessage.Content.ReadAsStringAsync();
                    return UncertainResultFactory.Retry<Stream>(
                        $"Retryable because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}.");
                }
                // Response error
                else
                {
                    var responseText = await responseMessage.Content.ReadAsStringAsync();
                    return UncertainResultFactory.Fail<Stream>(
                        $"Failed because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}."
                    );
                }
            }
            // Request error
            catch (HttpRequestException exception)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Retryable because {nameof(HttpRequestException)} was thrown during calling the API -> {exception}.");
            }
            // Task cancellation
            catch (TaskCanceledException exception)
                when (exception.CancellationToken == cancellationToken)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Failed because task was canceled by user during call to the API -> {exception}.");
            }
            // Operation cancellation 
            catch (OperationCanceledException exception)
            {
                return UncertainResultFactory.Retry<Stream>(
                    $"Retryable because operation was cancelled during calling the API -> {exception}.");
            }
            // Unhandled error
            catch (Exception exception)
            {
                return UncertainResultFactory.Fail<Stream>(
                    $"Failed because an unhandled exception was thrown when calling the API -> {exception}.");
            }
        }
    }
}
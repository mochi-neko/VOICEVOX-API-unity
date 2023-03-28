#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mochineko.Relent.Result;
using Mochineko.Relent.UncertainResult;
using UnityEngine;

namespace Mochineko.VOICEVOX_API.QueryCreation
{
    /// <summary>
    /// VOICEVOX query creation API.
    /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90
    /// </summary>
    public static class QueryCreationAPI
    {
        private const string AudioQueryEndPoint = "/audio_query";
        private const string AudioQueryFromPresetEndPoint = "/audio_query_from_preset";

        /// <summary>
        /// Creates an audio query from a text.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90/operation/audio_query_audio_query_post
        /// </summary>
        /// <param name="httpClient">HttpClient instance</param>
        /// <param name="text">[Required] Text input</param>
        /// <param name="speaker">[Required] Speaker ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio query from text</returns>
        /// <exception cref="ResultPatternMatchException"></exception>
        public static async UniTask<IUncertainResult<AudioQuery>> CreateQueryAsync(
            HttpClient httpClient,
            string text,
            int speaker,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(text))
            {
                return UncertainResultFactory.Fail<AudioQuery>(
                    "Failed because text is null or empty.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    "Retryable because cancellation has been already requested.");
            }
            
            // Build query parameters
            var queryParameters = new Dictionary<string, string>();
            queryParameters["text"] = text;
            queryParameters["speaker"] = speaker.ToString();
            if (coreVersion != null)
            {
                queryParameters["core_version"] = coreVersion;
            }
            using var query = new FormUrlEncodedContent(queryParameters);
            var queryPath = await query.ReadAsStringAsync();

            // Build URL with query parameters
            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryEndPoint
                      + "?" + queryPath;

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            try
            {
                // Send request and receive response
                using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
                if (responseMessage == null)
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because{nameof(HttpResponseMessage)} is null.");
                }
                
                // Read response text
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseText))
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because response string is null or empty.");
                }

                // Succeeded
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var deserializeResult = JsonSerializer.Deserialize<AudioQuery>(responseText);
                    if (deserializeResult is ISuccessResult<AudioQuery> success)
                    {
                        return UncertainResultFactory.Succeed(success.Result);
                    }
                    else if (deserializeResult is IFailureResult<AudioQuery> failure)
                    {
                        return UncertainResultFactory.Fail<AudioQuery>(
                            $"Failed to deserialize response to {nameof(AudioQuery)} because -> {failure.Message}.");
                    }
                    else
                    {
                        throw new ResultPatternMatchException(nameof(deserializeResult));
                    }
                }
                // Retryable
                else if (responseMessage.StatusCode is HttpStatusCode.TooManyRequests
                         || (int)responseMessage.StatusCode is >= 500 and <= 599)
                {
                    return UncertainResultFactory.Retry<AudioQuery>(
                        $"Retryable because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}.");
                }
                // Response error
                else
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}."
                    );
                }
            }
            // Request error
            catch (HttpRequestException exception)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Retryable because {nameof(HttpRequestException)} was thrown during calling the API -> {exception}.");
            }
            // Task cancellation
            catch (TaskCanceledException exception)
                when (exception.CancellationToken == cancellationToken)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Failed because task was canceled by user during call to the API -> {exception}.");
            }
            // Operation cancellation 
            catch (OperationCanceledException exception)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Retryable because operation was cancelled during calling the API -> {exception}.");
            }
            // Unhandled error
            catch (Exception exception)
            {
                return UncertainResultFactory.Fail<AudioQuery>(
                    $"Failed because an unhandled exception was thrown when calling the API -> {exception}.");
            }
        }

        /// <summary>
        /// Creates an audio query from a text by a preset.
        /// See https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90/operation/audio_query_from_preset_audio_query_from_preset_post
        /// </summary>
        /// <param name="httpClient">HttpClient instance</param>
        /// <param name="text">[Required] Text input</param>
        /// <param name="presetId">[Required] Preset ID</param>
        /// <param name="coreVersion">[Optional] VOICEVOX Core version</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio query from text</returns>
        /// <exception cref="ResultPatternMatchException"></exception>
        public static async UniTask<IUncertainResult<AudioQuery>> CreateQueryFromPresetAsync(
            HttpClient httpClient,
            string text,
            int presetId,
            string? coreVersion,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(text))
            {
                return UncertainResultFactory.Fail<AudioQuery>(
                    "Failed because text is null or empty.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    "Retryable because cancellation has been already requested.");
            }
            
            // Build query parameters
            var queryParameters = new Dictionary<string, string>();
            queryParameters["text"] = text;
            queryParameters["preset_id"] = presetId.ToString();
            if (coreVersion != null)
            {
                queryParameters["core_version"] = coreVersion;
            }
            using var query = new FormUrlEncodedContent(queryParameters);
            var queryPath = await query.ReadAsStringAsync();

            // Build URL with query parameters
            var url = VoiceVoxBaseURL.BaseURL
                      + AudioQueryFromPresetEndPoint
                      + "?" + queryPath;

            // Create request
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            try
            {
                // Send request and receive response
                using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
                if (responseMessage == null)
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because{nameof(HttpResponseMessage)} is null.");
                }
                
                // Read response text
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseText))
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because response string is null or empty.");
                }

                // Succeeded
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var deserializeResult = JsonSerializer.Deserialize<AudioQuery>(responseText);
                    if (deserializeResult is ISuccessResult<AudioQuery> success)
                    {
                        return UncertainResultFactory.Succeed(success.Result);
                    }
                    else if (deserializeResult is IFailureResult<AudioQuery> failure)
                    {
                        return UncertainResultFactory.Fail<AudioQuery>(
                            $"Failed to deserialize response to {nameof(AudioQuery)} because -> {failure.Message}.");
                    }
                    else
                    {
                        throw new ResultPatternMatchException(nameof(deserializeResult));
                    }
                }
                // Retryable
                else if (responseMessage.StatusCode is HttpStatusCode.TooManyRequests
                         || (int)responseMessage.StatusCode is >= 500 and <= 599)
                {
                    return UncertainResultFactory.Retry<AudioQuery>(
                        $"Retryable because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}.");
                }
                // Response error
                else
                {
                    return UncertainResultFactory.Fail<AudioQuery>(
                        $"Failed because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode} with response -> {responseText}."
                    );
                }
            }
            // Request error
            catch (HttpRequestException exception)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Retryable because {nameof(HttpRequestException)} was thrown during calling the API -> {exception}.");
            }
            // Task cancellation
            catch (TaskCanceledException exception)
                when (exception.CancellationToken == cancellationToken)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Failed because task was canceled by user during call to the API -> {exception}.");
            }
            // Operation cancellation 
            catch (OperationCanceledException exception)
            {
                return UncertainResultFactory.Retry<AudioQuery>(
                    $"Retryable because operation was cancelled during calling the API -> {exception}.");
            }
            // Unhandled error
            catch (Exception exception)
            {
                return UncertainResultFactory.Fail<AudioQuery>(
                    $"Failed because an unhandled exception was thrown when calling the API -> {exception}.");
            }
        }
    }
}
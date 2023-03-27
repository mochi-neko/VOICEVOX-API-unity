#nullable enable
using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mochineko.Relent.Resilience;
using Mochineko.Relent.UncertainResult;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Mochineko.VOICEVOX_API.QueryCreation;
using Mochineko.VOICEVOX_API.Synthesis;
using Mochineko.SimpleAudioCodec;

namespace Mochineko.VOICEVOX_API.Samples
{
    public class SpeechSynthesisSample : MonoBehaviour
    {
        [SerializeField] private string text = string.Empty;
        [SerializeField] private int speakerID;
        [SerializeField] private AudioSource? audioSource = null;

        private readonly IPolicy<AudioQuery> queryCreationPolicy
            = PolicyFactory.BuildQueryCreationPolicy();
        private readonly IPolicy<Stream> synthesisPolicy
            = PolicyFactory.BuildSynthesisPolicy();

        private AudioClip? audioClip;

        private void Awake()
        {
            Assert.IsNotNull(audioSource);
        }

        private void OnDestroy()
        {
            if (audioClip != null)
            {
                Object.Destroy(audioClip);
                audioClip = null;
            }
        }

        [ContextMenu("Synthesis")]
        public void Synthesis()
        {
            SynthesisAsync(
                    text, 
                    speakerID,
                    this.GetCancellationTokenOnDestroy())
                .Forget();
        }
        
        private async UniTask SynthesisAsync(
            string text,
            int speakerID,
            CancellationToken cancellationToken)
        {
            if (audioSource == null)
            {
                Debug.LogError(audioSource);
                return;
            }

            if (audioClip != null)
            {
                Object.Destroy(audioClip);
                audioClip = null;
            }

            audioSource.Stop();

            await UniTask.SwitchToThreadPool();

            AudioQuery audioQuery;
            var createQueryResult = await queryCreationPolicy.ExecuteAsync(
                async innerCancellationToken => await QueryCreationAPI.CreateQueryAsync(
                    HttpClientPool.PooledClient,
                    text: text,
                    speaker: speakerID,
                    coreVersion: null,
                    cancellationToken: innerCancellationToken),
                cancellationToken);
            if (createQueryResult is IUncertainSuccessResult<AudioQuery> createQuerySuccess)
            {
                audioQuery = createQuerySuccess.Result;
                Debug.Log($"[VOICEVOX_API.Samples] Succeeded to create query from text:{text}.");
            }
            else if (createQueryResult is IUncertainRetryableResult<AudioQuery> createQueryRetryable)
            {
                Debug.LogError(
                    $"[VOICEVOX_API.Samples] Failed to create query because -> {createQueryRetryable.Message}.");
                return;
            }
            else if (createQueryResult is IUncertainFailureResult<AudioQuery> createQueryFailure)
            {
                Debug.LogError($"[VOICEVOX_API.Samples] Failed to create query because -> {createQueryFailure}.");
                return;
            }
            else
            {
                throw new UncertainResultPatternMatchException(nameof(createQueryResult));
            }

            // Synthesize speech from AudioQuery by VOICEVOX synthesis API.
            Stream stream;
            var synthesisResult = await synthesisPolicy.ExecuteAsync(
                async innerCancellationToken => await SynthesisAPI.SynthesizeAsync(
                    HttpClientPool.PooledClient,
                    audioQuery: audioQuery,
                    speaker: speakerID,
                    enableInterrogativeUpspeak: null,
                    coreVersion: null,
                    cancellationToken: innerCancellationToken),
                cancellationToken);
            if (synthesisResult is IUncertainSuccessResult<Stream> synthesisSuccess)
            {
                stream = synthesisSuccess.Result;
                await using var _ = stream;
                Debug.Log($"[VOICEVOX_API.Samples] Succeeded to synthesis speech from text:{text}.");
            }
            else if (synthesisResult is IUncertainRetryableResult<Stream> synthesisRetryable)
            {
                Debug.LogError(
                    $"[VOICEVOX_API.Samples] Failed to synthesis speech because -> {synthesisRetryable.Message}.");
                return;
            }
            else if (synthesisResult is IUncertainFailureResult<Stream> synthesisFailure)
            {
                Debug.LogError($"[VOICEVOX_API.Samples] Failed to synthesis speech because -> {synthesisFailure.Message}.");
                return;
            }
            else
            {
                throw new UncertainResultPatternMatchException(nameof(synthesisResult));
            }

            try
            {
                // Decode WAV data to AudioClip by SimpleAudioCodec WAV decoder.
                audioClip = await WaveDecoder.DecodeBlockByBlockAsync(
                    stream: stream,
                    fileName: "Synthesis.wav",
                    cancellationToken: cancellationToken);

                Debug.Log($"[VOICEVOX_API.Samples] Succeeded to decode audio, " +
                          $"samples:{audioClip.samples}, " +
                          $"frequency:{audioClip.frequency}, " +
                          $"channels:{audioClip.channels}, " +
                          $"length:{audioClip.length}.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
            finally
            {
                await stream.DisposeAsync();
            }

            await UniTask.SwitchToMainThread(cancellationToken);

            // Play AudioClip.
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
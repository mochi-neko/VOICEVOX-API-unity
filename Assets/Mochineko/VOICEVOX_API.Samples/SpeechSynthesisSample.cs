using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        /// <summary>
        /// A text you want to synthesize.
        /// </summary>
        [SerializeField] private string text = null;
        /// <summary>
        /// Speaker ID of VOICEVOX to speech.
        /// </summary>
        [SerializeField] private int speakerID;
        /// <summary>
        /// Audio source to play.
        /// </summary>
        [SerializeField] private AudioSource audioSource = null;

        private AudioClip audioClip;

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

        // TODO: Queues each request and playing audio.
        [ContextMenu("Synthesis")]
        public async Task SynthesisAsync()
        {
            if (audioClip != null)
            {
                Object.Destroy(audioClip);
                audioClip = null;
            }
            audioSource.Stop();

            var cancellationToken = this.GetCancellationTokenOnDestroy();
            // var path = Path.Combine(
            //     Application.dataPath,
            //     "Mochineko/VOICEVOX_API.Samples",
            //     $"synthesis_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}.wav");

            await UniTask.SwitchToThreadPool();

            AudioQuery query;
            try
            {
                // Create AudioQuery from text by VOICEVOX query creation API.
                query = await QueryCreationAPI.CreateQueryAsync(
                    text: text,
                    speaker: 1,
                    coreVersion: null,
                    cancellationToken: cancellationToken);
                
                Debug.Log($"[VOICEVOX_API.Samples] Succeeded to create audio query:{query.ToJson()}.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            Stream stream;
            try
            {
                // Synthesize speech from AudioQuery by VOICEVOX synthesis API.
                stream = await SynthesisAPI.SynthesizeAsync(
                    query: query,
                    speaker: speakerID,
                    enableInterrogativeUpspeak: null,
                    coreVersion: null,
                    cancellationToken: cancellationToken);
                
                Debug.Log($"[VOICEVOX_API.Samples] Succeeded to synthesis speech: {stream.Length}.");
                
                // Save to file
                // await using var writer = File.OpenWrite(path);
                //
                // await writer.WriteAsync(
                //     (stream as MemoryStream).ToArray(),
                //     cancellationToken);
                //
                // stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
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
            
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
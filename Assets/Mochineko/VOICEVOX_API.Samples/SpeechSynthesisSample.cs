using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mochineko.SimpleAudioCodec;
using Mochineko.VOICEVOX_API.QueryCreation;
using Mochineko.VOICEVOX_API.Synthesis;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Mochineko.VOICEVOX_API.Samples
{
    public class SpeechSynthesisSample : MonoBehaviour
    {
        [SerializeField] private string text = null;
        [SerializeField] private int speakerID;
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
            var path = Path.Combine(
                Application.dataPath,
                "Mochineko/VOICEVOX_API.Samples",
                $"synthesis_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}.wav");

            await UniTask.SwitchToThreadPool();

            AudioQuery query;
            try
            {
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
                stream = await SynthesisAPI.SynthesisAsync(
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
            
            // TODO: Queueing audio clip.
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
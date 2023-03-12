# VOICEVOX-API-unity

Binds [VOICEVOX](https://github.com/VOICEVOX/voicevox) text to speech [API](https://voicevox.github.io/voicevox_engine/api/) to pure C# on Unity.

See also [official page](https://voicevox.hiroshiba.jp/).

Support version is `0.14.3`.

## Avaiable API

- [x] Query Creation
  - [x] `/audio_query`
  - [x] `/audio_query_from_preset`
- [ ] Query Editing
- [ ] Synthesis
  - [x] `/synthesis`
  - [ ] `/cancellable_synthesis`
  - [ ] `/multi_synthesis`
  - [ ] `/morphable_targets`
  - [ ] `/synthesis_morphing`
- [ ] Otherwise
- [ ] Library
- [ ] User Dictionary
- [ ] Settings

## How to import by UnityPackageManager

Add dependencies:

```json
{
  "dependencies": {
    "com.mochineko.voicevox-api": "https://github.com/mochi-neko/VOICEVOX-API-unity.git?path=/Assets/Mochineko/VOICEVOX_API#0.1.0",
    "com.unity.nuget.newtonsoft-json": "3.0.2",
    ...
  }
}
```

to your `mainfest.json`.

If you have already used Newtonsoft.Json on your project, remove dependency:`"com.unity.nuget.newtonsoft-json": "3.0.2",`.

## How to use speech synthesize from text by VOICEVOX API

1. Launch your VOICEVOX server or application.
2. If you change VOICEVOX server address from default, you have to set `VoiceVoxBaseURL.BaseURL`.
3. Input text to synthesis speech.
4. Create `AudioQuery` from text by query creation API of VOICEVOX.
5. Synthesize speech from `AudioQuery` by synthesis API of VOICEVOX.
6. Decode synthesized WAV file and play on Unity as you like.
  - You can select how to decode WAV file on your project.
  - In this sample, I use (simple-audio-codec-unity)[https://github.com/mochi-neko/simple-audio-codec-unity] I wrote.

An essential sample code with [UniTask](https://github.com/Cysharp/UniTask) is as follows:

```cs
using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Mochineko.VOICEVOX_API;
using Mochineko.VOICEVOX_API.QueryCreation;
using Mochineko.VOICEVOX_API.Synthesis;
using Mochineko.SimpleAudioCodec;

namespace XXX
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
            
            // Play AudioClip.
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
```

See also [Samples](https://github.com/mochi-neko/VOICEVOX-API-unity/tree/main/Assets/Mochineko/VOICEVOX_API.Samples).

## Changelog

See [CHANGELOG](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/CHANGELOG.md)

## 3rd Party Notices

See [NOTICE](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/NOTICE.md).

## License

[MIT License](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/LICENSE)
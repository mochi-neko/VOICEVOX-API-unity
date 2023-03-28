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
    "com.mochineko.voicevox-api": "https://github.com/mochi-neko/VOICEVOX-API-unity.git?path=/Assets/Mochineko/VOICEVOX_API#0.2.1",
    "com.mochineko.relent.result": "https://github.com/mochi-neko/Relent.git?path=/Assets/Mochineko/Relent/Result#0.1.1",
    "com.mochineko.relent.uncertain-result": "https://github.com/mochi-neko/Relent.git?path=/Assets/Mochineko/Relent/UncertainResult#0.1.1",
    "com.mochineko.relent.resilience": "https://github.com/mochi-neko/Relent.git?path=/Assets/Mochineko/Relent/Resilience#0.1.1",
    "com.unity.nuget.newtonsoft-json": "3.0.2",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
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
4. Create `AudioQuery` from text by `QueryCreationAPI`.
5. Synthesize speech from `AudioQuery` by `SynthesisAPI`.
6. Decode synthesized WAV file and play on Unity as you like.
  - You can select how to decode WAV file on your project.
  - In this sample, I use [simple-audio-codec-unity](https://github.com/mochi-neko/simple-audio-codec-unity) I wrote.

A sample code with [UniTask](https://github.com/Cysharp/UniTask) is
 [this](https://github.com/mochi-neko/VOICEVOX-API-unity/tree/main/Assets/Mochineko/VOICEVOX_API.Samples/SpeechSynthesisSample.cs).

## Changelog

See [CHANGELOG](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/CHANGELOG.md)

## 3rd Party Notices

See [NOTICE](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/NOTICE.md).

## License

[MIT License](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/LICENSE)
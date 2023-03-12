# VOICEVOX-API-unity

Binds [VOICEVOX](https://github.com/VOICEVOX/voicevox) text to speech [API](https://voicevox.github.io/voicevox_engine/api/) to pure C# on Unity.

See also [official page](https://voicevox.hiroshiba.jp/).

Support version is `0.14.3`.

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

## How to use speech synthesis from text by VOICEVOX

1. Launch your VOICEVOX server.
2. 

An essential sample code with [UniTask](https://github.com/Cysharp/UniTask) is as follows:

```cs

```

See also [Sample]().

## Avaiable API

- [x] Query Creation
  - [x] `/audio_query`
  - [x] `/audio_query_from_preset`
- [ ] Query Editing
- [ ] Synthesis
  - [x] `/synthesis`
  - [x] `/cancellable_synthesis`
  - [ ] `/multi_synthesis`
  - [ ] `/morphable_targets`
  - [ ] `/synthesis_morphing`
- [ ] Otherwise
- [ ] Library
- [ ] User Dictionary
- [ ] Settings

## Changelog

See [CHANGELOG](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/CHANGELOG.md)

## 3rd Party Notices

See [NOTICE](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/NOTICE.md).

## License

[MIT License](https://github.com/mochi-neko/VOICEVOX-API-unity/blob/main/LICENSE)
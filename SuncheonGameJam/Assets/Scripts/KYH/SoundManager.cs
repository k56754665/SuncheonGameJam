using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    // 🔹 추가: 루프용 SFX 전용 소스(OneShot과 분리)
    [Header("SFX Loop Channel")]
    [SerializeField] private AudioSource sfxLoopSource;
    [SerializeField, Range(0f,1f)] private float sfxLoopVolume = 0.3f;
    private bool _sfxLoopActive = false;
    private SoundType _currentLoopType;

    [Header("Sound DB")]
    [SerializeField] private SoundDB soundDB; // ScriptableObject
    
    private Dictionary<SoundType, AudioClip> _clipMap;

    protected override void Awake() {
        base.Awake();

        // 🔹 AudioSource 자동 생성
        if (sfxSource == null) {
            var go = new GameObject("SFX_AudioSource");
            go.transform.SetParent(transform);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.volume = 0.6f; // 기본 SFX 볼륨
        }
        if (bgmSource == null) {
            var go = new GameObject("BGM_AudioSource");
            go.transform.SetParent(transform);
            bgmSource = go.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.volume = 0.15f; // 기본 BGM 볼륨
        }
        // 🔹 추가: 루프용 SFX 채널 생성
        if (sfxLoopSource == null) {
            var go = new GameObject("SFX_Loop_AudioSource");
            go.transform.SetParent(transform);
            sfxLoopSource = go.AddComponent<AudioSource>();
            sfxLoopSource.playOnAwake = false;
            sfxLoopSource.loop = true;
            sfxLoopSource.volume = sfxLoopVolume;
        }

        // 🔹 SoundDB → Dictionary 초기화
        _clipMap = new Dictionary<SoundType, AudioClip>();
        foreach (var e in soundDB.entries) {
            _clipMap[e.type] = e.clip;
        }

        // 테스트용 BGM 실행
        //PlayBGM(SoundType.BGM_Test);
    }
    
    #region Subscribe
    private void OnEnable() {
        EventBus.SubscribeEndMiniGame(PlayEndMiniGame);
    }
    private void OnDisable() {
        EventBus.UnsubscribeEndMiniGame(PlayEndMiniGame);
    }
    #endregion

    public void PlayEndMiniGame(AnimalStruct animal , bool isSuccess)
    {
        if(isSuccess)
            Play(SoundType.GetAnimal);
        else
            Play(SoundType.FailGet);
    }

    // -------------------
    // 사운드 재생 (OneShot)
    // -------------------
    public void Play(SoundType type) {
        if (_clipMap.TryGetValue(type, out var clip)) {
            Debug.Log(type);
            Debug.Log(clip);
            sfxSource.PlayOneShot(clip);
        }
        foreach(var e in _clipMap)
        {
            Debug.Log(e.Key);
            Debug.Log(e.Value);
        }
    }
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    // -------------------
    // 루프 SFX 제어 (겹침 동안 반복재생 등에 사용)
    // -------------------
    public void PlaySFXLoop(SoundType type, float? volumeOverride = null) {
        if (!_clipMap.TryGetValue(type, out var clip)) return;

        // 같은 루프면 중복 시작 방지
        if (_sfxLoopActive && sfxLoopSource.clip == clip && sfxLoopSource.isPlaying) return;

        sfxLoopSource.Stop();
        sfxLoopSource.clip = clip;
        sfxLoopSource.volume = volumeOverride.HasValue ? Mathf.Clamp01(volumeOverride.Value) : sfxLoopVolume;
        sfxLoopSource.loop = true;
        sfxLoopSource.Play();

        _sfxLoopActive = true;
        _currentLoopType = type;
    }

    public void StopSFXLoop() {
        if (!_sfxLoopActive) return;
        sfxLoopSource.Stop();
        _sfxLoopActive = false;
    }

    public bool IsSFXLooping(SoundType type) {
        return _sfxLoopActive && sfxLoopSource.clip != null && _currentLoopType.Equals(type) && sfxLoopSource.isPlaying;
    }

    // -------------------
    // BGM
    // -------------------
    public void PlayBGM(SoundType type, bool loop = true) {
        if (_clipMap.TryGetValue(type, out var clip)) {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return; // 같은 곡이면 무시
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM() => bgmSource.Stop();

    public void FadeBGM(SoundType type, float fadeTime = 1f) {
        StartCoroutine(FadeBGMCoroutine(type, fadeTime));
    }

    private IEnumerator FadeBGMCoroutine(SoundType type, float fadeTime) {
        if (!_clipMap.TryGetValue(type, out var newClip)) yield break;

        // 현재 볼륨 저장(나중에 복원)
        float originalVol = bgmSource.volume;

        // 현재 곡 페이드 아웃
        float t = 0f;
        while (t < fadeTime) {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(originalVol, 0f, t / fadeTime);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // 새 곡 페이드 인
        t = 0f;
        while (t < fadeTime) {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, originalVol, t / fadeTime);
            yield return null;
        }

        // 볼륨 최종 보정
        bgmSource.volume = originalVol;
    }
    public bool SFXLOOPINGISPLAYING()
    {
        return sfxLoopSource.isPlaying;
    }
}

public enum SoundType {
    Button_Click,
    Book,
    ReedWalk,
    MudWalk,
    SeaWalk,
    GetAnimal,
    CatchingAnimal,
    FailGet,
    BGM_Reed,
    BGM_Sea,
    SeaPortalInteraction,
    MudPortalInteraction
}

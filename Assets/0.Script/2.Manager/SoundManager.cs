using UnityEngine;

public class SoundManager : MonoBehaviour
{
     public static SoundManager Instance { get; private set; }
    
        // 단일 SFX용 오디오 소스
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip[] sfxClips;
        
        // BGM용 오디오 소스
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioClip[] bgmClips;
    
        // 볼륨 설정
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float bgmVolume = 1f;
    
        // 사운드 끄기/켜기 플래그
        [SerializeField] private bool isSoundMuted = false;
        [SerializeField] private bool isBgmMuted = false;
    
        void Awake()
        {
            // 싱글톤 패턴 구현
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 방지
            }
            else
            {
                Destroy(gameObject);
                return;
            }
    
            // 오디오 소스 초기화
            if (sfxSource == null)
            {
                Debug.LogWarning("[SoundManager] SFX 소스가 설정되지 않았습니다. 기본으로 생성합니다.");
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
    
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true; // BGM은 기본적으로 루프
            }
    
            // 초기 볼륨 적용
            ApplyVolume();
        }
    
        // 사운드 재생 (SFX)
        public void PlaySound(Enums.Sfx sfx)
        {
            if (isSoundMuted || sfxClips[(int)sfx] == null) return;
    
            sfxSource.volume = sfxVolume * masterVolume;
            sfxSource.PlayOneShot(sfxClips[(int)sfx]);
        }
    
        // BGM 재생 (페이드 적용)
        public void PlayBGM(Enums.Bgm bgm, float fadeDuration = 1.0f)
        {
            if (isBgmMuted || bgmClips[(int)bgm] == null) return;
    
            if (bgmSource.clip != bgmClips[(int)bgm] || !bgmSource.isPlaying)
            {
                StartCoroutine(FadeBGMCoroutine(bgmClips[(int)bgm], fadeDuration));
            }
        }
    
        // BGM 정지
        public void StopBGM(float fadeDuration = 1.0f)
        {
            if (bgmSource.isPlaying)
            {
                StartCoroutine(FadeOutBGMCoroutine(fadeDuration));
            }
        }
    
        // 볼륨 설정
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }
    
        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }
    
        public void SetBgmVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }
    
        // 사운드 및 BGM 음소거 토글
        public void ToggleSoundMute()
        {
            isSoundMuted = !isSoundMuted;
            ApplyVolume();
        }
    
        public void ToggleBgmMute()
        {
            isBgmMuted = !isBgmMuted;
            ApplyVolume();
        }
    
        // 볼륨 적용
        private void ApplyVolume()
        {
            if (sfxSource != null)
            {
                sfxSource.volume = isSoundMuted ? 0f : (sfxVolume * masterVolume);
            }
            if (bgmSource != null)
            {
                bgmSource.volume = isBgmMuted ? 0f : (bgmVolume * masterVolume);
            }
        }
    
        // BGM 페이드 인/아웃 코루틴
        private System.Collections.IEnumerator FadeBGMCoroutine(AudioClip newClip, float fadeDuration)
        {
            float time = 0f;
            float startVolume = bgmSource.volume;
    
            // 현재 BGM 페이드 아웃
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }
    
            bgmSource.Stop();
            bgmSource.clip = newClip;
            bgmSource.Play();
    
            // 새 BGM 페이드 인
            time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(0f, bgmVolume * masterVolume, time / fadeDuration);
                yield return null;
            }
    
            bgmSource.volume = isBgmMuted ? 0f : (bgmVolume * masterVolume); // 최종 볼륨 적용
        }
    
        private System.Collections.IEnumerator FadeOutBGMCoroutine(float fadeDuration)
        {
            float time = 0f;
            float startVolume = bgmSource.volume;
    
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }
    
            bgmSource.Stop();
            bgmSource.volume = isBgmMuted ? 0f : (bgmVolume * masterVolume); 
        }
}

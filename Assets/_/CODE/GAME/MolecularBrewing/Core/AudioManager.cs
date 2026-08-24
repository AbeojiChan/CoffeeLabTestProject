using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    public class AudioManager : MonoBehaviour
    {
        #region Publics

        public static AudioManager Instance { get; private set; }
        public float m_masterVolume = 1.0f;
        public float m_sfxVolume = 1.0f;
        public float m_bgmVolume = 0.35f;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion


        #region Main API

        // ================= SFX API =================

        public static void PlaySFX(string clipName, float volumeScale = 1.0f)
        {
            EnsureInstance();
            if (Instance != null)
            {
                Instance.PlayOneShotInternal(clipName, volumeScale);
            }
        }

        public static void PlayLoopingSFX(string clipName, float volumeScale = 1.0f)
        {
            EnsureInstance();
            if (Instance != null)
            {
                Instance.PlayLoopInternal(clipName, volumeScale);
            }
        }

        public static void StopLoopingSFX()
        {
            if (Instance != null)
            {
                Instance.StopLoopInternal();
            }
        }

        public static void PlayCoffeeGrind(bool loop = true)
        {
            if (loop) PlayLoopingSFX("SFX_Coffee_Grind", 0.9f);
            else PlaySFX("SFX_Coffee_Grind", 0.9f);
        }

        public static void PlayLeavesDrying(bool loop = true)
        {
            if (loop) PlayLoopingSFX("SFX_Leaves_drying", 0.9f);
            else PlaySFX("SFX_Leaves_drying", 0.9f);
        }

        public static void PlayMilkFrother(bool loop = true)
        {
            if (loop) PlayLoopingSFX("SFX_Milk_Froth", 0.9f);
            else PlaySFX("SFX_Milk_Froth", 0.9f);
        }

        public static void PlaySolvent(bool loop = true)
        {
            if (loop) PlayLoopingSFX("SFX_Solvent", 0.9f);
            else PlaySFX("SFX_Solvent", 0.9f);
        }

        public static void PlaySynthesis(float volume = 1.0f)
        {
            PlaySFX("SFX_Synthesis", volume);
        }

        public static void PlayMetro(float volume = 1.0f)
        {
            PlaySFX("SFX_Metro", volume);
        }

        public static void PlayBond(float volume = 1.0f)
        {
            PlaySFX("SFX_Bond", volume);
        }

        public static void PlayBreak(float volume = 1.0f)
        {
            PlaySFX("SFX_Break", volume);
        }

        public static void PlayMenuNav(float volume = 0.85f)
        {
            PlaySFX("SFX_Menu_Nav", volume);
        }

        // ================= BGM API =================

        public static void PlayBGM(string bgmName, float fadeDuration = 0.75f)
        {
            EnsureInstance();
            if (Instance != null)
            {
                Instance.PlayBgmInternal(bgmName, fadeDuration);
            }
        }

        public static void PlayGameBGM(float fadeDuration = 0.75f)
        {
            PlayBGM("BGM_Game", fadeDuration);
        }

        public static void PlayDialogBGM(float fadeDuration = 0.75f)
        {
            PlayBGM("BGM_Dialog", fadeDuration);
        }

        public static void PlayCityBGM(float fadeDuration = 0.75f)
        {
            PlayBGM("BGM_City", fadeDuration);
        }

        public static void StopBGM(float fadeDuration = 0.75f)
        {
            if (Instance != null)
            {
                Instance.StopBgmInternal(fadeDuration);
            }
        }

        public static void SetBGMVolume(float volume)
        {
            if (Instance != null)
            {
                Instance.m_bgmVolume = Mathf.Clamp01(volume);
                if (Instance._bgmSource != null && Instance._bgmSource.isPlaying)
                {
                    Instance._bgmSource.volume = Instance.m_bgmVolume * Instance.m_masterVolume;
                }
            }
        }

        #endregion


        #region Tools and Utilities

        private static void EnsureInstance()
        {
            if (Instance == null)
            {
                GameObject obj = new GameObject("AudioManager");
                Instance = obj.AddComponent<AudioManager>();
                DontDestroyOnLoad(obj);
                Instance.InitializeAudioSources();
            }
        }

        private void InitializeAudioSources()
        {
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
                _sfxSource.loop = false;
            }

            if (_loopSource == null)
            {
                _loopSource = gameObject.AddComponent<AudioSource>();
                _loopSource.playOnAwake = false;
                _loopSource.loop = true;
            }

            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.playOnAwake = false;
                _bgmSource.loop = true;
            }
        }

        private AudioClip GetOrLoadClip(string clipName)
        {
            if (string.IsNullOrEmpty(clipName)) return null;

            if (_clipCache.TryGetValue(clipName, out var cachedClip) && cachedClip != null)
            {
                return cachedClip;
            }

            // 1. Try BGM/ folder
            AudioClip clip = Resources.Load<AudioClip>("BGM/" + clipName);

            // 2. Try SFX/ folder
            if (clip == null)
            {
                clip = Resources.Load<AudioClip>("SFX/" + clipName);
            }

            // 3. Try Root Resources folder
            if (clip == null)
            {
                clip = Resources.Load<AudioClip>(clipName);
            }

            // 4. Aliases and fallbacks
            if (clip == null && clipName.Contains("Milk"))
            {
                clip = Resources.Load<AudioClip>("SFX/SFX_Milk_Froth");
                if (clip == null) clip = Resources.Load<AudioClip>("SFX/SFX_Milk_Frother");
            }
            if (clip == null && (clipName == "SFX_Meneu_Nav" || clipName == "SFX_Menu_Nav"))
            {
                clip = Resources.Load<AudioClip>("SFX/SFX_Menu_Nav");
            }

            if (clip != null)
            {
                _clipCache[clipName] = clip;
                return clip;
            }

            Debug.LogWarning($"[AudioManager] Could not load audio clip: {clipName}");
            return null;
        }

        private void PlayOneShotInternal(string clipName, float volumeScale)
        {
            AudioClip clip = GetOrLoadClip(clipName);
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip, volumeScale * m_sfxVolume * m_masterVolume);
            }
        }

        private void PlayLoopInternal(string clipName, float volumeScale)
        {
            AudioClip clip = GetOrLoadClip(clipName);
            if (clip != null && _loopSource != null)
            {
                if (_loopSource.isPlaying && _loopSource.clip == clip) return;

                _loopSource.clip = clip;
                _loopSource.volume = volumeScale * m_sfxVolume * m_masterVolume;
                _loopSource.loop = true;
                _loopSource.Play();
            }
        }

        private void StopLoopInternal()
        {
            if (_loopSource != null && _loopSource.isPlaying)
            {
                _loopSource.Stop();
                _loopSource.clip = null;
            }
        }

        private void PlayBgmInternal(string bgmName, float fadeDuration)
        {
            if (_currentBgmName == bgmName && _bgmSource != null && _bgmSource.isPlaying)
            {
                return;
            }

            AudioClip clip = GetOrLoadClip(bgmName);
            if (clip == null) return;

            _currentBgmName = bgmName;

            if (_bgmFadeRoutine != null)
            {
                StopCoroutine(_bgmFadeRoutine);
            }

            _bgmFadeRoutine = StartCoroutine(CrossFadeBgmRoutine(clip, fadeDuration));
        }

        private void StopBgmInternal(float fadeDuration)
        {
            _currentBgmName = "";
            if (_bgmFadeRoutine != null)
            {
                StopCoroutine(_bgmFadeRoutine);
            }
            _bgmFadeRoutine = StartCoroutine(FadeOutBgmRoutine(fadeDuration));
        }

        private IEnumerator CrossFadeBgmRoutine(AudioClip newClip, float duration)
        {
            float targetVol = m_bgmVolume * m_masterVolume;

            // Fade out current track if playing
            if (_bgmSource != null && _bgmSource.isPlaying && _bgmSource.volume > 0.01f)
            {
                float startVol = _bgmSource.volume;
                float halfDuration = duration * 0.5f;
                float elapsed = 0f;

                while (elapsed < halfDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _bgmSource.volume = Mathf.Lerp(startVol, 0f, elapsed / halfDuration);
                    yield return null;
                }
            }

            if (_bgmSource != null)
            {
                _bgmSource.clip = newClip;
                _bgmSource.volume = 0f;
                _bgmSource.loop = true;
                _bgmSource.Play();

                float elapsed = 0f;
                float fadeInDuration = duration * 0.5f;

                while (elapsed < fadeInDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _bgmSource.volume = Mathf.Lerp(0f, targetVol, elapsed / fadeInDuration);
                    yield return null;
                }

                _bgmSource.volume = targetVol;
            }
        }

        private IEnumerator FadeOutBgmRoutine(float duration)
        {
            if (_bgmSource != null && _bgmSource.isPlaying)
            {
                float startVol = _bgmSource.volume;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _bgmSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
                    yield return null;
                }

                _bgmSource.Stop();
                _bgmSource.clip = null;
                _bgmSource.volume = 0f;
            }
        }

        #endregion


        #region Private and Protected

        private AudioSource _sfxSource;
        private AudioSource _loopSource;
        private AudioSource _bgmSource;
        private string _currentBgmName = "";
        private Coroutine _bgmFadeRoutine;
        private Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

        #endregion
    }
}

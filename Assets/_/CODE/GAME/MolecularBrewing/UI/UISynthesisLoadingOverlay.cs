using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    public class UISynthesisLoadingOverlay : MonoBehaviour
    {
        #region Publics

        public float m_loadingDuration = 2.0f;

        #endregion


        #region Unity API

        private void Update()
        {
            if (_isPlaying)
            {
                if (_spinnerRing1 != null)
                {
                    _spinnerRing1.transform.Rotate(0, 0, -220f * Time.deltaTime);
                }
                if (_spinnerRing2 != null)
                {
                    _spinnerRing2.transform.Rotate(0, 0, 150f * Time.deltaTime);
                }
            }
        }

        #endregion


        #region Main API

        public void Initialize()
        {
            if (_progressFillRect == null || _spinnerRing1 == null)
            {
                BuildUI();
            }
            gameObject.SetActive(false);
        }

        public void PlaySynthesisSequence(float duration, Action onComplete)
        {
            m_loadingDuration = Mathf.Clamp(duration, 2.0f, 5.0f);
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            StartCoroutine(SynthesisRoutine(onComplete));
        }

        public void PlaySynthesisSequence(Action onComplete)
        {
            PlaySynthesisSequence(m_loadingDuration, onComplete);
        }

        #endregion


        #region Tools and Utilities

        private void BuildUI()
        {
            var t = UIStyleHelper.Theme;

            RectTransform rootRt = GetComponent<RectTransform>();
            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            // Semi-transparent backdrop
            GameObject backdrop = UIStyleHelper.CreatePanel(transform, "Backdrop", new Color(0.1f, 0.15f, 0.2f, 0.70f));
            RectTransform bdRt = backdrop.GetComponent<RectTransform>();
            bdRt.anchorMin = Vector2.zero;
            bdRt.anchorMax = Vector2.one;
            bdRt.offsetMin = Vector2.zero;
            bdRt.offsetMax = Vector2.zero;

            // Center Mixing Chamber Frame
            GameObject frameObj = UIStyleHelper.CreatePanel(transform, "CenterFrame", t.m_panelBg);
            RectTransform frRt = frameObj.GetComponent<RectTransform>();
            frRt.anchorMin = new Vector2(0.25f, 0.20f);
            frRt.anchorMax = new Vector2(0.75f, 0.80f);
            frRt.offsetMin = Vector2.zero;
            frRt.offsetMax = Vector2.zero;

            // Title
            _titleText = UIStyleHelper.CreateText(frameObj.transform, "Title", "CYBER-SYNTHESIZER ENGAGED", 16, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform ttRt = _titleText.rectTransform;
            ttRt.anchorMin = new Vector2(0.05f, 0.82f);
            ttRt.anchorMax = new Vector2(0.95f, 0.95f);
            ttRt.offsetMin = Vector2.zero;
            ttRt.offsetMax = Vector2.zero;

            // Outer Spinning Ring
            GameObject ring1 = UIStyleHelper.CreatePanel(frameObj.transform, "SpinnerRing1", new Color(t.m_accentCyan.r, t.m_accentCyan.g, t.m_accentCyan.b, 0.4f), UIStyleHelper.CircleSprite);
            _spinnerRing1 = ring1.GetComponent<RectTransform>();
            _spinnerRing1.anchorMin = new Vector2(0.35f, 0.40f);
            _spinnerRing1.anchorMax = new Vector2(0.65f, 0.75f);
            _spinnerRing1.offsetMin = Vector2.zero;
            _spinnerRing1.offsetMax = Vector2.zero;

            // Inner Counter-Rotating Ring
            GameObject ring2 = UIStyleHelper.CreatePanel(frameObj.transform, "SpinnerRing2", new Color(t.m_accentAmber.r, t.m_accentAmber.g, t.m_accentAmber.b, 0.5f), UIStyleHelper.CircleSprite);
            _spinnerRing2 = ring2.GetComponent<RectTransform>();
            _spinnerRing2.anchorMin = new Vector2(0.40f, 0.46f);
            _spinnerRing2.anchorMax = new Vector2(0.60f, 0.69f);
            _spinnerRing2.offsetMin = Vector2.zero;
            _spinnerRing2.offsetMax = Vector2.zero;

            // Center Pulse Core
            GameObject coreObj = UIStyleHelper.CreatePanel(frameObj.transform, "PulseCore", t.m_accentGreen, UIStyleHelper.CircleSprite);
            RectTransform crRt = coreObj.GetComponent<RectTransform>();
            crRt.anchorMin = new Vector2(0.46f, 0.53f);
            crRt.anchorMax = new Vector2(0.54f, 0.62f);
            crRt.offsetMin = Vector2.zero;
            crRt.offsetMax = Vector2.zero;

            // Telemetry / Status Line
            _statusText = UIStyleHelper.CreateText(frameObj.transform, "Status", "EXTRACTING MOLECULAR BONDS & ALKALOIDS...", 11, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Normal);
            RectTransform stRt = _statusText.rectTransform;
            stRt.anchorMin = new Vector2(0.05f, 0.24f);
            stRt.anchorMax = new Vector2(0.95f, 0.35f);
            stRt.offsetMin = Vector2.zero;
            stRt.offsetMax = Vector2.zero;

            // Progress Track
            GameObject trackObj = UIStyleHelper.CreatePanel(frameObj.transform, "Track", t.m_wellBg);
            RectTransform trRt = trackObj.GetComponent<RectTransform>();
            trRt.anchorMin = new Vector2(0.10f, 0.12f);
            trRt.anchorMax = new Vector2(0.90f, 0.20f);
            trRt.offsetMin = Vector2.zero;
            trRt.offsetMax = Vector2.zero;

            // Progress Fill
            GameObject fillObj = UIStyleHelper.CreatePanel(trackObj.transform, "Fill", t.m_accentCyan);
            _progressFillRect = fillObj.GetComponent<RectTransform>();
            _progressFillRect.anchorMin = Vector2.zero;
            _progressFillRect.anchorMax = new Vector2(0f, 1f);
            _progressFillRect.offsetMin = Vector2.zero;
            _progressFillRect.offsetMax = Vector2.zero;

            // Percentage Label
            _percentageText = UIStyleHelper.CreateText(trackObj.transform, "Percent", "SYNTHESIS: 0%", 10, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform ptRt = _percentageText.rectTransform;
            ptRt.anchorMin = Vector2.zero;
            ptRt.anchorMax = Vector2.one;
            ptRt.offsetMin = Vector2.zero;
            ptRt.offsetMax = Vector2.zero;
        }

        private IEnumerator SynthesisRoutine(Action onComplete)
        {
            _isPlaying = true;
            float elapsed = 0f;
            _progressFillRect.anchorMax = new Vector2(0f, 1f);

            AudioManager.PlaySynthesis(1.0f);

            string[] phases = new string[]
            {
                "EXTRACTING COVALENT BONDS & SOLUBLES...",
                "AERATING EMULSION & MOLECULAR MIXING...",
                "ACTIVATING MEDICINAL BIO-PROPERTIES...",
                "COMPILING FINAL TASTE MATRIX..."
            };

            while (elapsed < m_loadingDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / m_loadingDuration);

                _progressFillRect.anchorMax = new Vector2(progress, 1f);
                _percentageText.text = $"SYNTHESIZING: {Mathf.RoundToInt(progress * 100f)}%";

                int phaseIndex = Mathf.Clamp(Mathf.FloorToInt(progress * phases.Length), 0, phases.Length - 1);
                _statusText.text = phases[phaseIndex];

                yield return null;
            }

            _progressFillRect.anchorMax = new Vector2(1f, 1f);
            _percentageText.text = "SYNTHESIS COMPLETE!";
            _statusText.text = "<color=#66FFCC>BEVERAGE READY FOR PRESENTATION!</color>";

            yield return new WaitForSeconds(0.2f);

            _isPlaying = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        #endregion


        #region Private and Protected

        [Header("GameObject Serialized Bindings")]
        [SerializeField] private RectTransform _spinnerRing1;
        [SerializeField] private RectTransform _spinnerRing2;
        [SerializeField] private RectTransform _progressFillRect;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _percentageText;

        private bool _isPlaying;

        #endregion
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    public class UIBrewResultModal : MonoBehaviour
    {
        #region Publics

        public event Action OnTweakSynthesisRequested;
        public event Action OnEndOfShiftRequested;

        #endregion


        #region Unity API

        #endregion


        #region Main API

        public void Initialize()
        {
            if (!HasSerializedBindings())
            {
                BuildUI();
            }
            else
            {
                BindExistingHierarchy();
            }
            gameObject.SetActive(false);
        }

        public void DisplayResult(DrinkEvaluationResult result, bool isNewDiscovery = false)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            _titleText.text = result.m_drinkName;
            if (isNewDiscovery)
            {
                _subtitleText.text = $"<color=#FBBF24>[+] NEW BLUEPRINT DISCOVERED & CODEX UNLOCKED! [+]</color>\n{result.m_subtitle}";
            }
            else
            {
                _subtitleText.text = result.m_subtitle;
            }

            _rankBadgeText.text = result.m_rank;
            _scoreText.text = $"Synthesis Rating: {result.m_matchScore}%";
            _drinkHeaderImage.color = result.m_primaryColor;

            if (result.m_unlockedEffects != null && result.m_unlockedEffects.Count > 0)
            {
                var eff = result.m_unlockedEffects[0];
                _effectBannerText.text = $"[ACTIVE SPECIAL PROPERTY]\n{eff.m_displayName}: {eff.m_tagline}";
                _effectBannerImage.color = eff.m_badgeColor;
                _effectBannerObj.SetActive(true);
            }
            else
            {
                _effectBannerText.text = "[STANDARD BEVERAGE - NO SPECIAL PROPERTY ACTIVATED]";
                _effectBannerImage.color = new Color(0.2f, 0.3f, 0.4f, 1f);
                _effectBannerObj.SetActive(true);
            }

            string tagsStr = result.m_highlightedTags != null && result.m_highlightedTags.Count > 0 ?
                string.Join("   ", result.m_highlightedTags) : "Neutral Chemistry";

            if (isNewDiscovery)
            {
                _tagsText.text = $"<color=#FBBF24>[NEW BLUEPRINT]</color>   {tagsStr}";
            }
            else
            {
                _tagsText.text = tagsStr;
            }

            _critiqueText.text = result.m_baristaNotes;

            _meterNeuralCalm.SetValue(result.m_stats.m_neuralCalm, true);
            _meterSedation.SetValue(result.m_stats.m_sedation, true);
            _meterFocus.SetValue(result.m_stats.m_focus, true);
            _meterEuphoria.SetValue(result.m_stats.m_euphoria, true);
            _meterSweet.SetValue(result.m_stats.m_sweetness, true);
            _meterBitter.SetValue(result.m_stats.m_bitterness, true);
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

            // Backdrop
            GameObject backdrop = UIStyleHelper.CreatePanel(transform, "Backdrop", new Color(0.1f, 0.15f, 0.2f, 0.70f));
            RectTransform bdRt = backdrop.GetComponent<RectTransform>();
            bdRt.anchorMin = Vector2.zero;
            bdRt.anchorMax = Vector2.one;
            bdRt.offsetMin = Vector2.zero;
            bdRt.offsetMax = Vector2.zero;

            // Main Card Window
            GameObject cardObj = UIStyleHelper.CreatePanel(transform, "Card", t.m_panelBg);
            RectTransform cRt = cardObj.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.18f, 0.06f);
            cRt.anchorMax = new Vector2(0.82f, 0.94f);
            cRt.offsetMin = Vector2.zero;
            cRt.offsetMax = Vector2.zero;

            // Header Banner
            GameObject headerObj = UIStyleHelper.CreatePanel(cardObj.transform, "HeaderBanner", t.m_headerBg);
            _drinkHeaderImage = headerObj.GetComponent<Image>();
            RectTransform hRt = headerObj.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0f, 0.82f);
            hRt.anchorMax = new Vector2(1f, 1f);
            hRt.offsetMin = Vector2.zero;
            hRt.offsetMax = Vector2.zero;

            _titleText = UIStyleHelper.CreateText(headerObj.transform, "Title", "DRINK NAME", 20, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform tRt = _titleText.rectTransform;
            tRt.anchorMin = new Vector2(0.05f, 0.40f);
            tRt.anchorMax = new Vector2(0.80f, 0.95f);
            tRt.offsetMin = Vector2.zero;
            tRt.offsetMax = Vector2.zero;

            _subtitleText = UIStyleHelper.CreateText(headerObj.transform, "Subtitle", "Subtitle", 12, new Color(0.9f, 0.95f, 1f, 0.9f), TextAlignmentOptions.Center, FontStyles.Normal);
            RectTransform stRt = _subtitleText.rectTransform;
            stRt.anchorMin = new Vector2(0.05f, 0.05f);
            stRt.anchorMax = new Vector2(0.80f, 0.45f);
            stRt.offsetMin = Vector2.zero;
            stRt.offsetMax = Vector2.zero;

            // Rank Badge
            GameObject badgeObj = UIStyleHelper.CreatePanel(headerObj.transform, "Badge", t.m_accentCyan, UIStyleHelper.CircleSprite);
            RectTransform bgRt = badgeObj.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0.83f, 0.15f);
            bgRt.anchorMax = new Vector2(0.97f, 0.85f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            _rankBadgeText = UIStyleHelper.CreateText(badgeObj.transform, "RankText", "S", 22, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform rbRt = _rankBadgeText.rectTransform;
            rbRt.anchorMin = Vector2.zero;
            rbRt.anchorMax = Vector2.one;
            rbRt.offsetMin = Vector2.zero;
            rbRt.offsetMax = Vector2.zero;

            // Special Effect Banner
            _effectBannerObj = UIStyleHelper.CreatePanel(cardObj.transform, "EffectCard", t.m_cardBg);
            _effectBannerImage = _effectBannerObj.GetComponent<Image>();
            RectTransform efRt = _effectBannerObj.GetComponent<RectTransform>();
            efRt.anchorMin = new Vector2(0.04f, 0.70f);
            efRt.anchorMax = new Vector2(0.96f, 0.80f);
            efRt.offsetMin = Vector2.zero;
            efRt.offsetMax = Vector2.zero;

            _effectBannerText = UIStyleHelper.CreateText(_effectBannerObj.transform, "EffectText", "Special Effect...", 11, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform eftRt = _effectBannerText.rectTransform;
            eftRt.anchorMin = Vector2.zero;
            eftRt.anchorMax = Vector2.one;
            eftRt.offsetMin = new Vector2(5, 5);
            eftRt.offsetMax = new Vector2(-5, -5);

            // Body Left: Bio-Stats & Neural Meters
            GameObject statsGroup = UIStyleHelper.CreatePanel(cardObj.transform, "StatsGroup", t.m_cardBg);
            RectTransform sgRt = statsGroup.GetComponent<RectTransform>();
            sgRt.anchorMin = new Vector2(0.04f, 0.16f);
            sgRt.anchorMax = new Vector2(0.48f, 0.68f);
            sgRt.offsetMin = Vector2.zero;
            sgRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(statsGroup.transform, "StatsTitle", "NEURAL BIO-STATS", 12, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);

            GameObject metersList = new GameObject("Meters", typeof(RectTransform), typeof(VerticalLayoutGroup));
            metersList.transform.SetParent(statsGroup.transform, false);
            RectTransform mlRt = metersList.GetComponent<RectTransform>();
            mlRt.anchorMin = new Vector2(0.04f, 0.04f);
            mlRt.anchorMax = new Vector2(0.96f, 0.88f);
            mlRt.offsetMin = Vector2.zero;
            mlRt.offsetMax = Vector2.zero;

            VerticalLayoutGroup vlg = metersList.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 4;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;

            _meterNeuralCalm = CreateMeter(metersList.transform, "Neural Calm (Anxiety)", new Color(0.05f, 0.70f, 0.60f, 1f));
            _meterSedation = CreateMeter(metersList.transform, "Sedation (Sleep)", new Color(0.35f, 0.25f, 0.80f, 1f));
            _meterFocus = CreateMeter(metersList.transform, "Focus (Overclock)", new Color(0.85f, 0.25f, 0.30f, 1f));
            _meterEuphoria = CreateMeter(metersList.transform, "Euphoria (Joy)", new Color(0.85f, 0.30f, 0.75f, 1f));
            _meterSweet = CreateMeter(metersList.transform, "Sweetness", new Color(0.85f, 0.65f, 0.20f, 1f));
            _meterBitter = CreateMeter(metersList.transform, "Bitterness", new Color(0.40f, 0.25f, 0.15f, 1f));

            // Body Right: Barista Critique & Story Feedback
            GameObject rightGroup = UIStyleHelper.CreatePanel(cardObj.transform, "RightGroup", t.m_cardBg);
            RectTransform rgRt = rightGroup.GetComponent<RectTransform>();
            rgRt.anchorMin = new Vector2(0.52f, 0.16f);
            rgRt.anchorMax = new Vector2(0.96f, 0.68f);
            rgRt.offsetMin = Vector2.zero;
            rgRt.offsetMax = Vector2.zero;

            _scoreText = UIStyleHelper.CreateText(rightGroup.transform, "Score", "Synthesis Score: 95%", 13, t.m_accentCyan, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            RectTransform scRt = _scoreText.rectTransform;
            scRt.anchorMin = new Vector2(0.04f, 0.86f);
            scRt.anchorMax = new Vector2(0.96f, 0.96f);
            scRt.offsetMin = Vector2.zero;
            scRt.offsetMax = Vector2.zero;

            _tagsText = UIStyleHelper.CreateText(rightGroup.transform, "Tags", "Tags...", 10, t.m_accentGreen, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            RectTransform tgRt = _tagsText.rectTransform;
            tgRt.anchorMin = new Vector2(0.04f, 0.72f);
            tgRt.anchorMax = new Vector2(0.96f, 0.85f);
            tgRt.offsetMin = Vector2.zero;
            tgRt.offsetMax = Vector2.zero;

            _critiqueText = UIStyleHelper.CreateText(rightGroup.transform, "Critique", "Barista critique...", 10, t.m_textSecondary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
            RectTransform cqRt = _critiqueText.rectTransform;
            cqRt.anchorMin = new Vector2(0.04f, 0.04f);
            cqRt.anchorMax = new Vector2(0.96f, 0.70f);
            cqRt.offsetMin = Vector2.zero;
            cqRt.offsetMax = Vector2.zero;

            // Bottom Action Buttons
            Button tweakBtn = UIStyleHelper.CreateButton(cardObj.transform, "TweakBtn", "< TWEAK MOLECULES", t.m_btnSecondary, t.m_textPrimary, () =>
            {
                gameObject.SetActive(false);
                OnTweakSynthesisRequested?.Invoke();
            });
            RectTransform twRt = tweakBtn.GetComponent<RectTransform>();
            twRt.anchorMin = new Vector2(0.04f, 0.03f);
            twRt.anchorMax = new Vector2(0.48f, 0.12f);
            twRt.offsetMin = Vector2.zero;
            twRt.offsetMax = Vector2.zero;

            Button endShiftBtn = UIStyleHelper.CreateButton(cardObj.transform, "EndShiftBtn", "END OF SHIFT // HEAD OUT >", t.m_accentGreen, Color.white, () =>
            {
                gameObject.SetActive(false);
                OnEndOfShiftRequested?.Invoke();
            });
            RectTransform bbRt = endShiftBtn.GetComponent<RectTransform>();
            bbRt.anchorMin = new Vector2(0.52f, 0.03f);
            bbRt.anchorMax = new Vector2(0.96f, 0.12f);
            bbRt.offsetMin = Vector2.zero;
            bbRt.offsetMax = Vector2.zero;
        }

        private UIStatMeter CreateMeter(Transform parent, string name, Color color)
        {
            GameObject meterObj = new GameObject($"Meter_{name}", typeof(RectTransform));
            meterObj.transform.SetParent(parent, false);
            UIStatMeter meter = meterObj.AddComponent<UIStatMeter>();
            meter.Initialize(name, color);
            return meter;
        }

        private bool HasSerializedBindings()
        {
            return _titleText != null && _rankBadgeText != null;
        }

        private void BindExistingHierarchy()
        {
            if (_tweakBtn != null)
            {
                _tweakBtn.onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);
                    OnTweakSynthesisRequested?.Invoke();
                });
            }

            if (_endOfShiftBtn != null)
            {
                _endOfShiftBtn.onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);
                    OnEndOfShiftRequested?.Invoke();
                });
            }

            if (_meterNeuralCalm != null) _meterNeuralCalm.Initialize("Neural Calm", new Color(0.05f, 0.70f, 0.60f, 1f));
            if (_meterSedation != null) _meterSedation.Initialize("Sedation", new Color(0.35f, 0.25f, 0.80f, 1f));
            if (_meterFocus != null) _meterFocus.Initialize("Cognitive Focus", new Color(0.95f, 0.55f, 0.15f, 1f));
            if (_meterEuphoria != null) _meterEuphoria.Initialize("Euphoria", new Color(0.90f, 0.20f, 0.45f, 1f));
            if (_meterSweet != null) _meterSweet.Initialize("Sweetness", new Color(0.95f, 0.75f, 0.20f, 1f));
            if (_meterBitter != null) _meterBitter.Initialize("Bitterness", new Color(0.40f, 0.30f, 0.20f, 1f));
        }

        #endregion


        #region Private and Protected

        [Header("GameObject Serialized Bindings")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private TextMeshProUGUI _rankBadgeText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _critiqueText;
        [SerializeField] private TextMeshProUGUI _tagsText;
        [SerializeField] private Image _drinkHeaderImage;
        [SerializeField] private GameObject _effectBannerObj;
        [SerializeField] private Image _effectBannerImage;
        [SerializeField] private TextMeshProUGUI _effectBannerText;
        [SerializeField] private Button _tweakBtn;
        [SerializeField] private Button _endOfShiftBtn;

        [SerializeField] private UIStatMeter _meterNeuralCalm;
        [SerializeField] private UIStatMeter _meterSedation;
        [SerializeField] private UIStatMeter _meterFocus;
        [SerializeField] private UIStatMeter _meterEuphoria;
        [SerializeField] private UIStatMeter _meterSweet;
        [SerializeField] private UIStatMeter _meterBitter;

        #endregion
    }
}

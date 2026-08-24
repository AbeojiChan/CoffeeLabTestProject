using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    public class UIEndOfShiftDialogue : MonoBehaviour
    {
        #region Publics

        public event Action OnOpenCityMapRequested;

        #endregion


        #region Unity API

        private void Update()
        {
            if (_portraitRing != null && gameObject.activeSelf)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.04f;
                _portraitRing.transform.localScale = new Vector3(pulse, pulse, 1f);
            }
        }

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

        public void StartDialogueSequence(Action onCompleteCallback = null)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            _dialogueLines = new List<string>
            {
                "Phew! That's the final brew order of tonight's shift. The molecular bonds formed beautifully!",
                "Our pantry jars are running dangerously thin on fresh Ethiopian beans, bio-lotus blossoms, and milk emulsions.",
                "Let's take the night metro transit into the district market to restock our reagents before tomorrow's rush!"
            };

            _currentLineIndex = 0;
            DisplayCurrentLine();
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

            GameObject backdrop = UIStyleHelper.CreatePanel(transform, "Backdrop", new Color(0.04f, 0.06f, 0.09f, 0.85f));
            RectTransform bdRt = backdrop.GetComponent<RectTransform>();
            bdRt.anchorMin = Vector2.zero;
            bdRt.anchorMax = Vector2.one;
            bdRt.offsetMin = Vector2.zero;
            bdRt.offsetMax = Vector2.zero;

            GameObject dialogCard = UIStyleHelper.CreatePanel(transform, "DialogCard", t.m_panelBg);
            RectTransform dcRt = dialogCard.GetComponent<RectTransform>();
            dcRt.anchorMin = new Vector2(0.12f, 0.08f);
            dcRt.anchorMax = new Vector2(0.88f, 0.46f);
            dcRt.offsetMin = Vector2.zero;
            dcRt.offsetMax = Vector2.zero;

            GameObject ribbon = UIStyleHelper.CreatePanel(dialogCard.transform, "Ribbon", t.m_headerBg);
            RectTransform rbRt = ribbon.GetComponent<RectTransform>();
            rbRt.anchorMin = new Vector2(0f, 0.84f);
            rbRt.anchorMax = new Vector2(1f, 1f);
            rbRt.offsetMin = Vector2.zero;
            rbRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(ribbon.transform, "HeaderTag", "END OF SHIFT • EVENING NARRATIVE LOG", 11, t.m_accentCyan, TextAlignmentOptions.Left, FontStyles.Bold);
            RectTransform htRt = ribbon.transform.Find("HeaderTag").GetComponent<RectTransform>();
            htRt.anchorMin = new Vector2(0.03f, 0.1f);
            htRt.anchorMax = new Vector2(0.70f, 0.9f);
            htRt.offsetMin = Vector2.zero;
            htRt.offsetMax = Vector2.zero;

            GameObject portraitBox = UIStyleHelper.CreatePanel(dialogCard.transform, "PortraitBox", t.m_wellBg);
            RectTransform pbRt = portraitBox.GetComponent<RectTransform>();
            pbRt.anchorMin = new Vector2(0.03f, 0.10f);
            pbRt.anchorMax = new Vector2(0.25f, 0.78f);
            pbRt.offsetMin = Vector2.zero;
            pbRt.offsetMax = Vector2.zero;

            _portraitRing = UIStyleHelper.CreatePanel(portraitBox.transform, "Ring", t.m_accentCyan, UIStyleHelper.CircleSprite);
            RectTransform prRt = _portraitRing.GetComponent<RectTransform>();
            prRt.anchorMin = new Vector2(0.18f, 0.22f);
            prRt.anchorMax = new Vector2(0.82f, 0.88f);
            prRt.offsetMin = Vector2.zero;
            prRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(portraitBox.transform, "AvatarEmoji", "[ALBA]", 13, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform aeRt = portraitBox.transform.Find("AvatarEmoji").GetComponent<RectTransform>();
            aeRt.anchorMin = new Vector2(0.1f, 0.35f);
            aeRt.anchorMax = new Vector2(0.9f, 0.75f);
            aeRt.offsetMin = Vector2.zero;
            aeRt.offsetMax = Vector2.zero;

            GameObject nameplate = UIStyleHelper.CreatePanel(portraitBox.transform, "Nameplate", t.m_accentAmber);
            RectTransform npRt = nameplate.GetComponent<RectTransform>();
            npRt.anchorMin = new Vector2(0.05f, 0.06f);
            npRt.anchorMax = new Vector2(0.95f, 0.24f);
            npRt.offsetMin = Vector2.zero;
            npRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(nameplate.transform, "Name", "ALBA [HEAD BARISTA]", 9, Color.black, TextAlignmentOptions.Center, FontStyles.Bold);

            GameObject speechBody = UIStyleHelper.CreatePanel(dialogCard.transform, "SpeechBody", t.m_cardBg);
            RectTransform sbRt = speechBody.GetComponent<RectTransform>();
            sbRt.anchorMin = new Vector2(0.27f, 0.10f);
            sbRt.anchorMax = new Vector2(0.97f, 0.78f);
            sbRt.offsetMin = Vector2.zero;
            sbRt.offsetMax = Vector2.zero;

            _dialogueText = UIStyleHelper.CreateText(speechBody.transform, "Text", "Dialogue...", 18, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
            RectTransform dtRt = _dialogueText.rectTransform;
            dtRt.anchorMin = new Vector2(0.04f, 0.28f);
            dtRt.anchorMax = new Vector2(0.96f, 0.92f);
            dtRt.offsetMin = Vector2.zero;
            dtRt.offsetMax = Vector2.zero;

            _advanceButton = UIStyleHelper.CreateButton(speechBody.transform, "AdvanceBtn", "NEXT >", t.m_btnPrimary, Color.white, OnAdvanceClicked);
            RectTransform abRt = _advanceButton.GetComponent<RectTransform>();
            abRt.anchorMin = new Vector2(0.65f, 0.08f);
            abRt.anchorMax = new Vector2(0.96f, 0.30f);
            abRt.offsetMin = Vector2.zero;
            abRt.offsetMax = Vector2.zero;

            _advanceButtonText = _advanceButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void DisplayCurrentLine()
        {
            if (_currentLineIndex < 0 || _currentLineIndex >= _dialogueLines.Count) return;

            string targetText = _dialogueLines[_currentLineIndex];

            if (_typewriterRoutine != null)
            {
                StopCoroutine(_typewriterRoutine);
            }
            _typewriterRoutine = StartCoroutine(TypewriterRoutine(targetText));

            bool isLast = _currentLineIndex == _dialogueLines.Count - 1;
            _advanceButtonText.text = isLast ? "OPEN DISTRICT MAP >" : "NEXT >";
            _advanceButton.GetComponent<Image>().color = isLast ? UIStyleHelper.Theme.m_accentGreen : UIStyleHelper.Theme.m_btnPrimary;
        }

        private IEnumerator TypewriterRoutine(string line)
        {
            _isTyping = true;
            _dialogueText.text = "";

            for (int i = 0; i <= line.Length; i++)
            {
                _dialogueText.text = line.Substring(0, i);
                yield return new WaitForSecondsRealtime(0.015f);
            }

            _isTyping = false;
        }

        private void OnAdvanceClicked()
        {
            if (_isTyping)
            {
                if (_typewriterRoutine != null) StopCoroutine(_typewriterRoutine);
                _dialogueText.text = _dialogueLines[_currentLineIndex];
                _isTyping = false;
                return;
            }

            _currentLineIndex++;
            if (_currentLineIndex < _dialogueLines.Count)
            {
                DisplayCurrentLine();
            }
            else
            {
                gameObject.SetActive(false);
                OnOpenCityMapRequested?.Invoke();
            }
        }

        private bool HasSerializedBindings()
        {
            return _dialogueText != null && _advanceButton != null;
        }

        private void BindExistingHierarchy()
        {
            if (_advanceButton != null)
            {
                _advanceButton.onClick.AddListener(OnAdvanceClicked);
            }
        }

        #endregion


        #region Private and Protected

        [Header("GameObject Serialized Bindings")]
        [SerializeField] private GameObject _portraitRing;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private Button _advanceButton;
        [SerializeField] private TextMeshProUGUI _advanceButtonText;

        private List<string> _dialogueLines = new List<string>();
        private int _currentLineIndex = 0;
        private Coroutine _typewriterRoutine;
        private bool _isTyping;

        #endregion
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public enum RetractDirection
    {
        SlideLeft,
        SlideRight
    }

    public class RetractableSidePanel : MonoBehaviour
    {
        #region Publics

        [Header("Configuration")]
        public RetractDirection m_direction = RetractDirection.SlideLeft;
        public float m_retractDistance = 420f;
        public float m_transitionDuration = 0.28f;
        public bool m_startOpened = false;

        [Header("References")]
        public RectTransform m_panelTransform;
        public Button m_pullTabButton;
        public Image m_pullTabIcon;

        public bool IsOpened => _isOpened;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_panelTransform == null)
            {
                m_panelTransform = GetComponent<RectTransform>();
            }

            if (m_pullTabButton != null)
            {
                m_pullTabButton.onClick.AddListener(Toggle);
            }

            _isOpened = m_startOpened;
            _openedPos = m_panelTransform.anchoredPosition;

            float sign = (m_direction == RetractDirection.SlideLeft) ? -1f : 1f;
            _closedPos = _openedPos + new Vector2(sign * m_retractDistance, 0f);

            if (!_isOpened)
            {
                m_panelTransform.anchoredPosition = _closedPos;
            }
        }

        #endregion


        #region Main API

        public void Toggle()
        {
            SetState(!_isOpened);
        }

        public void SetState(bool open)
        {
            _isOpened = open;

            if (_slideRoutine != null)
            {
                StopCoroutine(_slideRoutine);
            }

            _slideRoutine = StartCoroutine(SlideRoutine(_isOpened ? _openedPos : _closedPos));
        }

        #endregion


        #region Tools and Utilities

        private IEnumerator SlideRoutine(Vector2 targetPos)
        {
            Vector2 startPos = m_panelTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < m_transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / m_transitionDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                m_panelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            m_panelTransform.anchoredPosition = targetPos;
            _slideRoutine = null;
        }

        #endregion


        #region Private and Protected

        private bool _isOpened = true;
        private Vector2 _openedPos;
        private Vector2 _closedPos;
        private Coroutine _slideRoutine;

        #endregion
    }
}

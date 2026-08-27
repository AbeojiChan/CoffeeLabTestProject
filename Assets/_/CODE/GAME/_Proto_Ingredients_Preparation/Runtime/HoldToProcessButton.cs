using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class HoldToProcessButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        #region Publics

        [Header("Settings")]
        public float m_holdDuration = 2.0f;
        public Image m_fillProgressImage;
        public Text m_statusText;
        public string m_idlePrompt = "HOLD TO PROCESS";
        public string m_processingPrompt = "PROCESSING...";
        public string m_completedPrompt = "PROCESS COMPLETE";

        [Header("Colors")]
        public Color m_idleColor = new Color(0.20f, 0.70f, 0.40f, 1f);
        public Color m_activeColor = new Color(0.95f, 0.65f, 0.15f, 1f);
        public Color m_completedColor = new Color(0.15f, 0.85f, 0.95f, 1f);

        public event Action OnHoldComplete;
        public event Action<float> OnProgressChanged;

        public bool IsCompleted => _isCompleted;

        #endregion


        #region Unity API

        private void Start()
        {
            ResetButton();
        }

        private void Update()
        {
            if (_isCompleted) return;

            if (_isHolding)
            {
                _holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_holdTimer / Mathf.Max(0.1f, m_holdDuration));
                UpdateUI(progress);
                OnProgressChanged?.Invoke(progress);

                if (_holdTimer >= m_holdDuration)
                {
                    _isHolding = false;
                    _isCompleted = true;
                    UpdateUI(1.0f);
                    OnHoldComplete?.Invoke();
                }
            }
            else if (_holdTimer > 0f)
            {
                // Smooth decay if released early
                _holdTimer = Mathf.Max(0f, _holdTimer - Time.deltaTime * 2.5f);
                float progress = Mathf.Clamp01(_holdTimer / Mathf.Max(0.1f, m_holdDuration));
                UpdateUI(progress);
                OnProgressChanged?.Invoke(progress);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isCompleted) return;
            _isHolding = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHolding = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHolding = false;
        }

        #endregion


        #region Main API

        public void ResetButton()
        {
            _isHolding = false;
            _isCompleted = false;
            _holdTimer = 0f;
            UpdateUI(0f);
        }

        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            if (!interactable)
            {
                ResetButton();
            }
        }

        #endregion


        #region Tools and Utilities

        private void UpdateUI(float progress)
        {
            if (m_fillProgressImage != null)
            {
                m_fillProgressImage.fillAmount = progress;
                if (_isCompleted)
                {
                    m_fillProgressImage.color = m_completedColor;
                }
                else if (_isHolding)
                {
                    m_fillProgressImage.color = m_activeColor;
                }
                else
                {
                    m_fillProgressImage.color = m_idleColor;
                }
            }

            if (m_statusText != null)
            {
                if (_isCompleted)
                {
                    m_statusText.text = m_completedPrompt;
                }
                else if (_isHolding)
                {
                    m_statusText.text = $"{m_processingPrompt} ({Mathf.RoundToInt(progress * 100)}%)";
                }
                else
                {
                    m_statusText.text = m_idlePrompt;
                }
            }
        }

        #endregion


        #region Private and Protected

        private bool _isHolding = false;
        private bool _isCompleted = false;
        private bool _isInteractable = true;
        private float _holdTimer = 0f;

        #endregion
    }
}

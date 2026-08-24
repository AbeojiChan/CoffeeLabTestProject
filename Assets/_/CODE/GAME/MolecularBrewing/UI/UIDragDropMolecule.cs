using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIDragDropMolecule : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        #region Publics

        public MoleculeType m_moleculeType;
        public MoleculeDefinition m_definition;
        public UIGridBondSlot m_currentSlot;
        public Transform m_leftPoolParent;

        public event Action<UIDragDropMolecule> OnStateChanged;

        #endregion


        #region Unity API

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 0.8f;
            _canvasGroup.blocksRaycasts = false;

            if (m_currentSlot != null)
            {
                m_currentSlot.ClearMolecule();
                m_currentSlot = null;
            }

            if (_dragLayer != null)
            {
                transform.SetParent(_dragLayer, true);
            }
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rootCanvas != null)
            {
                _rectTransform.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;
            }
            else
            {
                _rectTransform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            // If not placed in a valid slot, return to Left Pool
            if (m_currentSlot == null)
            {
                ReturnToPool();
            }

            OnStateChanged?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Right click or click to return to pool if slotted
            if (eventData.button == PointerEventData.InputButton.Right || m_currentSlot != null)
            {
                ReturnToPool();
            }
        }

        #endregion


        #region Main API

        public void Initialize(MoleculeDefinition def, Transform leftPool, Transform dragLayer, Canvas rootCanvas)
        {
            m_definition = def;
            m_moleculeType = def.m_type;
            m_leftPoolParent = leftPool;
            _dragLayer = dragLayer;
            _rootCanvas = rootCanvas;

            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            BuildVisuals();
        }

        public void SnapToSlot(UIGridBondSlot slot)
        {
            if (m_currentSlot != null && m_currentSlot != slot)
            {
                m_currentSlot.ClearMolecule();
            }

            m_currentSlot = slot;
            transform.SetParent(slot.transform, false);
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.anchoredPosition = Vector2.zero;

            OnStateChanged?.Invoke(this);
        }

        public void ReturnToPool()
        {
            if (m_currentSlot != null)
            {
                m_currentSlot.ClearMolecule();
                m_currentSlot = null;
            }

            if (m_leftPoolParent != null)
            {
                transform.SetParent(m_leftPoolParent, false);
            }

            OnStateChanged?.Invoke(this);
        }

        #endregion


        #region Tools and Utilities

        private void BuildVisuals()
        {
            _rectTransform.sizeDelta = new Vector2(76, 76);

            // Background circle
            _bgImage = GetComponent<Image>();
            if (_bgImage == null) _bgImage = gameObject.AddComponent<Image>();
            _bgImage.sprite = UIStyleHelper.CircleSprite;
            _bgImage.color = m_definition.m_primaryColor;

            // Outer ring
            GameObject ringObj = new GameObject("Ring", typeof(RectTransform), typeof(Image));
            ringObj.transform.SetParent(transform, false);
            _ringImage = ringObj.GetComponent<Image>();
            _ringImage.sprite = UIStyleHelper.CircleSprite;
            _ringImage.color = m_definition.m_accentColor;
            RectTransform ringRt = ringObj.GetComponent<RectTransform>();
            ringRt.anchorMin = Vector2.zero;
            ringRt.anchorMax = Vector2.one;
            ringRt.sizeDelta = new Vector2(6, 6);
            ringObj.transform.SetAsFirstSibling();

            // Formula Text (e.g. H2O, C8H10N4O2)
            _formulaText = UIStyleHelper.CreateText(
                transform,
                "Formula",
                m_definition.m_formula,
                14,
                Color.white,
                TextAlignmentOptions.Center,
                FontStyles.Bold
            );
            RectTransform fRt = _formulaText.rectTransform;
            fRt.anchorMin = new Vector2(0, 0.35f);
            fRt.anchorMax = new Vector2(1, 0.85f);
            fRt.offsetMin = Vector2.zero;
            fRt.offsetMax = Vector2.zero;

            // Short Name Text
            _nameText = UIStyleHelper.CreateText(
                transform,
                "Name",
                m_definition.m_displayName,
                9,
                new Color(1f, 1f, 1f, 0.85f),
                TextAlignmentOptions.Center,
                FontStyles.Normal
            );
            RectTransform nRt = _nameText.rectTransform;
            nRt.anchorMin = new Vector2(0, 0.1f);
            nRt.anchorMax = new Vector2(1, 0.4f);
            nRt.offsetMin = Vector2.zero;
            nRt.offsetMax = Vector2.zero;
        }

        #endregion


        #region Private and Protected

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Canvas _rootCanvas;
        private Transform _dragLayer;

        private Image _bgImage;
        private Image _ringImage;
        private TextMeshProUGUI _formulaText;
        private TextMeshProUGUI _nameText;

        #endregion
    }
}

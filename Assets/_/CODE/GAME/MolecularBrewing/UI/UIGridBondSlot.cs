using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MolecularBrewing.Runtime
{
    public class UIGridBondSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Publics

        public int m_gridX;
        public int m_gridY;
        public UIDragDropMolecule m_slottedMolecule;
        public bool m_hasMolecule => m_slottedMolecule != null;

        public event Action<UIGridBondSlot> OnSlotChanged;

        #endregion


        #region Unity API

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                UIDragDropMolecule molecule = eventData.pointerDrag.GetComponent<UIDragDropMolecule>();
                if (molecule != null)
                {
                    // If this slot is already occupied, swap or replace
                    if (m_slottedMolecule != null && m_slottedMolecule != molecule)
                    {
                        m_slottedMolecule.ReturnToPool();
                    }

                    m_slottedMolecule = molecule;
                    molecule.SnapToSlot(this);
                    UpdateVisualState();
                    OnSlotChanged?.Invoke(this);
                }
            }
            if (_hoverHighlight != null) _hoverHighlight.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging && _hoverHighlight != null)
            {
                _hoverHighlight.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_hoverHighlight != null)
            {
                _hoverHighlight.gameObject.SetActive(false);
            }
        }

        #endregion


        #region Main API

        public void Initialize(int x, int y)
        {
            m_gridX = x;
            m_gridY = y;

            BuildVisuals();
        }

        public void SetRightBondVisible(bool visible, Color bondColor)
        {
            if (_rightBond != null)
            {
                _rightBond.gameObject.SetActive(visible);
                _rightBond.color = bondColor;
            }
        }

        public void SetBottomBondVisible(bool visible, Color bondColor)
        {
            if (_bottomBond != null)
            {
                _bottomBond.gameObject.SetActive(visible);
                _bottomBond.color = bondColor;
            }
        }

        public void ClearMolecule()
        {
            m_slottedMolecule = null;
            UpdateVisualState();
            OnSlotChanged?.Invoke(this);
        }

        #endregion


        #region Tools and Utilities

        private void BuildVisuals()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null) rt = gameObject.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(90, 90);

            _slotBg = GetComponent<Image>();
            if (_slotBg == null) _slotBg = gameObject.AddComponent<Image>();
            _slotBg.sprite = UIStyleHelper.RoundedSprite;
            _slotBg.type = Image.Type.Sliced;
            _slotBg.color = _normalColor;

            // Hover ring
            GameObject hoverObj = UIStyleHelper.CreatePanel(transform, "HoverHighlight", _hoverColor, UIStyleHelper.RoundedSprite);
            _hoverHighlight = hoverObj.GetComponent<Image>();
            RectTransform hRt = _hoverHighlight.rectTransform;
            hRt.anchorMin = Vector2.zero;
            hRt.anchorMax = Vector2.one;
            hRt.sizeDelta = new Vector2(4, 4);
            _hoverHighlight.gameObject.SetActive(false);

            // Right Bond line
            GameObject rBondObj = UIStyleHelper.CreatePanel(transform, "RightBond", new Color(0.3f, 0.7f, 0.9f, 0.8f), UIStyleHelper.RoundedSprite);
            _rightBond = rBondObj.GetComponent<Image>();
            RectTransform rRt = _rightBond.rectTransform;
            rRt.anchorMin = new Vector2(1f, 0.5f);
            rRt.anchorMax = new Vector2(1f, 0.5f);
            rRt.pivot = new Vector2(0f, 0.5f);
            rRt.sizeDelta = new Vector2(16, 6);
            rRt.anchoredPosition = Vector2.zero;
            _rightBond.gameObject.SetActive(false);

            // Bottom Bond line
            GameObject bBondObj = UIStyleHelper.CreatePanel(transform, "BottomBond", new Color(0.3f, 0.7f, 0.9f, 0.8f), UIStyleHelper.RoundedSprite);
            _bottomBond = bBondObj.GetComponent<Image>();
            RectTransform bRt = _bottomBond.rectTransform;
            bRt.anchorMin = new Vector2(0.5f, 0f);
            bRt.anchorMax = new Vector2(0.5f, 0f);
            bRt.pivot = new Vector2(0.5f, 1f);
            bRt.sizeDelta = new Vector2(6, 16);
            bRt.anchoredPosition = Vector2.zero;
            _bottomBond.gameObject.SetActive(false);
        }

        private void UpdateVisualState()
        {
            if (_slotBg != null)
            {
                _slotBg.color = m_hasMolecule ? _occupiedColor : _normalColor;
            }
        }

        #endregion


        #region Private and Protected

        private Image _slotBg;
        private Image _hoverHighlight;
        private Color _normalColor = new Color(0.18f, 0.22f, 0.26f, 0.75f);
        private Color _hoverColor = new Color(0.35f, 0.65f, 0.85f, 0.95f);
        private Color _occupiedColor = new Color(0.12f, 0.16f, 0.20f, 0.90f);

        private Image _rightBond;
        private Image _bottomBond;

        #endregion
    }
}

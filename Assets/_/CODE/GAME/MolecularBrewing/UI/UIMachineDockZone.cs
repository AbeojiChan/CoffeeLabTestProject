using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    public class UIMachineDockZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Publics

        public event Action<List<IngredientDefinition>> OnDockTriggered;

        #endregion


        #region Unity API

        public void OnPointerEnter(PointerEventData eventData)
        {
            var t = UIStyleHelper.Theme;
            if (_dockBorder != null)
            {
                _dockBorder.color = t.m_accentCyan;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var t = UIStyleHelper.Theme;
            if (_dockBorder != null)
            {
                _dockBorder.color = t.m_lensBezel;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                var beaker = eventData.pointerDrag.GetComponent<UIBeakerDropZone>();
                if (beaker != null && beaker.m_dosedIngredients.Count > 0)
                {
                    OnDockTriggered?.Invoke(new List<IngredientDefinition>(beaker.m_dosedIngredients));
                }
            }
        }

        #endregion


        #region Main API

        public void Initialize()
        {
            BuildVisuals();
        }

        public void SetDockReady(bool ready, int doseCount)
        {
            var t = UIStyleHelper.Theme;
            _actionButton.interactable = ready;
            if (ready)
            {
                _statusText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(t.m_accentGreen)}>BEAKER LOADED ({doseCount} Doses)</color>\nReady for Molecular Extraction";
                _actionButtonText.text = "LOCK IN & SYNTHESIZE >";
                _dockInteriorImage.color = t.m_cardBg;
            }
            else
            {
                _statusText.text = "<color=#667788>DOCKING CHAMBER IDLE</color>\nDose Beaker & Drag Here";
                _actionButtonText.text = "INSERT BEAKER >";
                _dockInteriorImage.color = t.m_cardBg;
            }
        }

        #endregion


        #region Tools and Utilities

        private void BuildVisuals()
        {
            var t = UIStyleHelper.Theme;

            RectTransform rootRt = GetComponent<RectTransform>();
            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();

            // Outer Frame
            _dockBorder = GetComponent<Image>();
            if (_dockBorder == null) _dockBorder = gameObject.AddComponent<Image>();
            _dockBorder.sprite = UIStyleHelper.RoundedSprite;
            _dockBorder.type = Image.Type.Sliced;
            _dockBorder.color = t.m_lensBezel;

            // Machine Aperture / Slot Interior
            GameObject interior = UIStyleHelper.CreatePanel(transform, "DockInterior", t.m_cardBg);
            _dockInteriorImage = interior.GetComponent<Image>();
            RectTransform inRt = interior.GetComponent<RectTransform>();
            inRt.anchorMin = new Vector2(0.04f, 0.04f);
            inRt.anchorMax = new Vector2(0.96f, 0.96f);
            inRt.offsetMin = Vector2.zero;
            inRt.offsetMax = Vector2.zero;

            // Machine Title Header
            UIStyleHelper.CreateText(interior.transform, "MachineTitle", "CYBER-SYNTHESIZER\nEXTRACTION DOCK", 13, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform mtRt = interior.transform.Find("MachineTitle").GetComponent<RectTransform>();
            mtRt.anchorMin = new Vector2(0.05f, 0.78f);
            mtRt.anchorMax = new Vector2(0.95f, 0.96f);
            mtRt.offsetMin = Vector2.zero;
            mtRt.offsetMax = Vector2.zero;

            // Chamber Port Graphic (Hexagon / Tube graphic frame)
            GameObject portHole = UIStyleHelper.CreatePanel(interior.transform, "PortHole", t.m_wellBg);
            RectTransform phRt = portHole.GetComponent<RectTransform>();
            phRt.anchorMin = new Vector2(0.15f, 0.32f);
            phRt.anchorMax = new Vector2(0.85f, 0.75f);
            phRt.offsetMin = Vector2.zero;
            phRt.offsetMax = Vector2.zero;

            // Port Icon / Tube Ring
            GameObject ring = UIStyleHelper.CreatePanel(portHole.transform, "Ring", new Color(t.m_accentCyan.r, t.m_accentCyan.g, t.m_accentCyan.b, 0.25f), UIStyleHelper.CircleSprite);
            RectTransform rRt = ring.GetComponent<RectTransform>();
            rRt.anchorMin = new Vector2(0.20f, 0.15f);
            rRt.anchorMax = new Vector2(0.80f, 0.85f);
            rRt.offsetMin = Vector2.zero;
            rRt.offsetMax = Vector2.zero;

            // Status Description
            _statusText = UIStyleHelper.CreateText(portHole.transform, "Status", "DOCKING CHAMBER IDLE\nDose Beaker & Drag Here", 10, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Normal);
            RectTransform stRt = _statusText.rectTransform;
            stRt.anchorMin = Vector2.zero;
            stRt.anchorMax = Vector2.one;
            stRt.offsetMin = new Vector2(10, 10);
            stRt.offsetMax = new Vector2(-10, -10);

            // Trigger Button at Bottom
            _actionButton = UIStyleHelper.CreateButton(interior.transform, "ActionButton", "INSERT BEAKER >", t.m_accentGreen, Color.white, () =>
            {
                var beaker = FindFirstObjectByType<UIBeakerDropZone>();
                if (beaker != null && beaker.m_dosedIngredients.Count > 0)
                {
                    OnDockTriggered?.Invoke(new List<IngredientDefinition>(beaker.m_dosedIngredients));
                }
            });
            RectTransform abRt = _actionButton.GetComponent<RectTransform>();
            abRt.anchorMin = new Vector2(0.08f, 0.06f);
            abRt.anchorMax = new Vector2(0.92f, 0.22f);
            abRt.offsetMin = Vector2.zero;
            abRt.offsetMax = Vector2.zero;

            _actionButtonText = _actionButton.GetComponentInChildren<TextMeshProUGUI>();

            SetDockReady(false, 0);
        }

        #endregion


        #region Private and Protected

        private Image _dockBorder;
        private Image _dockInteriorImage;
        private TextMeshProUGUI _statusText;
        private Button _actionButton;
        private TextMeshProUGUI _actionButtonText;

        #endregion
    }
}

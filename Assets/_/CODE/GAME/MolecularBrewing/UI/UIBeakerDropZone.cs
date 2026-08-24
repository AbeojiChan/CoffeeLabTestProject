//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
//    public class UIBeakerDropZone : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
//    {
//        #region Publics

//        public const int MAX_DOSES = 7;
//        public List<IngredientDefinition> m_dosedIngredients = new List<IngredientDefinition>();
//        public RectTransform m_rectTransform;

//        public event Action<List<IngredientDefinition>> OnDosingChanged;
//        public event Action<IngredientDefinition> OnDoseRemoved;
//        public event Action<List<IngredientDefinition>> OnBeakerCleared;
//        public event Action<UIBeakerDropZone, PointerEventData> OnBeakerDroppedOnMachine;

//        #endregion


//        #region Unity API

//        public void OnPointerEnter(PointerEventData eventData)
//        {
//            var t = UIStyleHelper.Theme;
//            if (_beakerBorder != null)
//            {
//                _beakerBorder.color = t.m_accentCyan;
//            }
//        }

//        public void OnPointerExit(PointerEventData eventData)
//        {
//            var t = UIStyleHelper.Theme;
//            if (_beakerBorder != null)
//            {
//                _beakerBorder.color = t.m_lensBezel;
//            }
//        }

//        public void OnDrop(PointerEventData eventData)
//        {
//            if (eventData.pointerDrag != null)
//            {
//                var draggable = eventData.pointerDrag.GetComponent<UIDraggableIngredientItem>();
//                if (draggable != null && draggable.m_ingredient != null && draggable.m_stockCount > 0)
//                {
//                    AddDose(draggable.m_ingredient);
//                }
//            }
//        }

//        public void OnBeginDrag(PointerEventData eventData)
//        {
//            if (eventData.button != PointerEventData.InputButton.Left || m_dosedIngredients.Count == 0) return;

//            _isDragging = true;
//            _canvasGroup.alpha = 0.5f;
//            _canvasGroup.blocksRaycasts = false;

//            CreateBeakerDragGhost();
//        }

//        public void OnDrag(PointerEventData eventData)
//        {
//            if (!_isDragging || _dragGhost == null) return;

//            if (_rootCanvas != null)
//            {
//                _dragGhostRect.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;
//            }
//            else
//            {
//                _dragGhostRect.position = eventData.position;
//            }
//        }

//        public void OnEndDrag(PointerEventData eventData)
//        {
//            if (!_isDragging) return;

//            _isDragging = false;
//            _canvasGroup.alpha = 1f;
//            _canvasGroup.blocksRaycasts = true;

//            if (_dragGhost != null)
//            {
//                Destroy(_dragGhost);
//                _dragGhost = null;
//            }

//            OnBeakerDroppedOnMachine?.Invoke(this, eventData);
//        }

//        #endregion


//        #region Main API

//        public void Initialize(Canvas rootCanvas)
//        {
//            _rootCanvas = rootCanvas;
//            m_rectTransform = GetComponent<RectTransform>();
//            _canvasGroup = GetComponent<CanvasGroup>();

//            BuildVisuals();
//            RefreshBeakerState();
//        }

//        public bool AddDose(IngredientDefinition ingredient)
//        {
//            if (m_dosedIngredients.Count >= MAX_DOSES) return false;

//            m_dosedIngredients.Add(ingredient);
//            RefreshBeakerState();
//            OnDosingChanged?.Invoke(m_dosedIngredients);
//            return true;
//        }

//        public void RemoveDoseAt(int index)
//        {
//            if (index >= 0 && index < m_dosedIngredients.Count)
//            {
//                var removed = m_dosedIngredients[index];
//                m_dosedIngredients.RemoveAt(index);
//                RefreshBeakerState();
//                OnDoseRemoved?.Invoke(removed);
//                OnDosingChanged?.Invoke(m_dosedIngredients);
//            }
//        }

//        public void ClearBeaker(bool notifyRefund = true)
//        {
//            if (notifyRefund && m_dosedIngredients.Count > 0)
//            {
//                OnBeakerCleared?.Invoke(new List<IngredientDefinition>(m_dosedIngredients));
//            }
//            m_dosedIngredients.Clear();
//            RefreshBeakerState();
//            OnDosingChanged?.Invoke(m_dosedIngredients);
//        }

//        public int GetDoseCount() => m_dosedIngredients.Count;

//        #endregion


//        #region Tools and Utilities

//        private void BuildVisuals()
//        {
//            var t = UIStyleHelper.Theme;

//            // Outer Beaker Station Frame
//            _beakerBorder = GetComponent<Image>();
//            if (_beakerBorder == null) _beakerBorder = gameObject.AddComponent<Image>();
//            _beakerBorder.sprite = UIStyleHelper.RoundedSprite;
//            _beakerBorder.type = Image.Type.Sliced;
//            _beakerBorder.color = t.m_lensBezel;

//            // Glass Cylinder Interior
//            GameObject glassInterior = UIStyleHelper.CreatePanel(transform, "GlassInterior", t.m_lensGlass);
//            RectTransform giRt = glassInterior.GetComponent<RectTransform>();
//            giRt.anchorMin = new Vector2(0.04f, 0.04f);
//            giRt.anchorMax = new Vector2(0.96f, 0.96f);
//            giRt.offsetMin = Vector2.zero;
//            giRt.offsetMax = Vector2.zero;

//            // Liquid Fill Column (Scales with dose count)
//            GameObject fluidObj = UIStyleHelper.CreatePanel(glassInterior.transform, "LiquidColumn", t.m_beakerFill);
//            _liquidImage = fluidObj.GetComponent<Image>();
//            _liquidRect = fluidObj.GetComponent<RectTransform>();
//            _liquidRect.anchorMin = new Vector2(0.05f, 0.05f);
//            _liquidRect.anchorMax = new Vector2(0.95f, 0.05f);
//            _liquidRect.pivot = new Vector2(0.5f, 0f);
//            _liquidRect.offsetMin = Vector2.zero;
//            _liquidRect.offsetMax = Vector2.zero;

//            // Graduated Measurement Ticks Container
//            GameObject ticksObj = new GameObject("Ticks", typeof(RectTransform));
//            ticksObj.transform.SetParent(glassInterior.transform, false);
//            RectTransform tkRt = ticksObj.GetComponent<RectTransform>();
//            tkRt.anchorMin = Vector2.zero;
//            tkRt.anchorMax = Vector2.one;
//            tkRt.offsetMin = Vector2.zero;
//            tkRt.offsetMax = Vector2.zero;

//            for (int i = 1; i <= MAX_DOSES; i++)
//            {
//                float normY = (float)i / MAX_DOSES;
//                GameObject tick = UIStyleHelper.CreatePanel(ticksObj.transform, $"Tick_{i}", new Color(t.m_lensBezel.r, t.m_lensBezel.g, t.m_lensBezel.b, 0.6f));
//                RectTransform tRt = tick.GetComponent<RectTransform>();
//                tRt.anchorMin = new Vector2(0.05f, normY * 0.90f + 0.05f);
//                tRt.anchorMax = new Vector2(0.35f, normY * 0.90f + 0.05f);
//                tRt.sizeDelta = new Vector2(0, 2);

//                int volumeMl = i * 50;
//                TextMeshProUGUI markTxt = UIStyleHelper.CreateText(ticksObj.transform, $"Mark_{i}", $"{volumeMl}mL", 8, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform mtRt = markTxt.rectTransform;
//                mtRt.anchorMin = new Vector2(0.38f, normY * 0.90f);
//                mtRt.anchorMax = new Vector2(0.95f, normY * 0.90f + 0.08f);
//                mtRt.offsetMin = Vector2.zero;
//                mtRt.offsetMax = Vector2.zero;
//            }

//            // Beaker Header Label
//            _volumeLabelText = UIStyleHelper.CreateText(glassInterior.transform, "VolumeLabel", "0 / 350 mL", 11, t.m_accentCyan, TextAlignmentOptions.TopRight, FontStyles.Bold);
//            RectTransform vlRt = _volumeLabelText.rectTransform;
//            vlRt.anchorMin = new Vector2(0.40f, 0.90f);
//            vlRt.anchorMax = new Vector2(0.96f, 0.98f);
//            vlRt.offsetMin = Vector2.zero;
//            vlRt.offsetMax = Vector2.zero;

//            // Center Drop Prompt (Shown when empty)
//            _emptyPromptText = UIStyleHelper.CreateText(glassInterior.transform, "EmptyPrompt", "DRAG REAGENTS HERE\nTO DOSE BEAKER\n(Up to 7 Doses)", 11, t.m_textMuted, TextAlignmentOptions.Center, FontStyles.Normal);
//            RectTransform epRt = _emptyPromptText.rectTransform;
//            epRt.anchorMin = new Vector2(0.1f, 0.30f);
//            epRt.anchorMax = new Vector2(0.9f, 0.70f);
//            epRt.offsetMin = Vector2.zero;
//            epRt.offsetMax = Vector2.zero;

//            // Dosed Reagent Badges Container (Overlaid at top of beaker)
//            GameObject badgeList = new GameObject("DosedList", typeof(RectTransform), typeof(VerticalLayoutGroup));
//            badgeList.transform.SetParent(glassInterior.transform, false);
//            _dosedBadgeContainer = badgeList.transform;
//            RectTransform blRt = badgeList.GetComponent<RectTransform>();
//            blRt.anchorMin = new Vector2(0.08f, 0.08f);
//            blRt.anchorMax = new Vector2(0.92f, 0.88f);
//            blRt.offsetMin = Vector2.zero;
//            blRt.offsetMax = Vector2.zero;

//            VerticalLayoutGroup vlg = badgeList.GetComponent<VerticalLayoutGroup>();
//            vlg.spacing = 3;
//            vlg.childControlWidth = true;
//            vlg.childControlHeight = false;
//        }

//        private void RefreshBeakerState()
//        {
//            int count = m_dosedIngredients.Count;
//            float fillRatio = (float)count / MAX_DOSES;

//            // Update Fluid Column Height & Blended Color
//            _liquidRect.anchorMax = new Vector2(0.95f, Mathf.Clamp01(fillRatio * 0.90f + 0.05f));

//            if (count > 0)
//            {
//                Color blended = Color.black;
//                foreach (var ing in m_dosedIngredients)
//                {
//                    blended += ing.m_iconColor;
//                }
//                blended /= count;
//                blended.a = 0.85f;
//                _liquidImage.color = blended;
//                _emptyPromptText.gameObject.SetActive(false);
//            }
//            else
//            {
//                _liquidImage.color = Color.clear;
//                _emptyPromptText.gameObject.SetActive(true);
//            }

//            _volumeLabelText.text = $"{count * 50} / {MAX_DOSES * 50} mL ({count}/{MAX_DOSES} Doses)";

//            // Rebuild Dosed Badge Items
//            foreach (Transform child in _dosedBadgeContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            for (int i = 0; i < count; i++)
//            {
//                int index = i;
//                var ing = m_dosedIngredients[i];

//                GameObject badge = UIStyleHelper.CreatePanel(_dosedBadgeContainer, $"Dose_{i}", new Color(0.10f, 0.14f, 0.18f, 0.85f));
//                RectTransform bRt = badge.GetComponent<RectTransform>();
//                bRt.sizeDelta = new Vector2(0, 20);

//                // Color pip
//                GameObject pip = UIStyleHelper.CreatePanel(badge.transform, "Pip", ing.m_iconColor, UIStyleHelper.CircleSprite);
//                RectTransform pRt = pip.GetComponent<RectTransform>();
//                pRt.anchorMin = new Vector2(0.02f, 0.20f);
//                pRt.anchorMax = new Vector2(0.10f, 0.80f);
//                pRt.offsetMin = Vector2.zero;
//                pRt.offsetMax = Vector2.zero;

//                // Name
//                TextMeshProUGUI nameTxt = UIStyleHelper.CreateText(badge.transform, "Name", ing.m_displayName, 8, Color.white, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform nRt = nameTxt.rectTransform;
//                nRt.anchorMin = new Vector2(0.12f, 0.05f);
//                nRt.anchorMax = new Vector2(0.85f, 0.95f);
//                nRt.offsetMin = Vector2.zero;
//                nRt.offsetMax = Vector2.zero;

//                // Remove [X] button
//                Button removeBtn = UIStyleHelper.CreateButton(badge.transform, "RemoveBtn", "x", new Color(0.35f, 0.15f, 0.15f, 1f), Color.white, () => RemoveDoseAt(index));
//                RectTransform rRt = removeBtn.GetComponent<RectTransform>();
//                rRt.anchorMin = new Vector2(0.88f, 0.10f);
//                rRt.anchorMax = new Vector2(0.98f, 0.90f);
//                rRt.offsetMin = Vector2.zero;
//                rRt.offsetMax = Vector2.zero;
//            }
//        }

//        private void CreateBeakerDragGhost()
//        {
//            if (_rootCanvas == null) return;

//            _dragGhost = new GameObject("DragGhost_Beaker", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            _dragGhost.transform.SetParent(_rootCanvas.transform, false);
//            _dragGhost.transform.SetAsLastSibling();

//            _dragGhostRect = _dragGhost.GetComponent<RectTransform>();
//            _dragGhostRect.sizeDelta = new Vector2(160, 180);

//            Vector2 screenPos = Input.mousePosition;
//            if (_rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
//            {
//                _dragGhostRect.position = screenPos;
//            }
//            else
//            {
//                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvas.GetComponent<RectTransform>(), screenPos, _rootCanvas.worldCamera, out Vector2 localPoint);
//                _dragGhostRect.anchoredPosition = localPoint;
//            }

//            Image ghostImg = _dragGhost.GetComponent<Image>();
//            ghostImg.sprite = UIStyleHelper.RoundedSprite;
//            ghostImg.type = Image.Type.Sliced;
//            ghostImg.color = new Color(0.20f, 0.85f, 0.75f, 0.75f);

//            CanvasGroup cg = _dragGhost.GetComponent<CanvasGroup>();
//            cg.blocksRaycasts = false;
//            cg.alpha = 0.85f;

//            UIStyleHelper.CreateText(_dragGhost.transform, "GhostText", $"BEAKER ({m_dosedIngredients.Count} Doses)\nDrag to Machine Slot", 10, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//        }

//        #endregion


//        #region Private and Protected

//        private CanvasGroup _canvasGroup;
//        private Canvas _rootCanvas;
//        private Image _beakerBorder;
//        private Image _liquidImage;
//        private RectTransform _liquidRect;
//        private TextMeshProUGUI _volumeLabelText;
//        private TextMeshProUGUI _emptyPromptText;
//        private Transform _dosedBadgeContainer;

//        private bool _isDragging;
//        private GameObject _dragGhost;
//        private RectTransform _dragGhostRect;

//        #endregion
//    }
//}

//using System;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
//    public class UIDraggableIngredientItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
//    {
//        #region Publics

//        public IngredientDefinition m_ingredient;
//        public int m_stockCount;
//        public RectTransform m_rectTransform;

//        public event Action<UIDraggableIngredientItem> OnIngredientSelected;
//        public event Action<UIDraggableIngredientItem, PointerEventData> OnIngredientDroppedOnBeaker;

//        #endregion


//        #region Unity API

//        public void OnPointerClick(PointerEventData eventData)
//        {
//            if (!_isDragging && m_stockCount > 0)
//            {
//                OnIngredientSelected?.Invoke(this);
//            }
//        }

//        public void OnPointerEnter(PointerEventData eventData)
//        {
//            var t = UIStyleHelper.Theme;
//            if (_bgImage != null && m_stockCount > 0)
//            {
//                _bgImage.color = t.m_cardBg;
//            }
//        }

//        public void OnPointerExit(PointerEventData eventData)
//        {
//            var t = UIStyleHelper.Theme;
//            if (_bgImage != null)
//            {
//                _bgImage.color = m_stockCount > 0 ? t.m_wellBg : new Color(t.m_wellBg.r, t.m_wellBg.g, t.m_wellBg.b, 0.45f);
//            }
//        }

//        public void OnBeginDrag(PointerEventData eventData)
//        {
//            if (eventData.button != PointerEventData.InputButton.Left || m_stockCount <= 0) return;

//            _isDragging = true;
//            _canvasGroup.alpha = 0.6f;
//            _canvasGroup.blocksRaycasts = false;

//            CreateDragGhost();
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
//            _canvasGroup.alpha = m_stockCount > 0 ? 1f : 0.5f;
//            _canvasGroup.blocksRaycasts = true;

//            if (_dragGhost != null)
//            {
//                Destroy(_dragGhost);
//                _dragGhost = null;
//            }

//            // Check if dropped on Beaker
//            OnIngredientDroppedOnBeaker?.Invoke(this, eventData);
//        }

//        #endregion


//        #region Main API

//        public void Initialize(IngredientDefinition ingredient, int stockCount, Canvas rootCanvas)
//        {
//            m_ingredient = ingredient;
//            m_stockCount = stockCount;
//            _rootCanvas = rootCanvas;

//            m_rectTransform = GetComponent<RectTransform>();
//            _canvasGroup = GetComponent<CanvasGroup>();

//            BuildVisuals();
//            SetStockCount(stockCount);
//        }

//        public void SetStockCount(int count)
//        {
//            var t = UIStyleHelper.Theme;
//            m_stockCount = count;
//            if (_stockBadgeText != null)
//            {
//                _stockBadgeText.text = $"x{count}";
//                _stockBadgeImage.color = count > 0 ? t.m_btnPrimary : t.m_btnDanger;
//            }

//            if (_canvasGroup != null)
//            {
//                _canvasGroup.alpha = count > 0 ? 1f : 0.45f;
//            }

//            if (_bgImage != null)
//            {
//                _bgImage.color = count > 0 ? t.m_wellBg : new Color(t.m_wellBg.r, t.m_wellBg.g, t.m_wellBg.b, 0.45f);
//            }
//        }

//        #endregion


//        #region Tools and Utilities

//        private void BuildVisuals()
//        {
//            var t = UIStyleHelper.Theme;

//            _bgImage = GetComponent<Image>();
//            if (_bgImage == null) _bgImage = gameObject.AddComponent<Image>();
//            _bgImage.sprite = UIStyleHelper.RoundedSprite;
//            _bgImage.type = Image.Type.Sliced;
//            _bgImage.color = t.m_wellBg;

//            // Left icon container (Jar / Flask / Flora)
//            GameObject iconPill = UIStyleHelper.CreatePanel(transform, "IconPill", m_ingredient.m_iconColor, UIStyleHelper.CircleSprite);
//            RectTransform ipRt = iconPill.GetComponent<RectTransform>();
//            ipRt.anchorMin = new Vector2(0.04f, 0.18f);
//            ipRt.anchorMax = new Vector2(0.24f, 0.82f);
//            ipRt.offsetMin = Vector2.zero;
//            ipRt.offsetMax = Vector2.zero;

//            string shortTag = GetShortCategoryTag(m_ingredient.m_category);
//            TextMeshProUGUI tagTxt = UIStyleHelper.CreateText(iconPill.transform, "Tag", shortTag, 7, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform tgRt = tagTxt.rectTransform;
//            tgRt.anchorMin = Vector2.zero;
//            tgRt.anchorMax = Vector2.one;
//            tgRt.offsetMin = Vector2.zero;
//            tgRt.offsetMax = Vector2.zero;

//            // Stock Count Badge (Top Right)
//            GameObject stockBadge = UIStyleHelper.CreatePanel(transform, "StockBadge", t.m_btnPrimary);
//            _stockBadgeImage = stockBadge.GetComponent<Image>();
//            RectTransform sbRt = stockBadge.GetComponent<RectTransform>();
//            sbRt.anchorMin = new Vector2(0.80f, 0.50f);
//            sbRt.anchorMax = new Vector2(0.96f, 0.90f);
//            sbRt.offsetMin = Vector2.zero;
//            sbRt.offsetMax = Vector2.zero;

//            _stockBadgeText = UIStyleHelper.CreateText(stockBadge.transform, "Count", "x1", 8, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform sbtRt = _stockBadgeText.rectTransform;
//            sbtRt.anchorMin = Vector2.zero;
//            sbtRt.anchorMax = Vector2.one;
//            sbtRt.offsetMin = Vector2.zero;
//            sbtRt.offsetMax = Vector2.zero;

//            // Title
//            TextMeshProUGUI titleTxt = UIStyleHelper.CreateText(transform, "Title", m_ingredient.m_displayName, 9, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform tRt = titleTxt.rectTransform;
//            tRt.anchorMin = new Vector2(0.27f, 0.50f);
//            tRt.anchorMax = new Vector2(0.78f, 0.92f);
//            tRt.offsetMin = Vector2.zero;
//            tRt.offsetMax = Vector2.zero;

//            // Category & hint text
//            string catLabel = m_ingredient.m_category == IngredientCategory.MedicinalFlora ? "<color=#10B981>[CYBER-PLANT]</color>" : m_ingredient.m_category.ToString();
//            TextMeshProUGUI subTxt = UIStyleHelper.CreateText(transform, "Sub", $"{catLabel} • Drag to Dose", 8, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//            RectTransform sRt = subTxt.rectTransform;
//            sRt.anchorMin = new Vector2(0.27f, 0.10f);
//            sRt.anchorMax = new Vector2(0.96f, 0.50f);
//            sRt.offsetMin = Vector2.zero;
//            sRt.offsetMax = Vector2.zero;
//        }

//        private void CreateDragGhost()
//        {
//            if (_rootCanvas == null) return;

//            _dragGhost = new GameObject("DragGhost_Ingredient", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            _dragGhost.transform.SetParent(_rootCanvas.transform, false);
//            _dragGhost.transform.SetAsLastSibling();

//            _dragGhostRect = _dragGhost.GetComponent<RectTransform>();
//            _dragGhostRect.sizeDelta = new Vector2(140, 50);

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
//            ghostImg.color = new Color(m_ingredient.m_iconColor.r, m_ingredient.m_iconColor.g, m_ingredient.m_iconColor.b, 0.85f);

//            CanvasGroup cg = _dragGhost.GetComponent<CanvasGroup>();
//            cg.blocksRaycasts = false;
//            cg.alpha = 0.85f;

//            UIStyleHelper.CreateText(_dragGhost.transform, "GhostText", $"Dosing: {m_ingredient.m_displayName}", 9, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//        }

//        private string GetShortCategoryTag(IngredientCategory cat)
//        {
//            switch (cat)
//            {
//                case IngredientCategory.CoffeeBase: return "ROAST";
//                case IngredientCategory.DairyPlant: return "MILK";
//                case IngredientCategory.Sweetener: return "SUGAR";
//                case IngredientCategory.MedicinalFlora: return "FLORA";
//                case IngredientCategory.Solvent: return "H2O";
//                default: return "ITEM";
//            }
//        }

//        #endregion


//        #region Private and Protected

//        private CanvasGroup _canvasGroup;
//        private Canvas _rootCanvas;
//        private Image _bgImage;
//        private bool _isDragging;

//        private Image _stockBadgeImage;
//        private TextMeshProUGUI _stockBadgeText;

//        private GameObject _dragGhost;
//        private RectTransform _dragGhostRect;

//        #endregion
//    }
//}

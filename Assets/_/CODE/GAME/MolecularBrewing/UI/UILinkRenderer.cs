//using System;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//namespace MolecularBrewing.Runtime
//{
//    public class UILinkRenderer : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
//    {
//        #region Publics

//        public UIMoleculeNode m_nodeA;
//        public UIMoleculeNode m_nodeB;
//        public RectTransform m_rectTransform;

//        public event Action<UILinkRenderer> OnLinkSeverRequested;

//        #endregion


//        #region Unity API

//        public void OnPointerClick(PointerEventData eventData)
//        {
//            if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
//            {
//                OnLinkSeverRequested?.Invoke(this);
//            }
//        }

//        public void OnPointerEnter(PointerEventData eventData)
//        {
//            if (_lineImage != null)
//            {
//                _lineImage.color = new Color(1f, 0.3f, 0.3f, 1f); // Red highlight indicating sever on click
//            }
//        }

//        public void OnPointerExit(PointerEventData eventData)
//        {
//            UpdateColors();
//        }

//        #endregion


//        #region Main API

//        public void Initialize(UIMoleculeNode nodeA, UIMoleculeNode nodeB)
//        {
//            m_nodeA = nodeA;
//            m_nodeB = nodeB;

//            m_rectTransform = GetComponent<RectTransform>();
//            if (m_rectTransform == null) m_rectTransform = gameObject.AddComponent<RectTransform>();

//            BuildVisuals();
//            UpdateLineGeometry();
//        }

//        public void UpdateLineGeometry()
//        {
//            if (m_nodeA == null || m_nodeB == null) return;

//            Vector2 posA = m_nodeA.m_rectTransform.anchoredPosition;
//            Vector2 posB = m_nodeB.m_rectTransform.anchoredPosition;

//            Vector2 dir = posB - posA;
//            float dist = dir.magnitude;
//            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

//            m_rectTransform.anchoredPosition = posA;
//            m_rectTransform.sizeDelta = new Vector2(dist, 8f);
//            m_rectTransform.pivot = new Vector2(0f, 0.5f);
//            m_rectTransform.localRotation = Quaternion.Euler(0, 0, angle);

//            UpdateColors();
//        }

//        public void SetFreeLineGeometry(Vector2 startPos, Vector2 endPos, Color lineColor)
//        {
//            if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();

//            Vector2 dir = endPos - startPos;
//            float dist = dir.magnitude;
//            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

//            m_rectTransform.anchoredPosition = startPos;
//            m_rectTransform.sizeDelta = new Vector2(dist, 6f);
//            m_rectTransform.pivot = new Vector2(0f, 0.5f);
//            m_rectTransform.localRotation = Quaternion.Euler(0, 0, angle);

//            if (_lineImage != null)
//            {
//                _lineImage.color = lineColor;
//            }
//        }

//        #endregion


//        #region Tools and Utilities

//        private void BuildVisuals()
//        {
//            _lineImage = GetComponent<Image>();
//            if (_lineImage == null) _lineImage = gameObject.AddComponent<Image>();
//            _lineImage.sprite = UIStyleHelper.RoundedSprite;
//            _lineImage.type = Image.Type.Sliced;
//        }

//        private void UpdateColors()
//        {
//            if (_lineImage != null && m_nodeA != null && m_nodeB != null)
//            {
//                Color cA = m_nodeA.m_definition != null ? m_nodeA.m_definition.m_accentColor : Color.cyan;
//                Color cB = m_nodeB.m_definition != null ? m_nodeB.m_definition.m_accentColor : Color.cyan;
//                _lineImage.color = Color.Lerp(cA, cB, 0.5f);
//            }
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private Image _lineImage;

//        #endregion
//    }
//}

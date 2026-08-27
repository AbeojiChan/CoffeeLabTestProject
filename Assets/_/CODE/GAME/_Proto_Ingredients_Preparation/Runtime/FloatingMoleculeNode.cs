using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class FloatingMoleculeNode : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Publics

        [Header("Identity")]
        public string m_moleculeName = "Caffeine";
        public Color m_moleculeColor = new Color(0.95f, 0.40f, 0.20f, 1f);
        public float m_radius = 28f;

        [Header("Movement & Physics")]
        public Vector2 m_velocity;
        public float m_moveSpeed = 45f;
        public float m_confinementRadius = 300f;
        public Vector2 m_chamberCenter = Vector2.zero;

        [Header("UI References")]
        public Image m_nodeImage;
        public Text m_label;
        public Image m_selectionRing;

        // Left Click / Move events
        public event Action<FloatingMoleculeNode> OnMoleculeClicked;
        public event Action<FloatingMoleculeNode> OnMoleculeDragStart;
        public event Action<FloatingMoleculeNode, Vector2> OnMoleculeDragged;
        public event Action<FloatingMoleculeNode> OnMoleculeDragEnd;

        // Right Click Bond Draw events
        public event Action<FloatingMoleculeNode, Vector2> OnBondDrawStart;
        public event Action<FloatingMoleculeNode, Vector2> OnBondDrawUpdate;
        public event Action<FloatingMoleculeNode, FloatingMoleculeNode> OnBondDrawEnd;

        public RectTransform RectTransform => _rectTransform;
        public bool IsSelected => _isSelected;

        #endregion


        #region Unity API

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (m_nodeImage == null) m_nodeImage = GetComponent<Image>();

            // Random initial direction
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            m_velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * m_moveSpeed;
        }

        private void Update()
        {
            if (_isMovingPosition) return;

            // Move node
            Vector2 currentPos = _rectTransform.anchoredPosition;
            currentPos += m_velocity * Time.deltaTime;

            // Circular Confinement check
            Vector2 offset = currentPos - m_chamberCenter;
            float dist = offset.magnitude;
            float maxDist = m_confinementRadius - m_radius;

            if (dist > maxDist)
            {
                // Reflect velocity off the circular boundary
                Vector2 normal = offset.normalized;
                m_velocity = Vector2.Reflect(m_velocity, normal);

                // Add slight Brownian disturbance
                float randomOffset = UnityEngine.Random.Range(-0.25f, 0.25f);
                m_velocity = Quaternion.Euler(0, 0, randomOffset * Mathf.Rad2Deg) * m_velocity;
                m_velocity = m_velocity.normalized * m_moveSpeed;

                // Push back inside
                currentPos = m_chamberCenter + normal * maxDist;
            }

            _rectTransform.anchoredPosition = currentPos;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnMoleculeClicked?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            if (m_selectionRing != null) m_selectionRing.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            if (!_isSelected && m_selectionRing != null) m_selectionRing.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _isDrawingBond = true;
                Vector2 localPos = GetLocalPointerPos(eventData);
                OnBondDrawStart?.Invoke(this, localPos);
            }
            else
            {
                _isMovingPosition = true;
                OnMoleculeDragStart?.Invoke(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPos = GetLocalPointerPos(eventData);

            if (_isDrawingBond)
            {
                OnBondDrawUpdate?.Invoke(this, localPos);
            }
            else if (_isMovingPosition)
            {
                // Clamp position inside circular confinement
                Vector2 offset = localPos - m_chamberCenter;
                float maxDist = m_confinementRadius - m_radius;
                if (offset.magnitude > maxDist)
                {
                    localPos = m_chamberCenter + offset.normalized * maxDist;
                }
                _rectTransform.anchoredPosition = localPos;
                OnMoleculeDragged?.Invoke(this, localPos);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDrawingBond)
            {
                _isDrawingBond = false;

                // Find if released over another node
                FloatingMoleculeNode targetNode = null;
                if (eventData.pointerEnter != null)
                {
                    targetNode = eventData.pointerEnter.GetComponentInParent<FloatingMoleculeNode>();
                }

                OnBondDrawEnd?.Invoke(this, targetNode);
            }
            else if (_isMovingPosition)
            {
                _isMovingPosition = false;
                OnMoleculeDragEnd?.Invoke(this);
            }
        }

        #endregion


        #region Main API

        public void Setup(string moleculeName, Color color, float confinementRadius, Vector2 chamberCenter)
        {
            m_moleculeName = moleculeName;
            m_moleculeColor = color;
            m_confinementRadius = confinementRadius;
            m_chamberCenter = chamberCenter;

            if (m_nodeImage != null)
            {
                m_nodeImage.color = color;
            }

            if (m_label != null)
            {
                m_label.text = moleculeName.Length > 4 ? moleculeName.Substring(0, 4) : moleculeName;
            }
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (m_selectionRing != null)
            {
                m_selectionRing.gameObject.SetActive(selected || _isHovered);
            }
        }

        #endregion


        #region Tools and Utilities

        private Vector2 GetLocalPointerPos(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            {
                return localPoint;
            }
            return _rectTransform.anchoredPosition;
        }

        #endregion


        #region Private and Protected

        private RectTransform _rectTransform;
        private bool _isSelected = false;
        private bool _isHovered = false;
        private bool _isMovingPosition = false;
        private bool _isDrawingBond = false;

        #endregion
    }
}

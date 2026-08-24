using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIMoleculeNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Publics

        public MoleculeType m_moleculeType;
        public MoleculeDefinition m_definition;
        public RectTransform m_rectTransform;

        [Header("Thermal Drift & Brownian Motion")]
        public Vector2 m_driftVelocity;
        public float m_driftSpeed = 26f;
        public bool m_isDriftingEnabled = true;
        public bool m_isFrozen = false;

        public event Action<UIMoleculeNode> OnNodeMoved;
        public event Action<UIMoleculeNode, Vector2> OnRightClickDragStart;
        public event Action<UIMoleculeNode, Vector2> OnRightClickDragUpdate;
        public event Action<UIMoleculeNode, UIMoleculeNode> OnRightClickDragEnd;

        #endregion


        #region Unity API

        private void Update()
        {
            if (m_isDriftingEnabled && !_isLeftDragging)
            {
                UpdateDriftMotion();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _isRightClickDragging = true;
                OnRightClickDragStart?.Invoke(this, eventData.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && _isRightClickDragging)
            {
                _isRightClickDragging = false;
                UIMoleculeNode targetNode = null;
                if (eventData.pointerEnter != null)
                {
                    targetNode = eventData.pointerEnter.GetComponentInParent<UIMoleculeNode>();
                }
                OnRightClickDragEnd?.Invoke(this, targetNode);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _isLeftDragging = true;
                _canvasGroup.alpha = 0.9f;
                transform.SetAsLastSibling();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && _isLeftDragging)
            {
                if (_rootCanvas != null)
                {
                    m_rectTransform.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;
                }
                else
                {
                    m_rectTransform.position = eventData.position;
                }

                ClampToParentBounds();
                OnNodeMoved?.Invoke(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right && _isRightClickDragging)
            {
                OnRightClickDragUpdate?.Invoke(this, eventData.position);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && _isLeftDragging)
            {
                _isLeftDragging = false;
                _canvasGroup.alpha = 1f;

                // Impart a gentle throw impulse from drag delta
                if (eventData.delta.sqrMagnitude > 1f)
                {
                    m_driftVelocity = eventData.delta.normalized * m_driftSpeed;
                }

                ClampToParentBounds();
                OnNodeMoved?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_ringImage != null)
            {
                _ringImage.color = Color.white;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_ringImage != null && m_definition != null)
            {
                _ringImage.color = _isBonded ? m_definition.m_accentColor : new Color(m_definition.m_accentColor.r, m_definition.m_accentColor.g, m_definition.m_accentColor.b, 0.4f);
            }
        }

        #endregion


        #region Main API

        public void Initialize(MoleculeDefinition def, Canvas rootCanvas, Vector2 initialPos)
        {
            m_definition = def;
            m_moleculeType = def.m_type;
            _rootCanvas = rootCanvas;

            m_rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            m_rectTransform.sizeDelta = new Vector2(74, 74);
            m_rectTransform.anchoredPosition = initialPos;

            // Random initial drift direction
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            m_driftVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * m_driftSpeed;
            _noiseSeed = UnityEngine.Random.Range(0f, 1000f);

            BuildVisuals();
            SetBondedState(false);
        }

        public void SetBondedState(bool isBonded)
        {
            _isBonded = isBonded;
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isBonded ? 1f : 0.65f;
            }
            if (_ringImage != null && m_definition != null)
            {
                _ringImage.color = isBonded ? m_definition.m_accentColor : new Color(m_definition.m_accentColor.r, m_definition.m_accentColor.g, m_definition.m_accentColor.b, 0.4f);
            }
        }

        public void SetDriftState(bool enabled)
        {
            m_isDriftingEnabled = enabled;
        }

        public void SetFrozenState(bool isFrozen)
        {
            m_isFrozen = isFrozen;
        }

        #endregion


        #region Tools and Utilities

        private void UpdateDriftMotion()
        {
            // Organic subtle direction wandering (Perlin noise rotational perturbation)
            float noise = (Mathf.PerlinNoise(_noiseSeed, Time.time * 0.4f) - 0.5f) * 2f;
            m_driftVelocity = Quaternion.Euler(0, 0, noise * 30f * Time.deltaTime) * m_driftVelocity;

            // Bonded molecules have more mass / slightly slower, stabilized drift
            float bondMult = _isBonded ? 0.65f : 1.0f;
            float frozenMult = m_isFrozen ? 0.035f : 1.0f; // Ultra-slow cryogenic movement when frozen
            float speedMult = bondMult * frozenMult;
            m_rectTransform.anchoredPosition += m_driftVelocity * (speedMult * Time.deltaTime);

            // Circular Microscope Ocular Lens Boundary Bounce & Reflection
            RectTransform parentRt = transform.parent as RectTransform;
            if (parentRt != null)
            {
                float lensRadius = Mathf.Min(parentRt.rect.width, parentRt.rect.height) * 0.5f;
                float maxWorkableRadius = Mathf.Max(60f, lensRadius - 44f);

                Vector2 pos = m_rectTransform.anchoredPosition;
                float dist = pos.magnitude;

                if (dist >= maxWorkableRadius)
                {
                    Vector2 normal = pos / (dist > 0.001f ? dist : 1f);
                    pos = normal * (maxWorkableRadius * 0.99f);
                    m_rectTransform.anchoredPosition = pos;

                    // Reflect drift velocity inward against circle normal
                    float dot = Vector2.Dot(m_driftVelocity, normal);
                    if (dot > 0f)
                    {
                        m_driftVelocity = m_driftVelocity - 2f * dot * normal;
                    }
                }
            }
        }

        private void ClampToParentBounds()
        {
            RectTransform parentRt = transform.parent as RectTransform;
            if (parentRt != null)
            {
                float lensRadius = Mathf.Min(parentRt.rect.width, parentRt.rect.height) * 0.5f;
                float maxWorkableRadius = Mathf.Max(60f, lensRadius - 44f);

                Vector2 pos = m_rectTransform.anchoredPosition;
                float dist = pos.magnitude;

                if (dist > maxWorkableRadius)
                {
                    Vector2 normal = pos / (dist > 0.001f ? dist : 1f);
                    pos = normal * (maxWorkableRadius * 0.99f);
                    m_rectTransform.anchoredPosition = pos;
                }
            }
        }

        private void BuildVisuals()
        {
            if (_bgImage != null && _formulaText != null)
            {
                // Serialized / Pre-configured hierarchy mode
                _bgImage.color = m_definition.m_primaryColor;
                if (_ringImage != null) _ringImage.color = m_definition.m_accentColor;
                _formulaText.text = m_definition.m_formula;
                if (_nameText != null)
                {
                    _nameText.text = m_definition.m_isCyberPlant ? "[PLANT]" : m_definition.m_displayName;
                    _nameText.color = m_definition.m_isCyberPlant ? new Color(0.4f, 1f, 0.8f, 0.95f) : new Color(1f, 1f, 1f, 0.85f);
                }
                return;
            }

            _bgImage = GetComponent<Image>();
            if (_bgImage == null) _bgImage = gameObject.AddComponent<Image>();
            _bgImage.sprite = UIStyleHelper.CircleSprite;
            _bgImage.color = m_definition.m_primaryColor;

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

            _formulaText = UIStyleHelper.CreateText(
                transform,
                "Formula",
                m_definition.m_formula,
                13,
                Color.white,
                TextAlignmentOptions.Center,
                FontStyles.Bold
            );
            RectTransform fRt = _formulaText.rectTransform;
            fRt.anchorMin = new Vector2(0, 0.35f);
            fRt.anchorMax = new Vector2(1, 0.85f);
            fRt.offsetMin = Vector2.zero;
            fRt.offsetMax = Vector2.zero;

            string label = m_definition.m_isCyberPlant ? "[PLANT]" : m_definition.m_displayName;
            _nameText = UIStyleHelper.CreateText(
                transform,
                "Name",
                label,
                9,
                m_definition.m_isCyberPlant ? new Color(0.4f, 1f, 0.8f, 0.95f) : new Color(1f, 1f, 1f, 0.85f),
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

        [Header("GameObject Serialized Bindings")]
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _ringImage;
        [SerializeField] private TextMeshProUGUI _formulaText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Canvas _rootCanvas;
        private bool _isLeftDragging;
        private bool _isRightClickDragging;
        private bool _isBonded;
        private float _noiseSeed;

        #endregion
    }
}

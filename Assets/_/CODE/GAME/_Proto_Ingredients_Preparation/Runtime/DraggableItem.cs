using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Publics

    public bool m_isInsideZone = false;

    public bool isInsideGrinder
    {
        get => m_isInsideZone;
        set => m_isInsideZone = value;
    }

    #endregion


    #region Unity API

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    #endregion


    #region Main API

    public void SetInsideZone(bool isInside, Transform newParent = null)
    {
        m_isInsideZone = isInside;
        if (isInside && newParent != null)
        {
            transform.SetParent(newParent);
            _rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        m_isInsideZone = false;

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / GetCanvasScale();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if (!m_isInsideZone)
        {
            _rectTransform.anchoredPosition = _startPosition;
            transform.SetParent(_originalParent);
        }
    }

    #endregion


    #region Tools and Utilities

    private float GetCanvasScale()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.scaleFactor : 1f;
    }

    #endregion


    #region Private and Protected

    [SerializeField] private IngredientsData _data;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Vector2 _startPosition;

    #endregion
}
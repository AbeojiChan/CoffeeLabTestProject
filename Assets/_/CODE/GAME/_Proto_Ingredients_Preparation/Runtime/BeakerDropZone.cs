using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeakerDropZone : MonoBehaviour, IDropHandler
{
    #region Publics

    public int m_maxDoses = 7;
    public event Action<DraggableItem> OnItemDosed;
    public event Action OnBeakerCleared;

    public IReadOnlyList<DraggableItem> DosedItems => _dosedItems;
    public int CurrentDoseCount => _dosedItems.Count;
    public bool IsFull => _dosedItems.Count >= m_maxDoses;

    #endregion


    #region Unity API

    private void Awake()
    {
        if (_itemsContainer == null)
        {
            _itemsContainer = transform as RectTransform;
        }
    }

    #endregion


    #region Main API

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (eventData.pointerDrag.TryGetComponent<DraggableItem>(out var item))
        {
            if (IsFull)
            {
                Debug.LogWarning($"[BeakerDropZone] Beaker is at maximum capacity ({m_maxDoses} doses).");
                return;
            }

            if (!_dosedItems.Contains(item))
            {
                _dosedItems.Add(item);
                item.SetInsideZone(true, _itemsContainer != null ? _itemsContainer : transform);
                OnItemDosed?.Invoke(item);
                Debug.Log($"[BeakerDropZone] Dosed item: {item.name} ({_dosedItems.Count}/{m_maxDoses})");
            }
        }
    }

    public void RemoveItem(DraggableItem item)
    {
        if (item != null && _dosedItems.Contains(item))
        {
            _dosedItems.Remove(item);
        }
    }

    public void ClearBeaker()
    {
        foreach (var item in _dosedItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        _dosedItems.Clear();
        OnBeakerCleared?.Invoke();
    }

    #endregion


    #region Tools and Utilities
    #endregion


    #region Private and Protected

    [SerializeField] private RectTransform _itemsContainer;
    private readonly List<DraggableItem> _dosedItems = new List<DraggableItem>();

    #endregion
}

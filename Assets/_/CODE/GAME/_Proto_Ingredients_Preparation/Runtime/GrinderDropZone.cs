using UnityEngine;
using UnityEngine.EventSystems;

public class GrinderDropZone : MonoBehaviour, IDropHandler
{
    #region Publics

    public DraggableItem CurrentIngredient { get; private set; }
    public IngredientItem CurrentIngredientDataHolder { get; private set; }

    #endregion

    #region Unity API
    #endregion

    #region Main API

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || CurrentIngredient != null) return;

        if (eventData.pointerDrag.TryGetComponent<DraggableItem>(out var item) &&
            eventData.pointerDrag.TryGetComponent<IngredientItem>(out var ingredientHolder))
        {
            CurrentIngredient = item;
            CurrentIngredientDataHolder = ingredientHolder;

            item.isInsideGrinder = true;

            item.transform.SetParent(_snapSlot != null ? _snapSlot : transform);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void ConsumeIngredient()
    {
        if (CurrentIngredient != null)
        {
            Destroy(CurrentIngredient.gameObject);
            CurrentIngredient = null;
            CurrentIngredientDataHolder = null;
        }
    }

    #endregion

    #region Tools and Utilities
    #endregion

    #region Private and Protected

    [SerializeField] private RectTransform _snapSlot;

    #endregion
}
using UnityEngine;
using UnityEngine.EventSystems;

public class GrinderDropZone : MonoBehaviour, IDropHandler
    {
    #region Publics

    public DraggableItem CurrentIngredient { get; private set; }

    #endregion

    #region Unity API
    #endregion

    #region Main API

    public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null || CurrentIngredient != null) return;

		DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();
		if (item != null)
		{
			CurrentIngredient = item;
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
		}
	}

	#endregion

	#region Tools and Utilities
	#endregion

	#region Private and Protected

	[SerializeField] private RectTransform _snapSlot;	

	#endregion

    }

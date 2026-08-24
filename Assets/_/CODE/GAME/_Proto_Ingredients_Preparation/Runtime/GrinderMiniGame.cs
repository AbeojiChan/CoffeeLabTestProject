using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GrinderMinigame : MonoBehaviour, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuration")]
    [SerializeField] private float grindDuration = 2.5f;

    [Header("UI References")]
    [SerializeField] private Image progressBar;
    [SerializeField] private RectTransform snapSlot;

    private DraggableItem currentIngredient;
    private bool isHoldingButton = false;
    private float currentProgress = 0f;
    private bool isGrindingComplete = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || currentIngredient != null) return;

        DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (item != null)
        {
            currentIngredient = item;
            item.isInsideGrinder = true;

            item.transform.SetParent(snapSlot != null ? snapSlot : transform);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentIngredient != null && !isGrindingComplete)
        {
            isHoldingButton = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHoldingButton = false;
    }

    private void Update()
    {
        if (isHoldingButton && !isGrindingComplete)
        {
            currentProgress += Time.deltaTime;
            UpdateProgressDisplay();

            if (currentProgress >= grindDuration)
            {
                CompleteGrinding();
            }
        }
    }

    private void UpdateProgressDisplay()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = Mathf.Clamp01(currentProgress / grindDuration);
        }
    }

    private void CompleteGrinding()
    {
        isGrindingComplete = true;
        isHoldingButton = false;

        Destroy(currentIngredient.gameObject);
        currentIngredient = null;

        Debug.Log("Grinding Complete! Grounds ready.");
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldGrindButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Publics
    #endregion

    #region Unity API

    private void Update()
    {
        if (!_isHolding || _isComplete || _dropZone == null || _dropZone.CurrentIngredientDataHolder == null) return;

        IngredientsData data = _dropZone.CurrentIngredientDataHolder.Data;
        if (data == null) return;

        _currentProgress += Time.deltaTime;

        if (_progressBar != null)
        {
            _progressBar.fillAmount = Mathf.Clamp01(_currentProgress / data.GrindDuration);
        }

        if (_currentProgress >= data.GrindDuration)
        {
            _isHolding = false;
            _currentProgress = 0f;
            if (_progressBar != null)
            {
                _progressBar.fillAmount = 0f;
            }

            GameObject productToSpawn = data.ProcessedPrefab;

            _dropZone.ConsumeIngredient();

            if (productToSpawn != null && _finishedProductSpawnPoint != null)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                Transform parentToUse = canvas != null ? canvas.transform : transform.parent;
                GameObject spawned = Instantiate(productToSpawn, _finishedProductSpawnPoint.position, Quaternion.identity, parentToUse);
                spawned.transform.localScale = Vector3.one;
                spawned.transform.SetAsLastSibling();
            }

            Debug.Log($"produced : {data.name}");
        }
    }

    #endregion

    #region Main API

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_dropZone != null && _dropZone.CurrentIngredientDataHolder != null && !_isComplete)
        {
            _isHolding = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
    }

    #endregion

    #region Tools and Utilities

    public void ResetGrinder()
    {
        _isComplete = false;
        _isHolding = false;
        _currentProgress = 0f;
        if (_progressBar != null) _progressBar.fillAmount = 0f;
    }

    #endregion

    #region Private and Protected

    [Header("References")]
    [SerializeField] private GrinderDropZone _dropZone;
    [SerializeField] private Image _progressBar;
    [SerializeField] private Transform _finishedProductSpawnPoint;

    private bool _isHolding = false;
    private float _currentProgress = 0f;
    private bool _isComplete = false;

    #endregion
}
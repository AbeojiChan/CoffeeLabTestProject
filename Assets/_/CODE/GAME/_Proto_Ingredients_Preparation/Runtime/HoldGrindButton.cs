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
		if (!_isHolding || _isComplete) return;
		_currentProgress += Time.deltaTime;

		if (_progressBar != null)
		{
			_progressBar.fillAmount = Mathf.Clamp01(_currentProgress / _grindDuration);
		}

        if (_currentProgress >= _grindDuration)
        {
            _isComplete = true;
            _isHolding = false;
            _dropZone.ConsumeIngredient();
            Debug.Log("Beans ground successfully!");
			GameObject spawned = Instantiate(_finishedProduct, _finishedProductSpawnPoint.position, Quaternion.identity, transform.parent);
			spawned.transform.SetAsLastSibling();
		}
    }

	#endregion

	#region Main API
	public void OnPointerDown(PointerEventData eventData)
	{
		if (_dropZone != null && _dropZone.CurrentIngredient != null && !_isComplete)
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
	#endregion

	#region Private and Protected

	[Header("References")]
	[SerializeField] private GrinderDropZone _dropZone;
	[SerializeField] private Image _progressBar;

	[SerializeField] private GameObject _finishedProduct;

	[SerializeField] private Transform _finishedProductSpawnPoint;

	[Header("Settings")]
	[SerializeField] private float _grindDuration = 2.5f;

	private bool _isHolding = false;
	private float _currentProgress = 0f;

	private bool _isComplete = false;
	#endregion

    }

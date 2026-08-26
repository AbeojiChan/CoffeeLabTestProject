using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class PreparationTrayController : MonoBehaviour
    {
        #region Publics

        [Header("Capacity Configuration")]
        [Tooltip("Maximum number of ingredients that can be placed on the tray (adjustable in Inspector)")]
        public int m_maxCapacity = 7;

        [Header("UI References")]
        public Transform m_itemsContainer;
        public GameObject m_trayItemPrefab;
        public Button m_resetButton;
        public Button m_proceedButton;
        public Text m_capacityLabel;

        public event Action<RawIngredientItemData> OnItemAdded;
        public event Action<int> OnItemRemoved;
        public event Action OnTrayReset;
        public event Action<List<RawIngredientItemData>> OnProceedToMachine;

        public IReadOnlyList<RawIngredientItemData> CurrentItems => _items;
        public bool IsFull => _items.Count >= m_maxCapacity;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_resetButton != null)
            {
                m_resetButton.onClick.AddListener(ResetTray);
            }

            if (m_proceedButton != null)
            {
                m_proceedButton.onClick.AddListener(ProceedToPreparationMachine);
            }

            UpdateUI();
        }

        #endregion


        #region Main API

        public bool TryAddItem(RawIngredientItemData item)
        {
            if (item == null) return false;

            if (_items.Count >= m_maxCapacity)
            {
                Debug.LogWarning($"[PreparationTray] Tray is at max capacity ({m_maxCapacity}). Cannot add more items.");
                return false;
            }

            _items.Add(item);
            CreateTrayItemUI(item, _items.Count - 1);
            UpdateUI();

            OnItemAdded?.Invoke(item);
            return true;
        }

        public void RemoveItemAt(int index)
        {
            if (index < 0 || index >= _items.Count) return;

            _items.RemoveAt(index);
            RebuildTrayUI();
            UpdateUI();

            OnItemRemoved?.Invoke(index);
        }

        public void ResetTray()
        {
            _items.Clear();
            RebuildTrayUI();
            UpdateUI();

            OnTrayReset?.Invoke();
            Debug.Log("[PreparationTray] Tray was reset.");
        }

        public void ProceedToPreparationMachine()
        {
            if (_items.Count == 0)
            {
                Debug.LogWarning("[PreparationTray] Cannot proceed: Tray is empty!");
                return;
            }

            Debug.Log($"[PreparationTray] Proceeding with {_items.Count} raw ingredients to the 3-Chamber Machine!");
            OnProceedToMachine?.Invoke(new List<RawIngredientItemData>(_items));
        }

        #endregion


        #region Tools and Utilities

        private void CreateTrayItemUI(RawIngredientItemData item, int index)
        {
            if (m_itemsContainer == null) return;

            GameObject itemObj = null;
            if (m_trayItemPrefab != null)
            {
                itemObj = Instantiate(m_trayItemPrefab, m_itemsContainer);
            }
            else
            {
                itemObj = new GameObject($"TrayItem_{index}", typeof(RectTransform), typeof(Image), typeof(Button));
                itemObj.transform.SetParent(m_itemsContainer, false);
                var img = itemObj.GetComponent<Image>();
                img.color = item.m_themeColor;
                if (item.m_icon != null) img.sprite = item.m_icon;
            }

            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                int capturedIndex = index;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => RemoveItemAt(capturedIndex));
            }

            var image = itemObj.GetComponent<Image>();
            if (image != null && item.m_icon != null)
            {
                image.sprite = item.m_icon;
                image.color = item.m_themeColor;
            }

            var text = itemObj.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = item.m_displayName;
            }

            _spawnedUIItems.Add(itemObj);
        }

        private void RebuildTrayUI()
        {
            foreach (var obj in _spawnedUIItems)
            {
                if (obj != null) Destroy(obj);
            }
            _spawnedUIItems.Clear();

            for (int i = 0; i < _items.Count; i++)
            {
                CreateTrayItemUI(_items[i], i);
            }
        }

        private void UpdateUI()
        {
            if (m_capacityLabel != null)
            {
                m_capacityLabel.text = $"{_items.Count} / {m_maxCapacity}";
            }

            if (m_proceedButton != null)
            {
                m_proceedButton.interactable = _items.Count > 0;
            }
        }

        #endregion


        #region Private and Protected

        private readonly List<RawIngredientItemData> _items = new List<RawIngredientItemData>();
        private readonly List<GameObject> _spawnedUIItems = new List<GameObject>();

        #endregion
    }
}

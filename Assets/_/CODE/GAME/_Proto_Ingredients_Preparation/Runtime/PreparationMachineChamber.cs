using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class PreparationMachineChamber : MonoBehaviour
    {
        #region Publics

        [Header("Chamber Identity")]
        public MachineChamberType m_chamberType = MachineChamberType.Chamber1_Grinder;
        public string m_chamberTitle = "BROYEUR À GRAINS";
        public string m_acceptedCategoryHint = "Café, Poussières Lunaires & Solides";
        public Color m_themeAccent = new Color(0.85f, 0.55f, 0.20f, 1f);

        [Header("UI References")]
        public Transform m_loadedItemsContainer;
        public GameObject m_loadedItemPrefab;
        public Text m_chamberTitleText;
        public Text m_countText;
        public Image m_chamberBackground;
        public Image m_processingIndicator;

        public event Action<RawIngredientItemData> OnIngredientRemoved;

        public IReadOnlyList<RawIngredientItemData> LoadedIngredients => _loadedIngredients;

        #endregion


        #region Unity API

        private void Start()
        {
            if (m_chamberTitleText != null)
            {
                m_chamberTitleText.text = m_chamberTitle;
            }
            UpdateUI();
        }

        #endregion


        #region Main API

        public bool CanAccept(RawIngredientItemData ingredient)
        {
            if (ingredient == null) return false;
            return ingredient.m_targetChamber == m_chamberType;
        }

        public void AddIngredient(RawIngredientItemData ingredient)
        {
            if (ingredient == null) return;
            _loadedIngredients.Add(ingredient);
            UpdateUI();
        }

        public void RemoveIngredient(RawIngredientItemData ingredient)
        {
            if (_loadedIngredients.Remove(ingredient))
            {
                OnIngredientRemoved?.Invoke(ingredient);
                UpdateUI();
            }
        }

        public void ClearChamber()
        {
            _loadedIngredients.Clear();
            UpdateUI();
        }

        public void SetProcessingState(bool isProcessing, float progress = 0f)
        {
            if (m_processingIndicator != null)
            {
                m_processingIndicator.gameObject.SetActive(isProcessing || progress >= 1.0f);
                m_processingIndicator.color = isProcessing ? m_themeAccent : Color.white;
            }
        }

        #endregion


        #region Tools and Utilities

        private void UpdateUI()
        {
            if (m_countText != null)
            {
                m_countText.text = $"Chargé: {_loadedIngredients.Count}";
            }

            if (m_loadedItemsContainer == null) return;

            foreach (var spawned in _spawnedVisuals)
            {
                if (spawned != null) Destroy(spawned);
            }
            _spawnedVisuals.Clear();

            for (int i = 0; i < _loadedIngredients.Count; i++)
            {
                var item = _loadedIngredients[i];
                if (item == null) continue;

                GameObject obj = null;
                if (m_loadedItemPrefab != null)
                {
                    obj = Instantiate(m_loadedItemPrefab, m_loadedItemsContainer);
                }
                else
                {
                    obj = new GameObject($"Item_{item.m_displayName}", typeof(RectTransform), typeof(Image), typeof(Button));
                    obj.transform.SetParent(m_loadedItemsContainer, false);
                }

                var img = obj.GetComponent<Image>();
                if (img != null)
                {
                    img.color = item.m_themeColor;
                    if (item.m_sprite != null)
                    {
                        img.sprite = item.m_sprite;
                        img.color = Color.white;
                    }
                }

                var btn = obj.GetComponent<Button>();
                if (btn != null)
                {
                    var captured = item;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => RemoveIngredient(captured));
                }

                var txt = obj.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.text = item.m_displayName;
                }

                _spawnedVisuals.Add(obj);
            }
        }

        #endregion


        #region Private and Protected

        private readonly List<RawIngredientItemData> _loadedIngredients = new List<RawIngredientItemData>();
        private readonly List<GameObject> _spawnedVisuals = new List<GameObject>();

        #endregion
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class IngredientPantryPanel : MonoBehaviour
    {
        #region Publics

        [Header("Data List (Inputted in Inspector)")]
        public List<RawIngredientItemData> m_availableIngredients = new List<RawIngredientItemData>();

        [Header("References")]
        public PreparationTrayController m_trayController;
        public Transform m_cardsGridContainer;
        public GameObject m_ingredientCardPrefab;

        [Header("Tabs")]
        public Button m_tabAllButton;
        public Button m_tabBasesButton;
        public Button m_tabAdditivesButton;

        public event Action<RawIngredientItemData> OnIngredientSelected;

        #endregion


        #region Unity API

        private void Start()
        {
            if (m_tabAllButton != null)
            {
                m_tabAllButton.onClick.AddListener(ShowAll);
            }

            if (m_tabBasesButton != null)
            {
                m_tabBasesButton.onClick.AddListener(() => FilterType(RawIngredientType.Base));
            }

            if (m_tabAdditivesButton != null)
            {
                m_tabAdditivesButton.onClick.AddListener(() => FilterType(RawIngredientType.Additive));
            }

            PopulateCards(m_availableIngredients);
        }

        #endregion


        #region Main API

        public void SetIngredients(List<RawIngredientItemData> ingredients)
        {
            m_availableIngredients = ingredients ?? new List<RawIngredientItemData>();
            PopulateCards(m_availableIngredients);
        }

        public void FilterType(RawIngredientType type)
        {
            var filtered = m_availableIngredients.FindAll(item => item != null && item.m_ingredientType == type);
            PopulateCards(filtered.Count > 0 ? filtered : m_availableIngredients);
        }

        public void ShowAll()
        {
            PopulateCards(m_availableIngredients);
        }

        #endregion


        #region Tools and Utilities

        private void PopulateCards(List<RawIngredientItemData> items)
        {
            if (m_cardsGridContainer == null) return;

            foreach (var card in _spawnedCards)
            {
                if (card != null) Destroy(card);
            }
            _spawnedCards.Clear();

            if (items == null) return;

            foreach (var ingredient in items)
            {
                if (ingredient == null) continue;
                CreateIngredientCard(ingredient);
            }
        }

        private void CreateIngredientCard(RawIngredientItemData ingredient)
        {
            GameObject cardObj = null;
            if (m_ingredientCardPrefab != null)
            {
                cardObj = Instantiate(m_ingredientCardPrefab, m_cardsGridContainer);
            }
            else
            {
                cardObj = new GameObject($"Card_{ingredient.m_displayName}", typeof(RectTransform), typeof(Image), typeof(Button));
                cardObj.transform.SetParent(m_cardsGridContainer, false);
            }

            var image = cardObj.GetComponent<Image>();
            if (image != null)
            {
                image.color = ingredient.m_themeColor;
                if (ingredient.m_sprite != null)
                {
                    image.sprite = ingredient.m_sprite;
                    image.color = Color.white;
                }
            }

            var text = cardObj.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = ingredient.m_displayName;
            }

            var button = cardObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    OnIngredientSelected?.Invoke(ingredient);
                    if (m_trayController != null)
                    {
                        m_trayController.TryAddItem(ingredient);
                    }
                });
            }

            _spawnedCards.Add(cardObj);
        }

        #endregion


        #region Private and Protected

        private readonly List<GameObject> _spawnedCards = new List<GameObject>();

        #endregion
    }
}

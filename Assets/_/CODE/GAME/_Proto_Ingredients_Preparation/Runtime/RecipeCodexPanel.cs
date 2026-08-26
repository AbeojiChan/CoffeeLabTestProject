using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class RecipeCodexPanel : MonoBehaviour
    {
        #region Publics

        [Header("Data Lists (Inputted in Inspector)")]
        public List<RecipeData> m_recipes = new List<RecipeData>();
        public List<AdditiveEffectData> m_additiveEffects = new List<AdditiveEffectData>();

        [Header("UI Containers - 3 Columns (Upper Section)")]
        public Transform m_columnNeoContainer;
        public Transform m_columnMixContainer;
        public Transform m_columnHeartContainer;

        [Header("UI Container - Additives Effects (Lower Section)")]
        public Transform m_additivesGridContainer;

        [Header("Prefabs & Info")]
        public GameObject m_recipeCardPrefab;
        public GameObject m_additiveEffectCardPrefab;
        public Text m_selectedRecipeInfoText;

        public event Action<RecipeData> OnRecipeClicked;
        public event Action<AdditiveEffectData> OnAdditiveEffectClicked;

        #endregion


        #region Unity API

        private void Start()
        {
            BuildCodexLayout();
        }

        #endregion


        #region Main API

        public void SetData(List<RecipeData> recipes, List<AdditiveEffectData> additiveEffects)
        {
            m_recipes = recipes ?? new List<RecipeData>();
            m_additiveEffects = additiveEffects ?? new List<AdditiveEffectData>();
            BuildCodexLayout();
        }

        public void SelectRecipe(RecipeData recipe)
        {
            if (recipe == null) return;

            if (m_selectedRecipeInfoText != null)
            {
                if (recipe.m_isUnlocked)
                {
                    List<string> ingNames = new List<string>();
                    foreach (var ing in recipe.m_requiredRawIngredients)
                    {
                        if (ing != null) ingNames.Add(ing.m_displayName);
                    }
                    string ingredientsList = ingNames.Count > 0 ? string.Join(" + ", ingNames) : "Aucun";

                    List<string> molDetails = new List<string>();
                    foreach (var mol in recipe.m_requiredMolecules)
                    {
                        molDetails.Add($"{mol.m_requiredCount}x {mol.m_moleculeName}");
                    }
                    string moleculesList = molDetails.Count > 0 ? string.Join(", ", molDetails) : "N/A";

                    string effectLine = recipe.m_hasSpecialEffect
                        ? $"\n<b>Effet Inhérent :</b> <color=#FF8844>{recipe.m_inherentEffectName}</color> ({recipe.m_inherentEffectDescription})"
                        : "\n<b>Effet Inhérent :</b> <color=#88FF88>Boisson Neutre (Sans effet magique de base)</color>";

                    m_selectedRecipeInfoText.text = $"<b>{recipe.m_recipeName}</b> <color=#FFD759>[Origine: {recipe.CategoryTag}]</color>\n{recipe.m_description}{effectLine}\n\n<b>Ingrédients de Base:</b> {ingredientsList}\n<b>Molécules en Synthèse:</b> {moleculesList}";
                }
                else
                {
                    m_selectedRecipeInfoText.text = $"<b>[RECETTE NON DÉCOUVERTE]</b> <color=#888888>[Origine: {recipe.CategoryTag}]</color>\n\n<i>Cette formulation reste verrouillée. Préparez cette combinaison de base au moins une fois pour débloquer le blueprint dans la cyber-tablette.</i>";
                }
            }

            OnRecipeClicked?.Invoke(recipe);
        }

        public void SelectAdditiveEffect(AdditiveEffectData effect)
        {
            if (effect == null) return;

            if (m_selectedRecipeInfoText != null)
            {
                if (effect.m_isUnlocked)
                {
                    List<string> addNames = new List<string>();
                    foreach (var a in effect.m_requiredAdditives)
                    {
                        if (a != null) addNames.Add(a.m_displayName);
                    }
                    string addList = addNames.Count > 0 ? string.Join(" + ", addNames) : "Aucun";

                    m_selectedRecipeInfoText.text = $"<b>EFFET ADDITIF : <color=#44FFCC>{effect.m_effectName}</color></b>\n{effect.m_effectDescription}\n\n<b>Additif(s) Requis:</b> {addList}\n<i>Cet effet s'ajoute et se lie à la recette de base lorsqu'il est broyé/préparé avec elle.</i>";
                }
                else
                {
                    m_selectedRecipeInfoText.text = $"<b>[EFFET ADDITIF VERROUILLÉ]</b>\n\n<i>Combinaison d'additifs inconnue. Ajoutez et préparez cet additif sur le plateau pour découvrir son effet et l'enregistrer dans le codex.</i>";
                }
            }

            OnAdditiveEffectClicked?.Invoke(effect);
        }

        public void CheckAndUnlockRecipeAndEffects(List<RawIngredientItemData> preparedIngredients)
        {
            if (preparedIngredients == null || preparedIngredients.Count == 0) return;

            List<RawIngredientItemData> baseIngredients = preparedIngredients.FindAll(i => i != null && i.m_ingredientType == RawIngredientType.Base);
            List<RawIngredientItemData> additiveIngredients = preparedIngredients.FindAll(i => i != null && i.m_ingredientType == RawIngredientType.Additive);

            // 1. Check Base Recipes
            foreach (var recipe in m_recipes)
            {
                if (recipe == null || recipe.m_isUnlocked) continue;

                if (DoesIngredientsMatch(recipe.m_requiredRawIngredients, baseIngredients))
                {
                    recipe.Unlock();
                    Debug.Log($"<color=cyan><b>[RecipeCodex]</b> Recette de base débloquée : {recipe.m_recipeName} !</color>");
                }
            }

            // 2. Check Additive Effects
            foreach (var eff in m_additiveEffects)
            {
                if (eff == null || eff.m_isUnlocked) continue;

                if (eff.MatchesAdditives(additiveIngredients))
                {
                    eff.Unlock();
                    Debug.Log($"<color=cyan><b>[RecipeCodex]</b> Effet additif découvert : {eff.m_effectName} !</color>");
                }
            }

            BuildCodexLayout();
        }

        #endregion


        #region Tools and Utilities

        private bool DoesIngredientsMatch(List<RawIngredientItemData> recipeReqs, List<RawIngredientItemData> currentItems)
        {
            if (recipeReqs == null || currentItems == null) return false;
            if (recipeReqs.Count != currentItems.Count) return false;

            List<string> reqIds = new List<string>();
            foreach (var r in recipeReqs) if (r != null) reqIds.Add(r.m_id);

            List<string> curIds = new List<string>();
            foreach (var c in currentItems) if (c != null) curIds.Add(c.m_id);

            reqIds.Sort();
            curIds.Sort();

            for (int i = 0; i < reqIds.Count; i++)
            {
                if (reqIds[i] != curIds[i]) return false;
            }

            return true;
        }

        private void BuildCodexLayout()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null) Destroy(card);
            }
            _spawnedCards.Clear();

            // Populate 3 Columns
            foreach (var recipe in m_recipes)
            {
                if (recipe == null) continue;

                Transform targetContainer = m_columnMixContainer;
                if (recipe.m_cityOrigin == CityOrigin.Neo) targetContainer = m_columnNeoContainer;
                else if (recipe.m_cityOrigin == CityOrigin.Heart) targetContainer = m_columnHeartContainer;

                if (targetContainer != null)
                {
                    CreateRecipeCard(recipe, targetContainer);
                }
            }

            // Populate Additive Effects in Lower Grid
            if (m_additivesGridContainer != null)
            {
                foreach (var eff in m_additiveEffects)
                {
                    if (eff == null) continue;
                    CreateAdditiveCard(eff, m_additivesGridContainer);
                }
            }
        }

        private void CreateRecipeCard(RecipeData recipe, Transform parent)
        {
            GameObject cardObj = null;
            if (m_recipeCardPrefab != null)
            {
                cardObj = Instantiate(m_recipeCardPrefab, parent);
            }
            else
            {
                cardObj = new GameObject($"Recipe_{recipe.m_recipeName}", typeof(RectTransform), typeof(Image), typeof(Button));
                cardObj.transform.SetParent(parent, false);
            }

            var img = cardObj.GetComponent<Image>();
            if (img != null)
            {
                img.color = recipe.m_isUnlocked ? new Color(0.18f, 0.24f, 0.32f, 0.95f) : new Color(0.10f, 0.12f, 0.15f, 0.60f);
            }

            var text = cardObj.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = recipe.m_isUnlocked ? recipe.m_recipeName : "[LOCKED]";
            }

            var button = cardObj.GetComponent<Button>();
            if (button != null)
            {
                var captured = recipe;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectRecipe(captured));
            }

            _spawnedCards.Add(cardObj);
        }

        private void CreateAdditiveCard(AdditiveEffectData effect, Transform parent)
        {
            GameObject cardObj = null;
            if (m_additiveEffectCardPrefab != null)
            {
                cardObj = Instantiate(m_additiveEffectCardPrefab, parent);
            }
            else
            {
                cardObj = new GameObject($"AdditiveEffect_{effect.m_effectName}", typeof(RectTransform), typeof(Image), typeof(Button));
                cardObj.transform.SetParent(parent, false);
            }

            var img = cardObj.GetComponent<Image>();
            if (img != null)
            {
                img.color = effect.m_isUnlocked ? new Color(0.15f, 0.28f, 0.24f, 0.95f) : new Color(0.10f, 0.14f, 0.13f, 0.60f);
            }

            var text = cardObj.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = effect.m_isUnlocked ? effect.m_effectName : "[LOCKED EFFECT]";
            }

            var button = cardObj.GetComponent<Button>();
            if (button != null)
            {
                var captured = effect;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectAdditiveEffect(captured));
            }

            _spawnedCards.Add(cardObj);
        }

        #endregion


        #region Private and Protected

        private readonly List<GameObject> _spawnedCards = new List<GameObject>();

        #endregion
    }
}

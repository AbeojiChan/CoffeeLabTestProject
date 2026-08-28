using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class BrewResultModal : MonoBehaviour
    {
        #region Publics

        [Header("UI Visuals")]
        public Image m_drinkSpriteImage;
        public Text m_headerTitleText;
        public Text m_drinkNameText;
        public Text m_originTagText;
        public Text m_descriptionText;
        public Text m_effectsSummaryText;
        public Image m_cardBackground;

        [Header("Placeholders")]
        public Sprite m_defaultSuccessSprite;
        public Sprite m_failedTrialSprite;

        [Header("Action Buttons")]
        public Button m_serveDrinkButton;
        public Button m_discardButton;

        [Header("Screen Control")]
        public CanvasGroup m_modalCanvasGroup;

        public event Action<RecipeData, List<AdditiveEffectData>> OnDrinkServed;
        public event Action OnDrinkDiscarded;

        public bool IsOpen => _isOpen;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_modalCanvasGroup == null)
            {
                m_modalCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (m_serveDrinkButton != null)
            {
                m_serveDrinkButton.onClick.AddListener(HandleServe);
            }

            if (m_discardButton != null)
            {
                m_discardButton.onClick.AddListener(HandleDiscard);
            }
        }

        private void Start()
        {
            if (!_isOpen)
            {
                SetVisible(false);
            }
        }

        #endregion


        #region Main API

        public void DisplayResult(RecipeData matchedRecipe, List<AdditiveEffectData> matchedAdditiveEffects, List<RawIngredientItemData> usedIngredients)
        {
            _currentRecipe = matchedRecipe;
            _currentAdditiveEffects = matchedAdditiveEffects ?? new List<AdditiveEffectData>();

            if (matchedRecipe != null)
            {
                // --- SUCCESS: KNOWN RECIPE ---
                if (m_headerTitleText != null)
                {
                    m_headerTitleText.text = "❖  FORMULATION RÉUSSIE  ❖";
                    m_headerTitleText.color = new Color(0.20f, 0.90f, 0.50f, 1f);
                }

                if (m_drinkNameText != null)
                {
                    m_drinkNameText.text = matchedRecipe.m_recipeName;
                }

                if (m_originTagText != null)
                {
                    m_originTagText.text = $"[ ORIGINE : {matchedRecipe.CategoryTag.ToUpper()} ]";
                    m_originTagText.color = new Color(1f, 0.85f, 0.35f, 1f);
                }

                if (m_descriptionText != null)
                {
                    m_descriptionText.text = matchedRecipe.m_description;
                }

                if (m_drinkSpriteImage != null)
                {
                    if (matchedRecipe.m_icon != null)
                    {
                        m_drinkSpriteImage.sprite = matchedRecipe.m_icon;
                        m_drinkSpriteImage.color = Color.white;
                    }
                    else if (m_defaultSuccessSprite != null)
                    {
                        m_drinkSpriteImage.sprite = m_defaultSuccessSprite;
                        m_drinkSpriteImage.color = Color.white;
                    }
                    else
                    {
                        m_drinkSpriteImage.color = new Color(0.85f, 0.55f, 0.25f, 1f);
                    }
                }

                // Compile Effects
                List<string> effectLines = new List<string>();
                if (matchedRecipe.m_hasSpecialEffect)
                {
                    effectLines.Add($"• <b>Effet Inhérent :</b> {matchedRecipe.m_inherentEffectName} ({matchedRecipe.m_inherentEffectDescription})");
                }
                else
                {
                    effectLines.Add("• <b>Effet Inhérent :</b> Neutre (Saveur Standard)");
                }

                foreach (var addEff in _currentAdditiveEffects)
                {
                    if (addEff != null)
                    {
                        effectLines.Add($"• <color=#44FFCC><b>Synergie Additive :</b> {addEff.m_effectName}</color> ({addEff.m_effectDescription})");
                    }
                }

                if (m_effectsSummaryText != null)
                {
                    m_effectsSummaryText.text = string.Join("\n", effectLines);
                }
            }
            else
            {
                // --- FAILURE: DUBIOUS BREW TRIAL ---
                int trialNumber = UnityEngine.Random.Range(40, 99);

                if (m_headerTitleText != null)
                {
                    m_headerTitleText.text = "❖  ESSAI NON RÉPERTORIÉ  ❖";
                    m_headerTitleText.color = new Color(0.95f, 0.65f, 0.20f, 1f);
                }

                if (m_drinkNameText != null)
                {
                    m_drinkNameText.text = $"Barista Brew Trial #{trialNumber}";
                }

                if (m_originTagText != null)
                {
                    m_originTagText.text = "[ EXPÉRIMENTATION DOUTEUSE ]";
                    m_originTagText.color = new Color(0.85f, 0.40f, 0.40f, 1f);
                }

                if (m_descriptionText != null)
                {
                    m_descriptionText.text = "Un mélange instable et imprévisible issu d'une alchimie improvisée. L'aspect est trouble et le goût est... particulier, mais techniquement consommable.";
                }

                if (m_drinkSpriteImage != null)
                {
                    if (m_failedTrialSprite != null)
                    {
                        m_drinkSpriteImage.sprite = m_failedTrialSprite;
                        m_drinkSpriteImage.color = Color.white;
                    }
                    else
                    {
                        m_drinkSpriteImage.color = new Color(0.40f, 0.30f, 0.45f, 0.90f); // Murky purple/gray
                    }
                }

                if (m_effectsSummaryText != null)
                {
                    m_effectsSummaryText.text = "<color=#FF6666>• <b>Effet Imprévu :</b> Risque de tremblements légers ou de goût amer persistant.</color>";
                }
            }

            SetVisible(true);
        }

        public void CloseModal()
        {
            SetVisible(false);
        }

        #endregion


        #region Tools and Utilities

        private void HandleServe()
        {
            Debug.Log($"<color=green><b>[BrewResultModal]</b> Drink served: {(_currentRecipe != null ? _currentRecipe.m_recipeName : "Barista Brew Trial")}!</color>");
            CloseModal();
            OnDrinkServed?.Invoke(_currentRecipe, _currentAdditiveEffects);
        }

        private void HandleDiscard()
        {
            Debug.Log("<color=yellow><b>[BrewResultModal]</b> Drink discarded.</color>");
            CloseModal();
            OnDrinkDiscarded?.Invoke();
        }

        private void SetVisible(bool visible)
        {
            _isOpen = visible;
            if (m_modalCanvasGroup != null)
            {
                m_modalCanvasGroup.alpha = visible ? 1f : 0f;
                m_modalCanvasGroup.interactable = visible;
                m_modalCanvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        #endregion


        #region Private and Protected

        private bool _isOpen = false;
        private RecipeData _currentRecipe;
        private List<AdditiveEffectData> _currentAdditiveEffects = new List<AdditiveEffectData>();

        #endregion
    }
}

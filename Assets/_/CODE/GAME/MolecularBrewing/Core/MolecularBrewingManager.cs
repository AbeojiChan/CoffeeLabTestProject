using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    public class MolecularBrewingManager : MonoBehaviour
    {
        #region Publics

        public static MolecularBrewingManager Instance { get; private set; }
        public int m_playerCredits = 5000;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeDatabases();
        }

        private void Start()
        {
            if (_selectionScreen != null)
            {
                SetupUI(_rootCanvas != null ? _rootCanvas : GetComponentInParent<Canvas>(), _selectionScreen, _workspaceScreen, _resultModal, _loadingOverlay, _dialogueScreen, _cityMapScreen, _shopScreen);
            }
        }

        #endregion


        #region Main API

        public void SetupUI(
            Canvas rootCanvas,
            UISelectionScreen selectionScreen,
            UIMolecularWorkspaceScreen workspaceScreen,
            UIBrewResultModal resultModal,
            UISynthesisLoadingOverlay loadingOverlay,
            UIEndOfShiftDialogue dialogueScreen = null,
            UIPersonaCityMapScreen cityMapScreen = null,
            UIRCGShopScreen shopScreen = null)
        {
            _rootCanvas = rootCanvas;
            _selectionScreen = selectionScreen;
            _workspaceScreen = workspaceScreen;
            _resultModal = resultModal;
            _loadingOverlay = loadingOverlay;
            _dialogueScreen = dialogueScreen;
            _cityMapScreen = cityMapScreen;
            _shopScreen = shopScreen;

            if (_selectionScreen != null)
            {
                _selectionScreen.OnSynthesizeRequested += HandleSynthesizeRequested;
            }

            if (_workspaceScreen != null)
            {
                _workspaceScreen.OnBackToPantryRequested += ShowSelectionScreen;
                _workspaceScreen.OnValidateRequested += HandleValidateRequested;
            }

            if (_resultModal != null)
            {
                _resultModal.OnTweakSynthesisRequested += ShowWorkspaceScreen;
                _resultModal.OnEndOfShiftRequested += ShowEndOfShiftDialogue;
            }

            if (_dialogueScreen != null)
            {
                _dialogueScreen.OnOpenCityMapRequested += ShowCityMapScreen;
            }

            if (_cityMapScreen != null)
            {
                _cityMapScreen.OnEnterShopRequested += ShowShopScreen;
                _cityMapScreen.OnReturnToWorkbenchRequested += ShowSelectionScreen;
            }

            if (_shopScreen != null)
            {
                _shopScreen.OnReturnToWorkbenchRequested += ShowSelectionScreen;
                _shopScreen.OnOpenMapRequested += ShowCityMapScreen;
                _shopScreen.OnItemPurchased += HandleShopItemPurchased;
            }

            ShowSelectionScreen();
        }

        public MoleculeDefinition GetMoleculeDefinition(MoleculeType type)
        {
            if (_moleculeDefs.TryGetValue(type, out var def))
            {
                return def;
            }
            return MoleculeDefinition.CreateDefault(type);
        }

        public List<IngredientDefinition> GetAllIngredients() => _ingredients;
        public List<DrinkRecipePattern> GetAllRecipes() => _recipes;
        public List<SpecialEffectDefinition> GetAllSpecialEffects() => _specialEffects;

        public bool IsRecipeDiscovered(string recipeId)
        {
            return !string.IsNullOrEmpty(recipeId) && _discoveredRecipeIds.Contains(recipeId);
        }

        public bool TryDiscoverRecipe(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId)) return false;
            if (!_discoveredRecipeIds.Contains(recipeId))
            {
                _discoveredRecipeIds.Add(recipeId);
                return true;
            }
            return false;
        }

        public HashSet<string> GetDiscoveredRecipeIds() => _discoveredRecipeIds;

        public void ShowSelectionScreen()
        {
            AudioManager.PlayGameBGM();
            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(true);
            if (_workspaceScreen != null) _workspaceScreen.gameObject.SetActive(false);
            if (_resultModal != null) _resultModal.gameObject.SetActive(false);
            if (_loadingOverlay != null) _loadingOverlay.gameObject.SetActive(false);
            if (_dialogueScreen != null) _dialogueScreen.gameObject.SetActive(false);
            if (_cityMapScreen != null) _cityMapScreen.gameObject.SetActive(false);
            if (_shopScreen != null) _shopScreen.gameObject.SetActive(false);
        }

        public void ShowWorkspaceScreen()
        {
            AudioManager.PlayGameBGM();
            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(false);
            if (_workspaceScreen != null) _workspaceScreen.gameObject.SetActive(true);
            if (_resultModal != null) _resultModal.gameObject.SetActive(false);
            if (_loadingOverlay != null) _loadingOverlay.gameObject.SetActive(false);
            if (_dialogueScreen != null) _dialogueScreen.gameObject.SetActive(false);
            if (_cityMapScreen != null) _cityMapScreen.gameObject.SetActive(false);
            if (_shopScreen != null) _shopScreen.gameObject.SetActive(false);
        }

        public void ShowEndOfShiftDialogue()
        {
            AudioManager.PlayDialogBGM();
            if (_resultModal != null) _resultModal.gameObject.SetActive(false);
            if (_workspaceScreen != null) _workspaceScreen.gameObject.SetActive(false);
            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(false);

            if (_dialogueScreen == null && _rootCanvas != null)
            {
                _dialogueScreen = _rootCanvas.GetComponentInChildren<UIEndOfShiftDialogue>(true);
                if (_dialogueScreen == null)
                {
                    GameObject dlgObj = new GameObject("UIEndOfShiftDialogue", typeof(RectTransform));
                    dlgObj.transform.SetParent(_rootCanvas.transform, false);
                    _dialogueScreen = dlgObj.AddComponent<UIEndOfShiftDialogue>();
                    _dialogueScreen.Initialize();
                }
                _dialogueScreen.OnOpenCityMapRequested += ShowCityMapScreen;
            }

            if (_dialogueScreen != null)
            {
                _dialogueScreen.StartDialogueSequence();
            }
            else
            {
                ShowCityMapScreen();
            }
        }

        public void ShowCityMapScreen()
        {
            AudioManager.PlayCityBGM();
            if (_dialogueScreen != null) _dialogueScreen.gameObject.SetActive(false);
            if (_shopScreen != null) _shopScreen.gameObject.SetActive(false);
            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(false);
            if (_workspaceScreen != null) _workspaceScreen.gameObject.SetActive(false);

            if (_cityMapScreen == null && _rootCanvas != null)
            {
                _cityMapScreen = _rootCanvas.GetComponentInChildren<UIPersonaCityMapScreen>(true);
                if (_cityMapScreen == null)
                {
                    GameObject mapObj = new GameObject("UIPersonaCityMapScreen", typeof(RectTransform));
                    mapObj.transform.SetParent(_rootCanvas.transform, false);
                    _cityMapScreen = mapObj.AddComponent<UIPersonaCityMapScreen>();
                    _cityMapScreen.Initialize();
                }
                _cityMapScreen.OnEnterShopRequested += ShowShopScreen;
                _cityMapScreen.OnReturnToWorkbenchRequested += ShowSelectionScreen;
            }

            if (_cityMapScreen != null)
            {
                _cityMapScreen.OpenMap();
            }
            else
            {
                ShowShopScreen();
            }
        }

        public void ShowShopScreen()
        {
            AudioManager.PlayCityBGM();
            if (_cityMapScreen != null) _cityMapScreen.gameObject.SetActive(false);
            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(false);

            if (_shopScreen == null && _rootCanvas != null)
            {
                _shopScreen = _rootCanvas.GetComponentInChildren<UIRCGShopScreen>(true);
                if (_shopScreen == null)
                {
                    GameObject shopObj = new GameObject("UIRCGShopScreen", typeof(RectTransform));
                    shopObj.transform.SetParent(_rootCanvas.transform, false);
                    _shopScreen = shopObj.AddComponent<UIRCGShopScreen>();
                    _shopScreen.Initialize(m_playerCredits);
                }
                _shopScreen.OnReturnToWorkbenchRequested += ShowSelectionScreen;
                _shopScreen.OnOpenMapRequested += ShowCityMapScreen;
                _shopScreen.OnItemPurchased += HandleShopItemPurchased;
            }

            if (_shopScreen != null)
            {
                _shopScreen.OpenShop(m_playerCredits);
            }
            else
            {
                ShowSelectionScreen();
            }
        }

        #endregion


        #region Tools and Utilities

        private void InitializeDatabases()
        {
            foreach (MoleculeType type in Enum.GetValues(typeof(MoleculeType)))
            {
                _moleculeDefs[type] = MoleculeDefinition.CreateDefault(type);
            }

            _ingredients = IngredientDefinition.CreateDefaultIngredients();
            _recipes = DrinkRecipePattern.CreateDefaultRecipes();
            _specialEffects = SpecialEffectDefinition.CreateDefaultEffects();

            _discoveredRecipeIds.Clear();
            foreach (var r in _recipes)
            {
                if (r.m_isDiscoveredByDefault)
                {
                    _discoveredRecipeIds.Add(r.m_id);
                }
            }
        }

        private void HandleSynthesizeRequested(List<IngredientDefinition> selectedIngredients)
        {
            _currentDosedIngredients = selectedIngredients ?? new List<IngredientDefinition>();

            if (_selectionScreen != null) _selectionScreen.gameObject.SetActive(false);

            if (_workspaceScreen != null)
            {
                _workspaceScreen.gameObject.SetActive(true);
                _workspaceScreen.Initialize(selectedIngredients, _recipes, _specialEffects, GetMoleculeDefinition, _rootCanvas);
            }
        }

        private void HandleValidateRequested(List<MoleculeType> placedMolecules, List<(MoleculeType a, MoleculeType b)> activeBonds)
        {
            var evaluation = DrinkEvaluator.Evaluate(placedMolecules, activeBonds, _recipes, _specialEffects, GetMoleculeDefinition);

            bool isNewDiscovery = false;
            if (evaluation.m_matchedRecipe != null && evaluation.m_matchScore >= 60f)
            {
                isNewDiscovery = TryDiscoverRecipe(evaluation.m_matchedRecipe.m_id);
            }

            int count = _currentDosedIngredients != null && _currentDosedIngredients.Count > 0 ? _currentDosedIngredients.Count : 1;
            float t = Mathf.Clamp01((float)(count - 1) / 6f);
            float scaledDuration = Mathf.Lerp(2.0f, 5.0f, t);

            if (_loadingOverlay != null)
            {
                _loadingOverlay.PlaySynthesisSequence(scaledDuration, () =>
                {
                    if (_resultModal != null)
                    {
                        _resultModal.DisplayResult(evaluation, isNewDiscovery);
                    }
                });
            }
            else if (_resultModal != null)
            {
                _resultModal.DisplayResult(evaluation, isNewDiscovery);
            }
        }

        private void HandleShopItemPurchased(ShopItemData item, int qty)
        {
            m_playerCredits -= (item.m_price * qty);

            // Add item to the player workbench stock
            if (_selectionScreen != null)
            {
                if (item.m_preparedIngredient != null)
                {
                    _selectionScreen.AddPreparedIngredient(item.m_preparedIngredient, qty);
                }
                else if (item.m_rawIngredient != null && item.m_rawIngredient.m_preparedOutput != null)
                {
                    _selectionScreen.AddPreparedIngredient(item.m_rawIngredient.m_preparedOutput, qty);
                }
            }
        }

        #endregion


        #region Private and Protected

        private Dictionary<MoleculeType, MoleculeDefinition> _moleculeDefs = new Dictionary<MoleculeType, MoleculeDefinition>();
        private List<IngredientDefinition> _ingredients = new List<IngredientDefinition>();
        private List<DrinkRecipePattern> _recipes = new List<DrinkRecipePattern>();
        private List<SpecialEffectDefinition> _specialEffects = new List<SpecialEffectDefinition>();
        private List<IngredientDefinition> _currentDosedIngredients = new List<IngredientDefinition>();
        private HashSet<string> _discoveredRecipeIds = new HashSet<string>();

        [Header("GameObject HUD Serialized Screens")]
        [SerializeField] private Canvas _rootCanvas;
        [SerializeField] private UISelectionScreen _selectionScreen;
        [SerializeField] private UIMolecularWorkspaceScreen _workspaceScreen;
        [SerializeField] private UIBrewResultModal _resultModal;
        [SerializeField] private UISynthesisLoadingOverlay _loadingOverlay;
        [SerializeField] private UIEndOfShiftDialogue _dialogueScreen;
        [SerializeField] private UIPersonaCityMapScreen _cityMapScreen;
        [SerializeField] private UIRCGShopScreen _shopScreen;

        #endregion
    }
}

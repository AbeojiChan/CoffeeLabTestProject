using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class PreparationMachineScreen : MonoBehaviour
    {
        #region Publics

        [Header("Chamber References (3 Entrances)")]
        public PreparationMachineChamber m_chamberGrinder;   // Beans & Solids
        public PreparationMachineChamber m_chamberPlants;    // Plants & Botanics
        public PreparationMachineChamber m_chamberLiquids;   // Liquids & Solvents

        [Header("Staging Deck (Incoming Ingredients from Tray)")]
        public Transform m_stagingDeckContainer;
        public GameObject m_stagingCardPrefab;
        public Button m_autoDistributeButton;

        [Header("Processing Controls")]
        public HoldToProcessButton m_holdProcessButton;
        public Text m_instructionPromptText;

        [Header("Output Beaker Section")]
        public GameObject m_beakerVisualObject;
        public Image m_beakerLiquidFillImage;
        public Text m_beakerSummaryText;
        public Button m_proceedToSynthesisButton;
        public Button m_backToTabletButton;

        [Header("Synthesis Screen (Phase 3 Canvas)")]
        public MolecularSynthesisScreen m_synthesisScreen;

        [Header("Screen Control")]
        public CanvasGroup m_screenCanvasGroup;

        public event Action<List<RawIngredientItemData>> OnProceedToSynthesis;
        public event Action OnBackToTablet;

        public bool IsScreenOpen => _isOpen;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_screenCanvasGroup == null)
            {
                m_screenCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (m_holdProcessButton != null)
            {
                m_holdProcessButton.OnHoldComplete += HandleProcessCompleted;
                m_holdProcessButton.OnProgressChanged += HandleProcessProgress;
            }

            if (m_autoDistributeButton != null)
            {
                m_autoDistributeButton.onClick.AddListener(AutoDistributeAllToChambers);
            }

            if (m_proceedToSynthesisButton != null)
            {
                m_proceedToSynthesisButton.onClick.AddListener(HandleProceedToSynthesis);
            }

            if (m_backToTabletButton != null)
            {
                m_backToTabletButton.onClick.AddListener(HandleBackToTablet);
            }

            // Wire chamber item removals back to staging deck
            if (m_chamberGrinder != null) m_chamberGrinder.OnIngredientRemoved += ReturnItemToDeck;
            if (m_chamberPlants != null) m_chamberPlants.OnIngredientRemoved += ReturnItemToDeck;
            if (m_chamberLiquids != null) m_chamberLiquids.OnIngredientRemoved += ReturnItemToDeck;
        }

        private void Start()
        {
            if (!_isOpen)
            {
                SetScreenVisible(false);
            }
        }

        #endregion


        #region Main API

        public void OpenWithIngredients(List<RawIngredientItemData> incomingIngredients)
        {
            _allSessionIngredients = incomingIngredients != null
                ? new List<RawIngredientItemData>(incomingIngredients)
                : new List<RawIngredientItemData>();

            _unassignedDeckItems = new List<RawIngredientItemData>(_allSessionIngredients);
            _isProcessed = false;

            if (m_chamberGrinder != null) m_chamberGrinder.ClearChamber();
            if (m_chamberPlants != null) m_chamberPlants.ClearChamber();
            if (m_chamberLiquids != null) m_chamberLiquids.ClearChamber();

            if (m_holdProcessButton != null)
            {
                m_holdProcessButton.ResetButton();
            }

            if (m_proceedToSynthesisButton != null)
            {
                m_proceedToSynthesisButton.interactable = false;
            }

            if (m_beakerVisualObject != null)
            {
                m_beakerVisualObject.SetActive(false);
            }

            UpdateDeckUI();
            UpdatePrompt();
            SetScreenVisible(true);
        }

        public void CloseScreen()
        {
            SetScreenVisible(false);
        }

        public void AutoDistributeAllToChambers()
        {
            if (_isProcessed) return;

            var itemsToDistribute = new List<RawIngredientItemData>(_unassignedDeckItems);
            foreach (var item in itemsToDistribute)
            {
                if (item == null) continue;

                if (item.m_targetChamber == MachineChamberType.Chamber1_Grinder && m_chamberGrinder != null)
                {
                    m_chamberGrinder.AddIngredient(item);
                    _unassignedDeckItems.Remove(item);
                }
                else if (item.m_targetChamber == MachineChamberType.Chamber2_HerbProcessor && m_chamberPlants != null)
                {
                    m_chamberPlants.AddIngredient(item);
                    _unassignedDeckItems.Remove(item);
                }
                else if (item.m_targetChamber == MachineChamberType.Chamber3_LiquidDispenser && m_chamberLiquids != null)
                {
                    m_chamberLiquids.AddIngredient(item);
                    _unassignedDeckItems.Remove(item);
                }
            }

            UpdateDeckUI();
            UpdatePrompt();
        }

        public void PlaceIngredientInChamber(RawIngredientItemData item)
        {
            if (item == null || _isProcessed) return;

            bool placed = false;
            if (item.m_targetChamber == MachineChamberType.Chamber1_Grinder && m_chamberGrinder != null)
            {
                m_chamberGrinder.AddIngredient(item);
                placed = true;
            }
            else if (item.m_targetChamber == MachineChamberType.Chamber2_HerbProcessor && m_chamberPlants != null)
            {
                m_chamberPlants.AddIngredient(item);
                placed = true;
            }
            else if (item.m_targetChamber == MachineChamberType.Chamber3_LiquidDispenser && m_chamberLiquids != null)
            {
                m_chamberLiquids.AddIngredient(item);
                placed = true;
            }

            if (placed)
            {
                _unassignedDeckItems.Remove(item);
                UpdateDeckUI();
                UpdatePrompt();
            }
        }

        #endregion


        #region Tools and Utilities

        private void ReturnItemToDeck(RawIngredientItemData item)
        {
            if (item == null || _isProcessed) return;
            _unassignedDeckItems.Add(item);
            UpdateDeckUI();
            UpdatePrompt();
        }

        private void HandleProcessProgress(float progress)
        {
            bool isProcessing = progress > 0f && progress < 1.0f;
            if (m_chamberGrinder != null) m_chamberGrinder.SetProcessingState(isProcessing, progress);
            if (m_chamberPlants != null) m_chamberPlants.SetProcessingState(isProcessing, progress);
            if (m_chamberLiquids != null) m_chamberLiquids.SetProcessingState(isProcessing, progress);
        }

        private void HandleProcessCompleted()
        {
            _isProcessed = true;

            if (m_chamberGrinder != null) m_chamberGrinder.SetProcessingState(false, 1.0f);
            if (m_chamberPlants != null) m_chamberPlants.SetProcessingState(false, 1.0f);
            if (m_chamberLiquids != null) m_chamberLiquids.SetProcessingState(false, 1.0f);

            if (m_beakerVisualObject != null)
            {
                m_beakerVisualObject.SetActive(true);
            }

            int totalMolecules = 0;
            Dictionary<string, int> moleculeSummary = new Dictionary<string, int>();

            foreach (var ing in _allSessionIngredients)
            {
                if (ing == null || ing.m_yieldMoleculeNames == null) continue;
                foreach (var mol in ing.m_yieldMoleculeNames)
                {
                    if (string.IsNullOrEmpty(mol)) continue;
                    if (!moleculeSummary.ContainsKey(mol)) moleculeSummary[mol] = 0;
                    moleculeSummary[mol]++;
                    totalMolecules++;
                }
            }

            if (m_beakerSummaryText != null)
            {
                List<string> molLines = new List<string>();
                foreach (var kvp in moleculeSummary)
                {
                    molLines.Add($"• {kvp.Value}x {kvp.Key}");
                }
                string summaryStr = molLines.Count > 0 ? string.Join("\n", molLines) : "Aucun extrait";

                m_beakerSummaryText.text = $"<b>BÉCHER PRÉPARÉ</b>\n<i>{_allSessionIngredients.Count} ingrédients traités ({totalMolecules} molécules)</i>\n\n{summaryStr}";
            }

            if (m_instructionPromptText != null)
            {
                m_instructionPromptText.text = "<color=#20DF80><b>PRÉPARATION TERMINÉE !</b> Les extraits sont transférés dans le bécher. Prêt pour la synthèse moléculaire.</color>";
            }

            if (m_proceedToSynthesisButton != null)
            {
                m_proceedToSynthesisButton.interactable = true;
            }
        }

        private void HandleProceedToSynthesis()
        {
            Debug.Log($"<color=green><b>[PreparationMachineScreen]</b> Proceeding to Synthesis Machine with {_allSessionIngredients.Count} prepared ingredients in beaker!</color>");
            
            if (m_synthesisScreen != null)
            {
                m_synthesisScreen.OpenWithPreparedIngredients(_allSessionIngredients);
            }

            OnProceedToSynthesis?.Invoke(_allSessionIngredients);
        }

        private void HandleBackToTablet()
        {
            CloseScreen();
            OnBackToTablet?.Invoke();
        }

        private void SetScreenVisible(bool visible)
        {
            _isOpen = visible;
            if (m_screenCanvasGroup != null)
            {
                m_screenCanvasGroup.alpha = visible ? 1f : 0f;
                m_screenCanvasGroup.interactable = visible;
                m_screenCanvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        private void UpdateDeckUI()
        {
            if (m_stagingDeckContainer == null) return;

            foreach (var card in _spawnedDeckCards)
            {
                if (card != null) Destroy(card);
            }
            _spawnedDeckCards.Clear();

            for (int i = 0; i < _unassignedDeckItems.Count; i++)
            {
                var item = _unassignedDeckItems[i];
                if (item == null) continue;

                GameObject obj = null;
                if (m_stagingCardPrefab != null)
                {
                    obj = Instantiate(m_stagingCardPrefab, m_stagingDeckContainer);
                }
                else
                {
                    obj = new GameObject($"DeckItem_{item.m_displayName}", typeof(RectTransform), typeof(Image), typeof(Button));
                    obj.transform.SetParent(m_stagingDeckContainer, false);
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
                    btn.onClick.AddListener(() => PlaceIngredientInChamber(captured));
                }

                var txt = obj.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.text = item.m_displayName;
                }

                _spawnedDeckCards.Add(obj);
            }
        }

        private void UpdatePrompt()
        {
            if (_isProcessed) return;

            if (m_instructionPromptText != null)
            {
                if (_unassignedDeckItems.Count > 0)
                {
                    m_instructionPromptText.text = $"Placez les ingrédients ({_unassignedDeckItems.Count} restants) dans les 3 entrées de la machine ou cliquez sur 'Distribuer Tout'.";
                }
                else
                {
                    m_instructionPromptText.text = "<color=#FFD759>Tous les ingrédients sont chargés ! Maintenez le bouton <b>HOLD TO PROCESS</b> pour lancer la préparation.</color>";
                }
            }
        }

        #endregion


        #region Private and Protected

        private bool _isOpen = false;
        private bool _isProcessed = false;
        private List<RawIngredientItemData> _allSessionIngredients = new List<RawIngredientItemData>();
        private List<RawIngredientItemData> _unassignedDeckItems = new List<RawIngredientItemData>();
        private readonly List<GameObject> _spawnedDeckCards = new List<GameObject>();

        #endregion
    }
}

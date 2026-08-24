using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MolecularBrewing.Runtime
{
    public class UIPreparationStationModal : MonoBehaviour
    {
        #region Publics

        public event Action<List<IngredientDefinition>> OnBatchPrepared;
        public event Action OnCloseRequested;

        #endregion


        #region Unity API

        private void Update()
        {
            if (_activeRawItem == null || _isProcessingComplete) return;

            if (_isHoldingAction)
            {
                _processProgress += Time.deltaTime * 40f; // ~2.5 seconds to refine a batch
                _processProgress = Mathf.Clamp(_processProgress, 0f, 100f);
                UpdateMinigameVisuals();

                if (_processProgress >= 100f)
                {
                    _isProcessingComplete = true;
                    _isHoldingAction = false;
                    OnMinigameSuccess();
                }
            }
        }

        #endregion


        #region Main API

        public void Initialize(List<RawIngredientData> rawPantry)
        {
            _rawPantry = rawPantry ?? RawIngredientData.CreateDefaultRawPantry();

            if (!HasSerializedBindings())
            {
                BuildUI();
            }
            else
            {
                BindExistingHierarchy();
            }

            SelectApparatus(ApparatusType.CoffeeGrinder);
            RefreshStagedBasket();
            gameObject.SetActive(false);
        }

        public void OpenModal()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            SelectApparatus(_currentApparatus);
            RefreshStagedBasket();
        }

        public void CloseModal()
        {
            AudioManager.StopLoopingSFX();

            // Auto-collect any remaining staged items so work isn't lost
            if (_stagedBatch.Count > 0)
            {
                OnBatchPrepared?.Invoke(new List<IngredientDefinition>(_stagedBatch));
                _stagedBatch.Clear();
            }

            gameObject.SetActive(false);
            OnCloseRequested?.Invoke();
        }

        #endregion


        #region Tools and Utilities

        private void BuildUI()
        {
            var t = UIStyleHelper.Theme;

            RectTransform rootRt = GetComponent<RectTransform>();
            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            // Backdrop
            GameObject backdrop = UIStyleHelper.CreatePanel(transform, "Backdrop", new Color(0.1f, 0.15f, 0.2f, 0.70f));
            RectTransform bdRt = backdrop.GetComponent<RectTransform>();
            bdRt.anchorMin = Vector2.zero;
            bdRt.anchorMax = Vector2.one;
            bdRt.offsetMin = Vector2.zero;
            bdRt.offsetMax = Vector2.zero;

            // Main Modal Window
            GameObject windowObj = UIStyleHelper.CreatePanel(transform, "Window", t.m_panelBg);
            RectTransform wRt = windowObj.GetComponent<RectTransform>();
            wRt.anchorMin = new Vector2(0.04f, 0.04f);
            wRt.anchorMax = new Vector2(0.96f, 0.96f);
            wRt.offsetMin = Vector2.zero;
            wRt.offsetMax = Vector2.zero;

            // Header Bar
            GameObject headerObj = UIStyleHelper.CreatePanel(windowObj.transform, "Header", t.m_headerBg);
            RectTransform hRt = headerObj.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0f, 0.91f);
            hRt.anchorMax = new Vector2(1f, 1f);
            hRt.offsetMin = Vector2.zero;
            hRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(headerObj.transform, "Title", "PREPARATION LAB - REAGENTS REFINING APPARATUS", 16, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

            Button closeBtn = UIStyleHelper.CreateButton(headerObj.transform, "CloseBtn", "WORKBENCH [X]", t.m_btnDanger, Color.white, CloseModal);
            RectTransform cRt = closeBtn.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.86f, 0.15f);
            cRt.anchorMax = new Vector2(0.98f, 0.85f);
            cRt.offsetMin = Vector2.zero;
            cRt.offsetMax = Vector2.zero;

            // Apparatus Selection Tabs Bar
            GameObject tabsBar = UIStyleHelper.CreatePanel(windowObj.transform, "TabsBar", t.m_cardBg);
            RectTransform tbRt = tabsBar.GetComponent<RectTransform>();
            tbRt.anchorMin = new Vector2(0.02f, 0.82f);
            tbRt.anchorMax = new Vector2(0.98f, 0.89f);
            tbRt.offsetMin = Vector2.zero;
            tbRt.offsetMax = Vector2.zero;

            CreateApparatusTab(tabsBar.transform, ApparatusType.CoffeeGrinder, "COFFEE GRINDER", new Vector2(0.01f, 0.1f), new Vector2(0.24f, 0.9f));
            CreateApparatusTab(tabsBar.transform, ApparatusType.BotanicalMortar, "BOTANICAL MORTAR", new Vector2(0.26f, 0.1f), new Vector2(0.49f, 0.9f));
            CreateApparatusTab(tabsBar.transform, ApparatusType.SteamFrother, "STEAM FROTHER", new Vector2(0.51f, 0.1f), new Vector2(0.74f, 0.9f));
            CreateApparatusTab(tabsBar.transform, ApparatusType.SolventKettle, "KETTLE & SOLVENT", new Vector2(0.76f, 0.1f), new Vector2(0.99f, 0.9f));

            // ================= LEFT: RAW INGREDIENT PICKER =================
            GameObject pickerPanel = UIStyleHelper.CreatePanel(windowObj.transform, "PickerPanel", t.m_cardBg);
            RectTransform pkRt = pickerPanel.GetComponent<RectTransform>();
            pkRt.anchorMin = new Vector2(0.02f, 0.03f);
            pkRt.anchorMax = new Vector2(0.28f, 0.80f);
            pkRt.offsetMin = Vector2.zero;
            pkRt.offsetMax = Vector2.zero;

            UIStyleHelper.CreateText(pickerPanel.transform, "Title", "SELECT RAW SUPPLY", 12, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);

            GameObject listObj = new GameObject("List", typeof(RectTransform), typeof(VerticalLayoutGroup));
            listObj.transform.SetParent(pickerPanel.transform, false);
            _rawListContainer = listObj.transform;
            RectTransform lRt = listObj.GetComponent<RectTransform>();
            lRt.anchorMin = new Vector2(0.04f, 0.04f);
            lRt.anchorMax = new Vector2(0.96f, 0.90f);
            lRt.offsetMin = Vector2.zero;
            lRt.offsetMax = Vector2.zero;

            VerticalLayoutGroup vlg = listObj.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 6;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;

            // ================= CENTER: MINIGAME APPARATUS WORKSPACE =================
            GameObject gamePanel = UIStyleHelper.CreatePanel(windowObj.transform, "MinigamePanel", t.m_cardBg);
            RectTransform gpRt = gamePanel.GetComponent<RectTransform>();
            gpRt.anchorMin = new Vector2(0.30f, 0.03f);
            gpRt.anchorMax = new Vector2(0.66f, 0.80f);
            gpRt.offsetMin = Vector2.zero;
            gpRt.offsetMax = Vector2.zero;

            _apparatusHeaderTitle = UIStyleHelper.CreateText(gamePanel.transform, "AppTitle", "APPARATUS WORKSPACE", 14, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            RectTransform ahRt = _apparatusHeaderTitle.rectTransform;
            ahRt.anchorMin = new Vector2(0.05f, 0.91f);
            ahRt.anchorMax = new Vector2(0.95f, 0.98f);
            ahRt.offsetMin = Vector2.zero;
            ahRt.offsetMax = Vector2.zero;

            // Visual Chamber Graphic Frame
            GameObject chamberGraphic = UIStyleHelper.CreatePanel(gamePanel.transform, "GraphicFrame", t.m_wellBg);
            RectTransform cgRt = chamberGraphic.GetComponent<RectTransform>();
            cgRt.anchorMin = new Vector2(0.10f, 0.38f);
            cgRt.anchorMax = new Vector2(0.90f, 0.88f);
            cgRt.offsetMin = Vector2.zero;
            cgRt.offsetMax = Vector2.zero;

            _apparatusGraphicIcon = UIStyleHelper.CreatePanel(chamberGraphic.transform, "IconRing", new Color(0.20f, 0.70f, 0.90f, 0.3f), UIStyleHelper.CircleSprite).GetComponent<Image>();
            RectTransform agRt = _apparatusGraphicIcon.rectTransform;
            agRt.anchorMin = new Vector2(0.25f, 0.20f);
            agRt.anchorMax = new Vector2(0.75f, 0.80f);
            agRt.offsetMin = Vector2.zero;
            agRt.offsetMax = Vector2.zero;

            _apparatusStatusText = UIStyleHelper.CreateText(chamberGraphic.transform, "Status", "Load an ingredient from the left", 11, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform asRt = _apparatusStatusText.rectTransform;
            asRt.anchorMin = new Vector2(0.05f, 0.05f);
            asRt.anchorMax = new Vector2(0.95f, 0.30f);
            asRt.offsetMin = Vector2.zero;
            asRt.offsetMax = Vector2.zero;

            // Progress Bar
            GameObject progressTrack = UIStyleHelper.CreatePanel(gamePanel.transform, "ProgressTrack", t.m_wellBg);
            RectTransform ptRt = progressTrack.GetComponent<RectTransform>();
            ptRt.anchorMin = new Vector2(0.10f, 0.28f);
            ptRt.anchorMax = new Vector2(0.90f, 0.34f);
            ptRt.offsetMin = Vector2.zero;
            ptRt.offsetMax = Vector2.zero;

            GameObject progressFill = UIStyleHelper.CreatePanel(progressTrack.transform, "Fill", t.m_accentCyan);
            _progressBarFill = progressFill.GetComponent<RectTransform>();
            _progressBarFill.anchorMin = Vector2.zero;
            _progressBarFill.anchorMax = new Vector2(0f, 1f);
            _progressBarFill.offsetMin = Vector2.zero;
            _progressBarFill.offsetMax = Vector2.zero;

            _progressLabelText = UIStyleHelper.CreateText(progressTrack.transform, "Label", "REFINING PROGRESS: 0%", 9, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            RectTransform plRt = _progressLabelText.rectTransform;
            plRt.anchorMin = Vector2.zero;
            plRt.anchorMax = Vector2.one;
            plRt.offsetMin = Vector2.zero;
            plRt.offsetMax = Vector2.zero;

            // Minigame Action Trigger Button (Hold to Refine)
            _actionTriggerButton = UIStyleHelper.CreateButton(gamePanel.transform, "ActionTriggerBtn", "HOLD TO REFINE", t.m_btnPrimary, Color.white, null);
            RectTransform atRt = _actionTriggerButton.GetComponent<RectTransform>();
            atRt.anchorMin = new Vector2(0.15f, 0.06f);
            atRt.anchorMax = new Vector2(0.85f, 0.22f);
            atRt.offsetMin = Vector2.zero;
            atRt.offsetMax = Vector2.zero;

            _actionTriggerText = _actionTriggerButton.GetComponentInChildren<TextMeshProUGUI>();

            // Setup EventTrigger for PointerDown / PointerUp
            EventTrigger trigger = _actionTriggerButton.gameObject.AddComponent<EventTrigger>();
            var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            downEntry.callback.AddListener((d) => OnActionPointerDown());
            trigger.triggers.Add(downEntry);

            var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            upEntry.callback.AddListener((d) => OnActionPointerUp());
            trigger.triggers.Add(upEntry);

            // ================= RIGHT: STAGED BASKET & BATCH COLLECTION =================
            GameObject basketPanel = UIStyleHelper.CreatePanel(windowObj.transform, "BasketPanel", t.m_cardBg);
            RectTransform bpRt = basketPanel.GetComponent<RectTransform>();
            bpRt.anchorMin = new Vector2(0.68f, 0.03f);
            bpRt.anchorMax = new Vector2(0.98f, 0.80f);
            bpRt.offsetMin = Vector2.zero;
            bpRt.offsetMax = Vector2.zero;

            _basketHeaderTitle = UIStyleHelper.CreateText(basketPanel.transform, "BasketTitle", "STAGED BASKET (0 ITEMS)", 12, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            RectTransform bhRt = _basketHeaderTitle.rectTransform;
            bhRt.anchorMin = new Vector2(0.04f, 0.91f);
            bhRt.anchorMax = new Vector2(0.96f, 0.98f);
            bhRt.offsetMin = Vector2.zero;
            bhRt.offsetMax = Vector2.zero;

            // Staged Items Scroll/List
            GameObject stagedListObj = new GameObject("StagedList", typeof(RectTransform), typeof(VerticalLayoutGroup));
            stagedListObj.transform.SetParent(basketPanel.transform, false);
            _stagedListContainer = stagedListObj.transform;
            RectTransform slRt = stagedListObj.GetComponent<RectTransform>();
            slRt.anchorMin = new Vector2(0.04f, 0.26f);
            slRt.anchorMax = new Vector2(0.96f, 0.89f);
            slRt.offsetMin = Vector2.zero;
            slRt.offsetMax = Vector2.zero;

            VerticalLayoutGroup svlg = stagedListObj.GetComponent<VerticalLayoutGroup>();
            svlg.spacing = 4;
            svlg.childControlWidth = true;
            svlg.childControlHeight = false;

            // Empty basket prompt
            _emptyBasketText = UIStyleHelper.CreateText(basketPanel.transform, "EmptyBasketPrompt", "No items prepared yet.\nRefine ingredients and they will be stored here in batch!", 10, t.m_textMuted, TextAlignmentOptions.Center, FontStyles.Normal);
            RectTransform ebRt = _emptyBasketText.rectTransform;
            ebRt.anchorMin = new Vector2(0.08f, 0.40f);
            ebRt.anchorMax = new Vector2(0.92f, 0.75f);
            ebRt.offsetMin = Vector2.zero;
            ebRt.offsetMax = Vector2.zero;

            // Collect All to Workbench Button
            _collectAllButton = UIStyleHelper.CreateButton(basketPanel.transform, "CollectAllBtn", "COLLECT ALL TO WORKBENCH >", t.m_accentGreen, Color.white, OnCollectAllClicked);
            RectTransform caRt = _collectAllButton.GetComponent<RectTransform>();
            caRt.anchorMin = new Vector2(0.04f, 0.06f);
            caRt.anchorMax = new Vector2(0.96f, 0.22f);
            caRt.offsetMin = Vector2.zero;
            caRt.offsetMax = Vector2.zero;

            _collectAllButtonText = _collectAllButton.GetComponentInChildren<TextMeshProUGUI>();
            _collectAllButton.interactable = false;
        }

        private void CreateApparatusTab(Transform parent, ApparatusType type, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            var t = UIStyleHelper.Theme;
            Button btn = UIStyleHelper.CreateButton(parent, $"Tab_{type}", label, t.m_wellBg, t.m_textPrimary, () => SelectApparatus(type));
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            _tabButtons[type] = btn;
        }

        private void SelectApparatus(ApparatusType type)
        {
            AudioManager.StopLoopingSFX();
            var t = UIStyleHelper.Theme;
            _currentApparatus = type;

            foreach (var kvp in _tabButtons)
            {
                Image img = kvp.Value.GetComponent<Image>();
                TextMeshProUGUI txt = kvp.Value.GetComponentInChildren<TextMeshProUGUI>();
                if (kvp.Key == type)
                {
                    img.color = t.m_btnPrimary;
                    if (txt != null) txt.color = Color.white;
                }
                else
                {
                    img.color = t.m_wellBg;
                    if (txt != null) txt.color = t.m_textSecondary;
                }
            }

            switch (type)
            {
                case ApparatusType.CoffeeGrinder:
                    _apparatusHeaderTitle.text = "PRECISION SONIC BURR GRINDER";
                    _actionTriggerText.text = "HOLD TO GRIND BEANS";
                    break;
                case ApparatusType.BotanicalMortar:
                    _apparatusHeaderTitle.text = "BOTANICAL MORTAR & SONIC PESTLE";
                    _actionTriggerText.text = "HOLD TO CRUSH CYBER-PLANT";
                    break;
                case ApparatusType.SteamFrother:
                    _apparatusHeaderTitle.text = "THERMAL STEAM WAND & MILK FROTHER";
                    _actionTriggerText.text = "HOLD TO FROTH & AERATE";
                    break;
                case ApparatusType.SolventKettle:
                    _apparatusHeaderTitle.text = "MOLECULAR SOLVENT KETTLE";
                    _actionTriggerText.text = "HOLD TO HEAT & DISSOLVE";
                    break;
            }

            PopulateRawList();
        }

        private void PopulateRawList()
        {
            var t = UIStyleHelper.Theme;

            foreach (Transform child in _rawListContainer)
            {
                Destroy(child.gameObject);
            }

            List<RawIngredientData> matching = _rawPantry.FindAll(r => r.m_requiredApparatus == _currentApparatus);

            for (int i = 0; i < matching.Count; i++)
            {
                var raw = matching[i];
                GameObject itemObj = UIStyleHelper.CreatePanel(_rawListContainer, $"Raw_{raw.m_rawId}", t.m_wellBg);
                RectTransform rt = itemObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(0, 50);

                Button btn = itemObj.AddComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    AudioManager.PlayMenuNav();
                    LoadRawIngredient(raw);
                });

                GameObject pip = UIStyleHelper.CreatePanel(itemObj.transform, "Pip", raw.m_rawColor, UIStyleHelper.CircleSprite);
                RectTransform pRt = pip.GetComponent<RectTransform>();
                pRt.anchorMin = new Vector2(0.04f, 0.20f);
                pRt.anchorMax = new Vector2(0.16f, 0.80f);
                pRt.offsetMin = Vector2.zero;
                pRt.offsetMax = Vector2.zero;

                TextMeshProUGUI nameTxt = UIStyleHelper.CreateText(itemObj.transform, "Name", raw.m_rawName, 10, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
                RectTransform nRt = nameTxt.rectTransform;
                nRt.anchorMin = new Vector2(0.20f, 0.10f);
                nRt.anchorMax = new Vector2(0.96f, 0.90f);
                nRt.offsetMin = Vector2.zero;
                nRt.offsetMax = Vector2.zero;
            }

            if (matching.Count > 0)
            {
                LoadRawIngredient(matching[0]);
            }
            else
            {
                _activeRawItem = null;
                ResetMinigameState();
            }
        }

        private void LoadRawIngredient(RawIngredientData raw)
        {
            _activeRawItem = raw;
            ResetMinigameState();

            _apparatusStatusText.text = $"[Loaded: {raw.m_rawName}]\n{_actionTriggerText.text}";
            _apparatusGraphicIcon.color = new Color(raw.m_rawColor.r, raw.m_rawColor.g, raw.m_rawColor.b, 0.7f);
        }

        private void ResetMinigameState()
        {
            _processProgress = 0f;
            _isHoldingAction = false;
            _isProcessingComplete = false;
            _progressBarFill.anchorMax = new Vector2(0f, 1f);
            _progressLabelText.text = "REFINING PROGRESS: 0%";
            _actionTriggerButton.interactable = _activeRawItem != null;
        }

        private void OnActionPointerDown()
        {
            if (_activeRawItem != null && !_isProcessingComplete)
            {
                _isHoldingAction = true;

                switch (_currentApparatus)
                {
                    case ApparatusType.CoffeeGrinder:
                        AudioManager.PlayCoffeeGrind(true);
                        break;
                    case ApparatusType.BotanicalMortar:
                        AudioManager.PlayLeavesDrying(true);
                        break;
                    case ApparatusType.SteamFrother:
                        AudioManager.PlayMilkFrother(true);
                        break;
                    case ApparatusType.SolventKettle:
                        AudioManager.PlaySolvent(true);
                        break;
                }
            }
        }

        private void OnActionPointerUp()
        {
            _isHoldingAction = false;
            AudioManager.StopLoopingSFX();
        }

        private void UpdateMinigameVisuals()
        {
            float fillRatio = _processProgress / 100f;
            _progressBarFill.anchorMax = new Vector2(fillRatio, 1f);
            _progressLabelText.text = $"REFINING PROGRESS: {Mathf.RoundToInt(_processProgress)}%";

            float pulse = 1f + Mathf.Sin(Time.time * 20f) * 0.08f;
            _apparatusGraphicIcon.transform.localScale = new Vector3(pulse, pulse, 1f);
        }

        private void OnMinigameSuccess()
        {
            AudioManager.StopLoopingSFX();
            var t = UIStyleHelper.Theme;

            // Store the refined ingredient in the batch basket
            var prepared = _activeRawItem.m_preparedOutput;
            _stagedBatch.Add(prepared);
            RefreshStagedBasket();

            _progressBarFill.anchorMax = new Vector2(1f, 1f);
            _progressLabelText.text = "REFINING COMPLETE! STORED IN BASKET";
            _apparatusStatusText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(t.m_accentGreen)}>+1 {prepared.m_displayName} Staged!</color>\nRefine more or collect all to workbench.";
            _apparatusGraphicIcon.transform.localScale = Vector3.one;

            // Reset progress so player can immediately grind/refine another batch if desired
            _isProcessingComplete = false;
            _processProgress = 0f;
        }

        private void RefreshStagedBasket()
        {
            var t = UIStyleHelper.Theme;

            foreach (Transform child in _stagedListContainer)
            {
                Destroy(child.gameObject);
            }

            int count = _stagedBatch.Count;
            _basketHeaderTitle.text = $"STAGED BASKET ({count} READY)";

            if (count == 0)
            {
                _emptyBasketText.gameObject.SetActive(true);
                _collectAllButton.interactable = false;
                _collectAllButtonText.text = "COLLECT ALL TO WORKBENCH >";
            }
            else
            {
                _emptyBasketText.gameObject.SetActive(false);
                _collectAllButton.interactable = true;
                _collectAllButtonText.text = $"COLLECT ALL TO WORKBENCH ({count}) >";

                // Group and display staged counts
                Dictionary<IngredientDefinition, int> grouped = new Dictionary<IngredientDefinition, int>();
                foreach (var item in _stagedBatch)
                {
                    if (!grouped.ContainsKey(item)) grouped[item] = 0;
                    grouped[item]++;
                }

                foreach (var kvp in grouped)
                {
                    var ing = kvp.Key;
                    int qty = kvp.Value;

                    GameObject card = UIStyleHelper.CreatePanel(_stagedListContainer, $"Staged_{ing.m_id}", t.m_wellBg);
                    RectTransform cRt = card.GetComponent<RectTransform>();
                    cRt.sizeDelta = new Vector2(0, 36);

                    // Pip
                    GameObject pip = UIStyleHelper.CreatePanel(card.transform, "Pip", ing.m_iconColor, UIStyleHelper.CircleSprite);
                    RectTransform pRt = pip.GetComponent<RectTransform>();
                    pRt.anchorMin = new Vector2(0.04f, 0.20f);
                    pRt.anchorMax = new Vector2(0.14f, 0.80f);
                    pRt.offsetMin = Vector2.zero;
                    pRt.offsetMax = Vector2.zero;

                    // Name + Qty
                    TextMeshProUGUI nameTxt = UIStyleHelper.CreateText(card.transform, "Name", $"{ing.m_displayName} <color=#{ColorUtility.ToHtmlStringRGB(t.m_accentGreen)}>[x{qty}]</color>", 9, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
                    RectTransform nRt = nameTxt.rectTransform;
                    nRt.anchorMin = new Vector2(0.18f, 0.05f);
                    nRt.anchorMax = new Vector2(0.96f, 0.95f);
                    nRt.offsetMin = Vector2.zero;
                    nRt.offsetMax = Vector2.zero;
                }
            }
        }

        private void OnCollectAllClicked()
        {
            if (_stagedBatch.Count > 0)
            {
                OnBatchPrepared?.Invoke(new List<IngredientDefinition>(_stagedBatch));
                _stagedBatch.Clear();
                RefreshStagedBasket();
                CloseModal();
            }
        }

        private bool HasSerializedBindings()
        {
            return _actionTriggerButton != null && _rawListContainer != null;
        }

        private void BindExistingHierarchy()
        {
            _tabButtons.Clear();
            if (_tabGrinderBtn != null)
            {
                _tabButtons[ApparatusType.CoffeeGrinder] = _tabGrinderBtn;
                _tabGrinderBtn.onClick.AddListener(() => SelectApparatus(ApparatusType.CoffeeGrinder));
            }
            if (_tabMortarBtn != null)
            {
                _tabButtons[ApparatusType.BotanicalMortar] = _tabMortarBtn;
                _tabMortarBtn.onClick.AddListener(() => SelectApparatus(ApparatusType.BotanicalMortar));
            }
            if (_tabFrotherBtn != null)
            {
                _tabButtons[ApparatusType.SteamFrother] = _tabFrotherBtn;
                _tabFrotherBtn.onClick.AddListener(() => SelectApparatus(ApparatusType.SteamFrother));
            }
            if (_tabSolventBtn != null)
            {
                _tabButtons[ApparatusType.SolventKettle] = _tabSolventBtn;
                _tabSolventBtn.onClick.AddListener(() => SelectApparatus(ApparatusType.SolventKettle));
            }

            if (_actionTriggerButton != null)
            {
                EventTrigger trigger = _actionTriggerButton.gameObject.GetComponent<EventTrigger>();
                if (trigger == null) trigger = _actionTriggerButton.gameObject.AddComponent<EventTrigger>();
                trigger.triggers.Clear();

                EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                down.callback.AddListener((d) => OnActionPointerDown());
                trigger.triggers.Add(down);

                EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                up.callback.AddListener((d) => OnActionPointerUp());
                trigger.triggers.Add(up);
            }

            if (_collectAllButton != null)
            {
                _collectAllButton.onClick.AddListener(OnCollectAllClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(CloseModal);
            }
        }

        #endregion


        #region Private and Protected

        [Header("GameObject Serialized Bindings")]
        [SerializeField] private Button _tabGrinderBtn;
        [SerializeField] private Button _tabMortarBtn;
        [SerializeField] private Button _tabFrotherBtn;
        [SerializeField] private Button _tabSolventBtn;
        [SerializeField] private Transform _rawListContainer;
        [SerializeField] private TextMeshProUGUI _apparatusHeaderTitle;
        [SerializeField] private Image _apparatusGraphicIcon;
        [SerializeField] private TextMeshProUGUI _apparatusStatusText;
        [SerializeField] private RectTransform _progressBarFill;
        [SerializeField] private TextMeshProUGUI _progressLabelText;
        [SerializeField] private Button _actionTriggerButton;
        [SerializeField] private TextMeshProUGUI _actionTriggerText;
        [SerializeField] private TextMeshProUGUI _basketHeaderTitle;
        [SerializeField] private Transform _stagedListContainer;
        [SerializeField] private TextMeshProUGUI _emptyBasketText;
        [SerializeField] private Button _collectAllButton;
        [SerializeField] private TextMeshProUGUI _collectAllButtonText;
        [SerializeField] private Button _closeButton;

        private List<RawIngredientData> _rawPantry;
        private ApparatusType _currentApparatus = ApparatusType.CoffeeGrinder;
        private RawIngredientData _activeRawItem;
        private List<IngredientDefinition> _stagedBatch = new List<IngredientDefinition>();

        private Dictionary<ApparatusType, Button> _tabButtons = new Dictionary<ApparatusType, Button>();
        private bool _isHoldingAction;
        private float _processProgress;
        private bool _isProcessingComplete;

        #endregion
    }
}

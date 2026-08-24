//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    public class UISelectionScreen : MonoBehaviour
//    {
//        #region Publics

//        public event Action<List<IngredientDefinition>> OnSynthesizeRequested;

//        #endregion


//        #region Unity API

//        #endregion


//        #region Main API

//        public void Initialize(List<IngredientDefinition> allAvailableIngredients)
//        {
//            _allAvailableIngredients = allAvailableIngredients;
//            _rootCanvas = GetComponentInParent<Canvas>();

//            if (!HasSerializedBindings())
//            {
//                BuildUI();
//                BuildPrepModal();
//                BuildRecipeBookModal();
//            }
//            else
//            {
//                BindExistingHierarchy();
//            }

//            RefreshPreparedShelf();
//        }

//        public void AddPreparedIngredient(IngredientDefinition ingredient, int batchQuantity = 1)
//        {
//            if (ingredient == null) return;

//            _preparedLookup[ingredient.m_id] = ingredient;

//            if (!_preparedStock.ContainsKey(ingredient.m_id))
//            {
//                _preparedStock[ingredient.m_id] = 0;
//            }

//            _preparedStock[ingredient.m_id] += batchQuantity;

//            RefreshPreparedShelf();
//            ShowIngredientDetail(ingredient);
//        }

//        public void TryDoseIngredient(IngredientDefinition ingredient)
//        {
//            if (ingredient == null || _beakerZone == null) return;

//            if (_preparedStock.TryGetValue(ingredient.m_id, out int stock) && stock > 0)
//            {
//                bool added = _beakerZone.AddDose(ingredient);
//                if (added)
//                {
//                    _preparedStock[ingredient.m_id]--;
//                    RefreshPreparedShelf();
//                    ShowIngredientDetail(ingredient);
//                }
//            }
//        }

//        public void RefundIngredientStock(IngredientDefinition ingredient, int count = 1)
//        {
//            if (ingredient == null) return;

//            if (!_preparedStock.ContainsKey(ingredient.m_id))
//            {
//                _preparedStock[ingredient.m_id] = 0;
//            }

//            _preparedStock[ingredient.m_id] += count;
//            RefreshPreparedShelf();
//        }

//        #endregion


//        #region Tools and Utilities

//        private void BuildUI()
//        {
//            var t = UIStyleHelper.Theme;

//            RectTransform rootRt = GetComponent<RectTransform>();
//            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
//            rootRt.anchorMin = Vector2.zero;
//            rootRt.anchorMax = Vector2.one;
//            rootRt.offsetMin = Vector2.zero;
//            rootRt.offsetMax = Vector2.zero;

//            // Background
//            GameObject bgObj = UIStyleHelper.CreatePanel(transform, "Bg", t.m_background);
//            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
//            bgRt.anchorMin = Vector2.zero;
//            bgRt.anchorMax = Vector2.one;
//            bgRt.offsetMin = Vector2.zero;
//            bgRt.offsetMax = Vector2.zero;

//            // Top Header Bar
//            GameObject headerObj = UIStyleHelper.CreatePanel(transform, "Header", t.m_headerBg);
//            RectTransform hRt = headerObj.GetComponent<RectTransform>();
//            hRt.anchorMin = new Vector2(0f, 0.91f);
//            hRt.anchorMax = new Vector2(1f, 1f);
//            hRt.offsetMin = new Vector2(15, 6);
//            hRt.offsetMax = new Vector2(-15, -6);

//            // Preparation Lab Header Button (LEFT SIDE)
//            Button openPrepBtn = UIStyleHelper.CreateButton(headerObj.transform, "OpenPrepBtn", "+ PREPARATION LAB", t.m_btnPrimary, Color.white, OpenPreparationLab);
//            RectTransform opRt = openPrepBtn.GetComponent<RectTransform>();
//            opRt.anchorMin = new Vector2(0.01f, 0.15f);
//            opRt.anchorMax = new Vector2(0.18f, 0.85f);
//            opRt.offsetMin = Vector2.zero;
//            opRt.offsetMax = Vector2.zero;

//            // Center Header Title
//            TextMeshProUGUI titleTxt = UIStyleHelper.CreateText(headerObj.transform, "Title", "BARISTA WORKBENCH - PREPARATION & BEAKER DOSING", 15, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform ttRt = titleTxt.rectTransform;
//            ttRt.anchorMin = new Vector2(0.20f, 0f);
//            ttRt.anchorMax = new Vector2(0.80f, 1f);
//            ttRt.offsetMin = Vector2.zero;
//            ttRt.offsetMax = Vector2.zero;

//            // Blueprints Button (RIGHT SIDE)
//            Button blueprintsBtn = UIStyleHelper.CreateButton(headerObj.transform, "BlueprintsBtn", "BLUEPRINTS", t.m_btnPrimary, Color.white, ToggleRecipeBook);
//            RectTransform bpRt = blueprintsBtn.GetComponent<RectTransform>();
//            bpRt.anchorMin = new Vector2(0.82f, 0.15f);
//            bpRt.anchorMax = new Vector2(0.99f, 0.85f);
//            bpRt.offsetMin = Vector2.zero;
//            bpRt.offsetMax = Vector2.zero;

//            // ================= SECTION 1: REAGENT SHELF (LEFT) =================
//            GameObject shelfPanel = UIStyleHelper.CreatePanel(transform, "PantryShelf", t.m_panelBg);
//            RectTransform sRt = shelfPanel.GetComponent<RectTransform>();
//            sRt.anchorMin = new Vector2(0.02f, 0.04f);
//            sRt.anchorMax = new Vector2(0.40f, 0.89f);
//            sRt.offsetMin = Vector2.zero;
//            sRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(shelfPanel.transform, "ShelfTitle", "PREPARED REAGENTS (DRAG TO BEAKER)", 13, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);

//            // Shelf Empty State Prompt & Open Lab Button
//            _emptyShelfPanel = UIStyleHelper.CreatePanel(shelfPanel.transform, "EmptyShelfPrompt", t.m_cardBg);
//            RectTransform esRt = _emptyShelfPanel.GetComponent<RectTransform>();
//            esRt.anchorMin = new Vector2(0.05f, 0.35f);
//            esRt.anchorMax = new Vector2(0.95f, 0.88f);
//            esRt.offsetMin = Vector2.zero;
//            esRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(_emptyShelfPanel.transform, "PromptText", "NO PREPARED REAGENTS ON SHELF\n\nUse the Preparation Lab on the left to grind beans, crush cyber-plants, & froth milk.", 11, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Normal);

//            Button emptyOpenBtn = UIStyleHelper.CreateButton(_emptyShelfPanel.transform, "LaunchPrepBtn", "OPEN PREPARATION LAB >", t.m_accentGreen, Color.white, OpenPreparationLab);
//            RectTransform lpRt = emptyOpenBtn.GetComponent<RectTransform>();
//            lpRt.anchorMin = new Vector2(0.15f, 0.12f);
//            lpRt.anchorMax = new Vector2(0.85f, 0.32f);
//            lpRt.offsetMin = Vector2.zero;
//            lpRt.offsetMax = Vector2.zero;

//            // Shelf Grid Container
//            GameObject gridObj = new GameObject("Grid", typeof(RectTransform), typeof(GridLayoutGroup));
//            gridObj.transform.SetParent(shelfPanel.transform, false);
//            _shelfContainer = gridObj.transform;
//            RectTransform gRt = gridObj.GetComponent<RectTransform>();
//            gRt.anchorMin = new Vector2(0.02f, 0.28f);
//            gRt.anchorMax = new Vector2(0.98f, 0.93f);
//            gRt.offsetMin = Vector2.zero;
//            gRt.offsetMax = Vector2.zero;

//            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
//            glg.cellSize = new Vector2(175, 60);
//            glg.spacing = new Vector2(8, 8);
//            glg.childAlignment = TextAnchor.UpperLeft;

//            // Ingredient Detail Inspector at Bottom of Shelf
//            GameObject detailCard = UIStyleHelper.CreatePanel(shelfPanel.transform, "DetailCard", t.m_cardBg);
//            RectTransform dRt = detailCard.GetComponent<RectTransform>();
//            dRt.anchorMin = new Vector2(0.02f, 0.02f);
//            dRt.anchorMax = new Vector2(0.98f, 0.26f);
//            dRt.offsetMin = Vector2.zero;
//            dRt.offsetMax = Vector2.zero;

//            _ingredientTitleText = UIStyleHelper.CreateText(detailCard.transform, "Title", "Ingredient Details", 12, t.m_accentAmber, TextAlignmentOptions.TopLeft, FontStyles.Bold);
//            RectTransform itRt = _ingredientTitleText.rectTransform;
//            itRt.anchorMin = new Vector2(0.04f, 0.70f);
//            itRt.anchorMax = new Vector2(0.96f, 0.95f);
//            itRt.offsetMin = Vector2.zero;
//            itRt.offsetMax = Vector2.zero;

//            _ingredientDescText = UIStyleHelper.CreateText(detailCard.transform, "Desc", "Prepare an ingredient in the lab to inspect details.", 9, t.m_textSecondary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
//            RectTransform idRt = _ingredientDescText.rectTransform;
//            idRt.anchorMin = new Vector2(0.04f, 0.32f);
//            idRt.anchorMax = new Vector2(0.96f, 0.68f);
//            idRt.offsetMin = Vector2.zero;
//            idRt.offsetMax = Vector2.zero;

//            _moleculeYieldPreviewText = UIStyleHelper.CreateText(detailCard.transform, "Yield", "Yields: -", 9, t.m_accentCyan, TextAlignmentOptions.TopLeft, FontStyles.Bold);
//            RectTransform iyRt = _moleculeYieldPreviewText.rectTransform;
//            iyRt.anchorMin = new Vector2(0.04f, 0.04f);
//            iyRt.anchorMax = new Vector2(0.96f, 0.30f);
//            iyRt.offsetMin = Vector2.zero;
//            iyRt.offsetMax = Vector2.zero;

//            // ================= SECTION 2: WORKBENCH & MEASURING BEAKER (CENTER) =================
//            GameObject workbenchPanel = UIStyleHelper.CreatePanel(transform, "WorkbenchPanel", t.m_panelBg);
//            RectTransform wbRt = workbenchPanel.GetComponent<RectTransform>();
//            wbRt.anchorMin = new Vector2(0.42f, 0.04f);
//            wbRt.anchorMax = new Vector2(0.72f, 0.89f);
//            wbRt.offsetMin = Vector2.zero;
//            wbRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(workbenchPanel.transform, "WorkbenchTitle", "MEASURING BEAKER (DOSING STATION)", 13, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);

//            // Beaker Drop Zone Component
//            GameObject beakerObj = new GameObject("BeakerZone", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            beakerObj.transform.SetParent(workbenchPanel.transform, false);
//            _beakerZone = beakerObj.AddComponent<UIBeakerDropZone>();
//            _beakerZone.Initialize(_rootCanvas);
//            _beakerZone.OnDosingChanged += HandleDosingChanged;
//            _beakerZone.OnDoseRemoved += (ing) => RefundIngredientStock(ing, 1);
//            _beakerZone.OnBeakerCleared += (list) =>
//            {
//                foreach (var ing in list) RefundIngredientStock(ing, 1);
//            };
//            _beakerZone.OnBeakerDroppedOnMachine += (b, p) => HandleBeakerDroppedOnMachine(b, p);

//            RectTransform bkRt = beakerObj.GetComponent<RectTransform>();
//            bkRt.anchorMin = new Vector2(0.06f, 0.18f);
//            bkRt.anchorMax = new Vector2(0.94f, 0.92f);
//            bkRt.offsetMin = Vector2.zero;
//            bkRt.offsetMax = Vector2.zero;

//            // Live Molecule Yield Total Display on Beaker
//            GameObject yieldCard = UIStyleHelper.CreatePanel(workbenchPanel.transform, "YieldCard", t.m_cardBg);
//            RectTransform ycRt = yieldCard.GetComponent<RectTransform>();
//            ycRt.anchorMin = new Vector2(0.06f, 0.08f);
//            ycRt.anchorMax = new Vector2(0.94f, 0.16f);
//            ycRt.offsetMin = Vector2.zero;
//            ycRt.offsetMax = Vector2.zero;

//            _totalYieldText = UIStyleHelper.CreateText(yieldCard.transform, "TotalYield", "Total Molecules in Beaker: 0", 9, t.m_accentCyan, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform tyRt = _totalYieldText.rectTransform;
//            tyRt.anchorMin = Vector2.zero;
//            tyRt.anchorMax = Vector2.one;
//            tyRt.offsetMin = Vector2.zero;
//            tyRt.offsetMax = Vector2.zero;

//            // Rinse Beaker Button
//            Button rinseBtn = UIStyleHelper.CreateButton(workbenchPanel.transform, "RinseBtn", "RINSE BEAKER (REFUND)", t.m_btnDanger, Color.white, () => _beakerZone.ClearBeaker(notifyRefund: true));
//            RectTransform rnRt = rinseBtn.GetComponent<RectTransform>();
//            rnRt.anchorMin = new Vector2(0.06f, 0.02f);
//            rnRt.anchorMax = new Vector2(0.94f, 0.07f);
//            rnRt.offsetMin = Vector2.zero;
//            rnRt.offsetMax = Vector2.zero;

//            // ================= SECTION 3: SYNTHESIS MACHINE DOCK (RIGHT) =================
//            GameObject dockPanel = new GameObject("MachineDockZone", typeof(RectTransform));
//            dockPanel.transform.SetParent(transform, false);
//            _machineDockZone = dockPanel.AddComponent<UIMachineDockZone>();
//            _machineDockZone.Initialize();
//            _machineDockZone.OnDockTriggered += HandleDockTriggered;

//            RectTransform mdRt = dockPanel.GetComponent<RectTransform>();
//            mdRt.anchorMin = new Vector2(0.74f, 0.04f);
//            mdRt.anchorMax = new Vector2(0.98f, 0.89f);
//            mdRt.offsetMin = Vector2.zero;
//            mdRt.offsetMax = Vector2.zero;
//        }

//        private void BuildPrepModal()
//        {
//            GameObject modalObj = new GameObject("PreparationStationModal", typeof(RectTransform));
//            modalObj.transform.SetParent(transform, false);
//            _prepModal = modalObj.AddComponent<UIPreparationStationModal>();
//            _prepModal.Initialize(RawIngredientData.CreateDefaultRawPantry());
//            _prepModal.OnBatchPrepared += HandleBatchPrepared;
//        }

//        private void BuildRecipeBookModal()
//        {
//            var t = UIStyleHelper.Theme;

//            _recipeBookPanel = UIStyleHelper.CreatePanel(transform, "RecipeBookModal", t.m_panelBg);
//            RectTransform rbRt = _recipeBookPanel.GetComponent<RectTransform>();
//            rbRt.anchorMin = new Vector2(0.12f, 0.06f);
//            rbRt.anchorMax = new Vector2(0.88f, 0.94f);
//            rbRt.offsetMin = Vector2.zero;
//            rbRt.offsetMax = Vector2.zero;

//            // Header Bar
//            GameObject headerObj = UIStyleHelper.CreatePanel(_recipeBookPanel.transform, "Header", t.m_headerBg);
//            RectTransform hRt = headerObj.GetComponent<RectTransform>();
//            hRt.anchorMin = new Vector2(0.01f, 0.90f);
//            hRt.anchorMax = new Vector2(0.99f, 0.98f);
//            hRt.offsetMin = Vector2.zero;
//            hRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(headerObj.transform, "Title", "CYBER-BARISTA SYNTHESIS BLUEPRINTS", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform ttRt = headerObj.transform.Find("Title").GetComponent<RectTransform>();
//            ttRt.anchorMin = new Vector2(0.03f, 0.45f);
//            ttRt.anchorMax = new Vector2(0.70f, 0.95f);
//            ttRt.offsetMin = Vector2.zero;
//            ttRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(headerObj.transform, "Subtitle", "Target Molecular Formulations & Special Effect Protocols", 9, new Color(0.8f, 0.88f, 0.95f, 0.85f), TextAlignmentOptions.Left, FontStyles.Normal);
//            RectTransform stRt = headerObj.transform.Find("Subtitle").GetComponent<RectTransform>();
//            stRt.anchorMin = new Vector2(0.03f, 0.05f);
//            stRt.anchorMax = new Vector2(0.70f, 0.45f);
//            stRt.offsetMin = Vector2.zero;
//            stRt.offsetMax = Vector2.zero;

//            Button closeBtn = UIStyleHelper.CreateButton(headerObj.transform, "CloseBtn", "CLOSE [X]", t.m_btnDanger, Color.white, () => _recipeBookPanel.SetActive(false));
//            RectTransform cRt = closeBtn.GetComponent<RectTransform>();
//            cRt.anchorMin = new Vector2(0.84f, 0.15f);
//            cRt.anchorMax = new Vector2(0.98f, 0.85f);
//            cRt.offsetMin = Vector2.zero;
//            cRt.offsetMax = Vector2.zero;

//            // Blueprint Cards Scrollable Container
//            GameObject scrollObj = new GameObject("RecipeScroll", typeof(RectTransform), typeof(ScrollRect));
//            scrollObj.transform.SetParent(_recipeBookPanel.transform, false);
//            RectTransform sRt = scrollObj.GetComponent<RectTransform>();
//            sRt.anchorMin = new Vector2(0.02f, 0.03f);
//            sRt.anchorMax = new Vector2(0.98f, 0.88f);
//            sRt.offsetMin = Vector2.zero;
//            sRt.offsetMax = Vector2.zero;

//            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
//            viewport.transform.SetParent(scrollObj.transform, false);
//            RectTransform vpRt = viewport.GetComponent<RectTransform>();
//            vpRt.anchorMin = Vector2.zero;
//            vpRt.anchorMax = Vector2.one;
//            vpRt.offsetMin = Vector2.zero;
//            vpRt.offsetMax = Vector2.zero;

//            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
//            content.transform.SetParent(viewport.transform, false);
//            _recipeCardsContainer = content.transform;
//            RectTransform cntRt = content.GetComponent<RectTransform>();
//            cntRt.anchorMin = new Vector2(0f, 1f);
//            cntRt.anchorMax = new Vector2(1f, 1f);
//            cntRt.pivot = new Vector2(0.5f, 1f);
//            cntRt.sizeDelta = Vector2.zero;

//            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
//            vlg.spacing = 12;
//            vlg.padding = new RectOffset(6, 6, 8, 8);
//            vlg.childControlWidth = true;
//            vlg.childControlHeight = false;
//            vlg.childForceExpandWidth = true;
//            vlg.childForceExpandHeight = false;

//            ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
//            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

//            ScrollRect sr = scrollObj.GetComponent<ScrollRect>();
//            sr.viewport = vpRt;
//            sr.content = cntRt;
//            sr.horizontal = false;
//            sr.vertical = true;
//            sr.scrollSensitivity = 25f;

//            _recipeBookPanel.SetActive(false);
//        }

//        private void ToggleRecipeBook()
//        {
//            if (_recipeBookPanel != null)
//            {
//                bool willBeActive = !_recipeBookPanel.activeSelf;
//                _recipeBookPanel.SetActive(willBeActive);
//                if (willBeActive)
//                {
//                    _recipeBookPanel.transform.SetAsLastSibling();
//                    PopulateRecipeBook();
//                }
//            }
//        }

//        private void PopulateRecipeBook()
//        {
//            if (_recipeCardsContainer == null) return;

//            foreach (Transform child in _recipeCardsContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            var recipes = MolecularBrewingManager.Instance != null ?
//                MolecularBrewingManager.Instance.GetAllRecipes() : DrinkRecipePattern.CreateDefaultRecipes();

//            var t = UIStyleHelper.Theme;
//            int idx = 1;

//            foreach (var recipe in recipes)
//            {
//                bool isDiscovered = MolecularBrewingManager.Instance != null ?
//                    MolecularBrewingManager.Instance.IsRecipeDiscovered(recipe.m_id) : recipe.m_isDiscoveredByDefault;

//                CreateBlueprintCard(_recipeCardsContainer, recipe, idx++, isDiscovered, t);
//            }
//        }

//        private void CreateBlueprintCard(Transform parent, DrinkRecipePattern recipe, int index, bool isDiscovered, UIThemePalette t)
//        {
//            GameObject card = UIStyleHelper.CreatePanel(parent, $"Card_{recipe.m_id}", isDiscovered ? t.m_cardBg : new Color(0.10f, 0.12f, 0.16f, 0.95f));
//            RectTransform cRt = card.GetComponent<RectTransform>();
//            cRt.sizeDelta = new Vector2(0, isDiscovered ? 150 : 135);

//            GameObject tagBar = UIStyleHelper.CreatePanel(card.transform, "TagBar", isDiscovered ? recipe.m_themeColor : new Color(0.85f, 0.20f, 0.25f, 0.9f));
//            RectTransform tbRt = tagBar.GetComponent<RectTransform>();
//            tbRt.anchorMin = new Vector2(0f, 0f);
//            tbRt.anchorMax = new Vector2(0.015f, 1f);
//            tbRt.offsetMin = Vector2.zero;
//            tbRt.offsetMax = Vector2.zero;

//            if (isDiscovered)
//            {
//                UIStyleHelper.CreateText(card.transform, "Title", $"[BLUEPRINT {index:D2}] {recipe.m_displayName.ToUpper()}  <color=#94A3B8>•  \"{recipe.m_tagline}\"</color>", 12, recipe.m_themeColor, TextAlignmentOptions.Left, FontStyles.Bold);
//                RectTransform ttRt = card.transform.Find("Title").GetComponent<RectTransform>();
//                ttRt.anchorMin = new Vector2(0.03f, 0.76f);
//                ttRt.anchorMax = new Vector2(0.97f, 0.96f);
//                ttRt.offsetMin = Vector2.zero;
//                ttRt.offsetMax = Vector2.zero;

//                UIStyleHelper.CreateText(card.transform, "Desc", recipe.m_description, 10, t.m_textSecondary, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform dRt = card.transform.Find("Desc").GetComponent<RectTransform>();
//                dRt.anchorMin = new Vector2(0.03f, 0.52f);
//                dRt.anchorMax = new Vector2(0.97f, 0.74f);
//                dRt.offsetMin = Vector2.zero;
//                dRt.offsetMax = Vector2.zero;

//                GameObject formBox = UIStyleHelper.CreatePanel(card.transform, "FormBox", t.m_wellBg);
//                RectTransform fbRt = formBox.GetComponent<RectTransform>();
//                fbRt.anchorMin = new Vector2(0.03f, 0.08f);
//                fbRt.anchorMax = new Vector2(0.97f, 0.48f);
//                fbRt.offsetMin = Vector2.zero;
//                fbRt.offsetMax = Vector2.zero;

//                List<string> molReqs = new List<string>();
//                foreach (var mr in recipe.m_requiredMolecules)
//                {
//                    molReqs.Add($"{mr.m_minCount}x {mr.m_type}");
//                }
//                string molStr = string.Join("  |  ", molReqs);

//                List<string> bondReqs = new List<string>();
//                foreach (var br in recipe.m_requiredBonds)
//                {
//                    bondReqs.Add($"[{br.m_typeA}] <──(Covalent)──> [{br.m_typeB}]");
//                }
//                string bondStr = bondReqs.Count > 0 ? string.Join("   AND   ", bondReqs) : "None (Free Solution)";

//                UIStyleHelper.CreateText(formBox.transform, "Reqs", $"<color=#F59E0B>• Target Molecules:</color> {molStr}\n<color=#38BDF8>• Chemical Bonds Required:</color> {bondStr}", 10, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform rqRt = formBox.transform.Find("Reqs").GetComponent<RectTransform>();
//                rqRt.anchorMin = new Vector2(0.02f, 0.05f);
//                rqRt.anchorMax = new Vector2(0.98f, 0.95f);
//                rqRt.offsetMin = Vector2.zero;
//                rqRt.offsetMax = Vector2.zero;
//            }
//            else
//            {
//                UIStyleHelper.CreateText(card.transform, "Title", $"[BLUEPRINT {index:D2}] ???  <color=#EF4444>[UNDISCOVERED MOLECULAR BLUEPRINT]</color>", 12, new Color(0.95f, 0.35f, 0.40f, 1f), TextAlignmentOptions.Left, FontStyles.Bold);
//                RectTransform ttRt = card.transform.Find("Title").GetComponent<RectTransform>();
//                ttRt.anchorMin = new Vector2(0.03f, 0.76f);
//                ttRt.anchorMax = new Vector2(0.97f, 0.96f);
//                ttRt.offsetMin = Vector2.zero;
//                ttRt.offsetMax = Vector2.zero;

//                string hintText = string.IsNullOrEmpty(recipe.m_hint) ?
//                    "Formulation classified. Synthesize and link matching reagents in the chamber to decrypt." :
//                    $"<color=#F59E0B>Codex Hint:</color> {recipe.m_hint}";

//                UIStyleHelper.CreateText(card.transform, "Desc", hintText, 10, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform dRt = card.transform.Find("Desc").GetComponent<RectTransform>();
//                dRt.anchorMin = new Vector2(0.03f, 0.52f);
//                dRt.anchorMax = new Vector2(0.97f, 0.74f);
//                dRt.offsetMin = Vector2.zero;
//                dRt.offsetMax = Vector2.zero;

//                GameObject formBox = UIStyleHelper.CreatePanel(card.transform, "FormBox", t.m_wellBg);
//                RectTransform fbRt = formBox.GetComponent<RectTransform>();
//                fbRt.anchorMin = new Vector2(0.03f, 0.08f);
//                fbRt.anchorMax = new Vector2(0.97f, 0.48f);
//                fbRt.offsetMin = Vector2.zero;
//                fbRt.offsetMax = Vector2.zero;

//                UIStyleHelper.CreateText(formBox.transform, "Reqs", "<color=#64748B>• Target Formulation:</color> <color=#94A3B8>[ ??? REDACTED REAGENT RATIOS ??? ]</color>\n<color=#64748B>• Molecular Links:</color> <color=#94A3B8>[ ??? UNRESOLVED COVALENT BONDS - SYNTHESIZE TO DECRYPT ??? ]</color>", 10, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform rqRt = formBox.transform.Find("Reqs").GetComponent<RectTransform>();
//                rqRt.anchorMin = new Vector2(0.02f, 0.05f);
//                rqRt.anchorMax = new Vector2(0.98f, 0.95f);
//                rqRt.offsetMin = Vector2.zero;
//                rqRt.offsetMax = Vector2.zero;
//            }
//        }

//        private void HandleBatchPrepared(List<IngredientDefinition> batch)
//        {
//            if (batch == null || batch.Count == 0) return;

//            foreach (var ing in batch)
//            {
//                AddPreparedIngredient(ing, 1);
//            }
//        }

//        private void OpenPreparationLab()
//        {
//            if (_prepModal != null)
//            {
//                _prepModal.OpenModal();
//            }
//        }

//        private void RefreshPreparedShelf()
//        {
//            foreach (Transform child in _shelfContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            int availableTypes = 0;

//            foreach (var kvp in _preparedStock)
//            {
//                string id = kvp.Key;
//                int stock = kvp.Value;

//                if (stock > 0 && _preparedLookup.TryGetValue(id, out var ing))
//                {
//                    availableTypes++;
//                    CreateDraggableIngredientCard(ing, stock);
//                }
//            }

//            if (availableTypes == 0)
//            {
//                _emptyShelfPanel.SetActive(true);
//                _shelfContainer.gameObject.SetActive(false);
//            }
//            else
//            {
//                _emptyShelfPanel.SetActive(false);
//                _shelfContainer.gameObject.SetActive(true);
//            }
//        }

//        private void CreateDraggableIngredientCard(IngredientDefinition ing, int stock)
//        {
//            GameObject card = new GameObject($"Card_{ing.m_id}", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            card.transform.SetParent(_shelfContainer, false);

//            var draggable = card.AddComponent<UIDraggableIngredientItem>();
//            draggable.Initialize(ing, stock, _rootCanvas);

//            draggable.OnIngredientSelected += (d) =>
//            {
//                ShowIngredientDetail(d.m_ingredient);
//                TryDoseIngredient(d.m_ingredient);
//            };

//            draggable.OnIngredientDroppedOnBeaker += (d, p) =>
//            {
//                if (_beakerZone != null && RectTransformUtility.RectangleContainsScreenPoint(_beakerZone.m_rectTransform, p.position, _rootCanvas != null && _rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? _rootCanvas.worldCamera : null))
//                {
//                    TryDoseIngredient(d.m_ingredient);
//                }
//            };
//        }

//        private void ShowIngredientDetail(IngredientDefinition ing)
//        {
//            if (ing == null) return;

//            _ingredientTitleText.text = ing.m_displayName;
//            _ingredientDescText.text = ing.m_description;

//            List<string> yields = new List<string>();
//            foreach (var y in ing.m_moleculeYields)
//            {
//                yields.Add($"{y.m_count}x {y.m_type}");
//            }
//            _moleculeYieldPreviewText.text = "Yields: " + string.Join(", ", yields);
//        }

//        private void HandleDosingChanged(List<IngredientDefinition> dosed)
//        {
//            int count = dosed.Count;
//            _machineDockZone.SetDockReady(count > 0, count);

//            Dictionary<MoleculeType, int> yieldTotals = new Dictionary<MoleculeType, int>();
//            int totalMols = 0;
//            foreach (var ing in dosed)
//            {
//                foreach (var y in ing.m_moleculeYields)
//                {
//                    if (!yieldTotals.ContainsKey(y.m_type)) yieldTotals[y.m_type] = 0;
//                    yieldTotals[y.m_type] += y.m_count;
//                    totalMols += y.m_count;
//                }
//            }

//            if (totalMols > 0)
//            {
//                List<string> summary = new List<string>();
//                foreach (var kvp in yieldTotals)
//                {
//                    summary.Add($"{kvp.Value}x {kvp.Key}");
//                }
//                _totalYieldText.text = $"Beaker Molecules ({totalMols}): " + string.Join(", ", summary);
//            }
//            else
//            {
//                _totalYieldText.text = "Beaker is empty (Drag prepared reagents to dose)";
//            }
//        }

//        private void HandleBeakerDroppedOnMachine(UIBeakerDropZone beaker, UnityEngine.EventSystems.PointerEventData eventData)
//        {
//            if (_machineDockZone != null && beaker.m_dosedIngredients.Count > 0)
//            {
//                RectTransform mdRt = _machineDockZone.GetComponent<RectTransform>();
//                if (RectTransformUtility.RectangleContainsScreenPoint(mdRt, eventData.position, _rootCanvas != null && _rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? _rootCanvas.worldCamera : null))
//                {
//                    HandleDockTriggered(new List<IngredientDefinition>(beaker.m_dosedIngredients));
//                }
//            }
//        }

//        private void HandleDockTriggered(List<IngredientDefinition> dosedIngredients)
//        {
//            if (dosedIngredients != null && dosedIngredients.Count > 0)
//            {
//                if (_beakerZone != null)
//                {
//                    _beakerZone.ClearBeaker(notifyRefund: false);
//                }

//                OnSynthesizeRequested?.Invoke(dosedIngredients);
//            }
//        }

//        private bool HasSerializedBindings()
//        {
//            return _shelfContainer != null && _beakerZone != null;
//        }

//        private void BindExistingHierarchy()
//        {
//            if (_beakerZone != null)
//            {
//                _beakerZone.Initialize(_rootCanvas);
//                _beakerZone.OnDosingChanged += HandleDosingChanged;
//                _beakerZone.OnDoseRemoved += (ing) => RefundIngredientStock(ing, 1);
//                _beakerZone.OnBeakerCleared += (list) =>
//                {
//                    foreach (var ing in list) RefundIngredientStock(ing, 1);
//                };
//                _beakerZone.OnBeakerDroppedOnMachine += (b, p) => HandleBeakerDroppedOnMachine(b, p);
//            }

//            if (_machineDockZone != null)
//            {
//                _machineDockZone.Initialize();
//                _machineDockZone.OnDockTriggered += HandleDockTriggered;
//            }

//            if (_openPrepBtn != null)
//            {
//                _openPrepBtn.onClick.AddListener(OpenPreparationLab);
//            }

//            if (_blueprintsBtn != null)
//            {
//                _blueprintsBtn.onClick.AddListener(ToggleRecipeBook);
//            }

//            if (_prepModal != null)
//            {
//                _prepModal.Initialize(RawIngredientData.CreateDefaultRawPantry());
//                _prepModal.OnBatchPrepared += HandleBatchPrepared;
//            }

//            if (_recipeBookPanel != null)
//            {
//                _recipeBookPanel.SetActive(false);
//            }
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private Transform _shelfContainer;
//        [SerializeField] private GameObject _emptyShelfPanel;
//        [SerializeField] private UIBeakerDropZone _beakerZone;
//        [SerializeField] private UIMachineDockZone _machineDockZone;
//        [SerializeField] private UIPreparationStationModal _prepModal;
//        [SerializeField] private GameObject _recipeBookPanel;
//        [SerializeField] private Transform _recipeCardsContainer;
//        [SerializeField] private Button _openPrepBtn;
//        [SerializeField] private Button _blueprintsBtn;
//        [SerializeField] private TextMeshProUGUI _ingredientTitleText;
//        [SerializeField] private TextMeshProUGUI _ingredientDescText;
//        [SerializeField] private TextMeshProUGUI _moleculeYieldPreviewText;
//        [SerializeField] private TextMeshProUGUI _totalYieldText;

//        private List<IngredientDefinition> _allAvailableIngredients;
//        private Dictionary<string, int> _preparedStock = new Dictionary<string, int>();
//        private Dictionary<string, IngredientDefinition> _preparedLookup = new Dictionary<string, IngredientDefinition>();
//        private Canvas _rootCanvas;

//        #endregion
//    }
//}

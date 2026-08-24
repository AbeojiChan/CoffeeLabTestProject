//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using TMPro;
//using MolecularBrewing.Runtime;

//namespace MolecularBrewing.Editor
//{
//    public static class UIGameObjectHUDBaker
//    {
//        #region Publics

//        #endregion


//        #region Unity API

//        #endregion


//        #region Main API

//        [MenuItem("Tools/Molecular Brewing/Build GameObject HUD Scene & Prefabs", false, 10)]
//        public static void BuildGameObjectHUDSceneAndPrefabs()
//        {
//            string scenePath = "Assets/Scenes/MolecularBrewing_GameObjectHUD.unity";
//            var activeScene = EditorSceneManager.GetActiveScene();
//            UnityEngine.SceneManagement.Scene scene;
//            if (activeScene.path == scenePath)
//            {
//                scene = activeScene;
//            }
//            else
//            {
//                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
//            }

//            EnsurePrefabDirectories();

//            // 1. Build Master Canvas
//            Canvas masterCanvas = SetupMasterCanvas();

//            // Clear any old UI hierarchy inside MasterCanvas
//            for (int i = masterCanvas.transform.childCount - 1; i >= 0; i--)
//            {
//                UnityEngine.Object.DestroyImmediate(masterCanvas.transform.GetChild(i).gameObject);
//            }

//            var t = UIStyleHelper.Theme;

//            // 2. Build Fully Populated Screen Hierarchies
//            UISelectionScreen selectionScreen = BuildWorkbenchSelectionScreen(masterCanvas.transform, t);
//            UIMolecularWorkspaceScreen workspaceScreen = BuildMolecularWorkspaceScreen(masterCanvas.transform, t);
//            UISynthesisLoadingOverlay loadingOverlay = BuildSynthesisLoadingOverlay(masterCanvas.transform, t);
//            UIBrewResultModal resultModal = BuildBrewResultModal(masterCanvas.transform, t);
//            UIEndOfShiftDialogue dialogueScreen = BuildEndOfShiftDialogueScreen(masterCanvas.transform, t);
//            UIPersonaCityMapScreen cityMapScreen = BuildPersonaCityMapScreen(masterCanvas.transform, t);
//            UIRCGShopScreen shopScreen = BuildRCGShopScreen(masterCanvas.transform, t);

//            // 3. Build & Wire MolecularBrewingManager GameObject
//            GameObject managerObj = GameObject.Find("MolecularBrewingManager");
//            if (managerObj == null) managerObj = new GameObject("MolecularBrewingManager");
//            MolecularBrewingManager manager = managerObj.GetComponent<MolecularBrewingManager>();
//            if (manager == null) manager = managerObj.AddComponent<MolecularBrewingManager>();

//            SerializedObject serializedManager = new SerializedObject(manager);
//            serializedManager.FindProperty("_rootCanvas").objectReferenceValue = masterCanvas;
//            serializedManager.FindProperty("_selectionScreen").objectReferenceValue = selectionScreen;
//            serializedManager.FindProperty("_workspaceScreen").objectReferenceValue = workspaceScreen;
//            serializedManager.FindProperty("_resultModal").objectReferenceValue = resultModal;
//            serializedManager.FindProperty("_loadingOverlay").objectReferenceValue = loadingOverlay;
//            serializedManager.FindProperty("_dialogueScreen").objectReferenceValue = dialogueScreen;
//            serializedManager.FindProperty("_cityMapScreen").objectReferenceValue = cityMapScreen;
//            serializedManager.FindProperty("_shopScreen").objectReferenceValue = shopScreen;
//            serializedManager.ApplyModifiedProperties();

//            // 4. Build & Wire AudioManager GameObject
//            GameObject audioObj = GameObject.Find("AudioManager");
//            if (audioObj == null) audioObj = new GameObject("AudioManager");
//            if (audioObj.GetComponent<AudioManager>() == null) audioObj.AddComponent<AudioManager>();

//            // 5. Ensure EventSystem exists
//            if (GameObject.FindFirstObjectByType<EventSystem>() == null)
//            {
//                GameObject esObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
//            }

//            // 6. Bake Prefabs into Assets/_/CONTENT/Prefabs/UI/
//            BakePrefab(masterCanvas.gameObject, "Assets/_/CONTENT/Prefabs/UI/Canvases/Canvas_MasterRoot.prefab");
//            BakePrefab(selectionScreen.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_WorkbenchSelection.prefab");
//            BakePrefab(workspaceScreen.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_MolecularWorkspace.prefab");
//            BakePrefab(cityMapScreen.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_PersonaCityMap.prefab");
//            BakePrefab(shopScreen.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_RCGShop.prefab");
//            BakePrefab(resultModal.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_BrewResultModal.prefab");
//            BakePrefab(dialogueScreen.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_EndOfShiftDialogue.prefab");
//            BakePrefab(loadingOverlay.gameObject, "Assets/_/CONTENT/Prefabs/UI/Screens/Screen_SynthesisLoadingOverlay.prefab");

//            // Build and bake standalone reusable widgets
//            BuildAndBakeStandaloneWidgets(masterCanvas.transform, t);

//            // 7. Save Scene
//            EditorSceneManager.MarkSceneDirty(scene);
//            EditorSceneManager.SaveScene(scene);
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();

//            Debug.Log("<color=#10B981><b>[UIGameObjectHUDBaker]</b> Fully constructed all GameObject HUD hierarchies, wired serialized components, and baked prefabs in: " + scenePath + "</color>");
//            EditorUtility.DisplayDialog("GameObject HUD Build Complete", "Successfully built fully-populated GameObject HUD hierarchies, pre-wired serialized components, and generated prefabs in:\n" + scenePath, "OK");
//        }

//        #endregion


//        #region Tools and Utilities

//        private static void EnsurePrefabDirectories()
//        {
//            if (!AssetDatabase.IsValidFolder("Assets/_/CONTENT/Prefabs"))
//                AssetDatabase.CreateFolder("Assets/_/CONTENT", "Prefabs");
//            if (!AssetDatabase.IsValidFolder("Assets/_/CONTENT/Prefabs/UI"))
//                AssetDatabase.CreateFolder("Assets/_/CONTENT/Prefabs", "UI");
//            if (!AssetDatabase.IsValidFolder("Assets/_/CONTENT/Prefabs/UI/Canvases"))
//                AssetDatabase.CreateFolder("Assets/_/CONTENT/Prefabs/UI", "Canvases");
//            if (!AssetDatabase.IsValidFolder("Assets/_/CONTENT/Prefabs/UI/Screens"))
//                AssetDatabase.CreateFolder("Assets/_/CONTENT/Prefabs/UI", "Screens");
//            if (!AssetDatabase.IsValidFolder("Assets/_/CONTENT/Prefabs/UI/Widgets"))
//                AssetDatabase.CreateFolder("Assets/_/CONTENT/Prefabs/UI", "Widgets");
//        }

//        private static Canvas SetupMasterCanvas()
//        {
//            GameObject canvasObj = GameObject.Find("HUD_MasterCanvas");
//            if (canvasObj == null)
//            {
//                canvasObj = new GameObject("HUD_MasterCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
//            }

//            Canvas canvas = canvasObj.GetComponent<Canvas>();
//            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

//            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
//            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//            scaler.referenceResolution = new Vector2(1920, 1080);
//            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
//            scaler.matchWidthOrHeight = 0.5f;

//            return canvas;
//        }

//        private static UISelectionScreen BuildWorkbenchSelectionScreen(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_WorkbenchSelection", parent);
//            UISelectionScreen screen = root.AddComponent<UISelectionScreen>();
//            root.AddComponent<CanvasGroup>();

//            // Background
//            CreatePanelObject("Bg", root.transform, Vector2.zero, Vector2.one, t.m_background);

//            // Header Bar
//            GameObject header = CreatePanelObject("Header", root.transform, new Vector2(0f, 0.91f), new Vector2(1f, 1f), t.m_headerBg);
//            RectTransform hRt = header.GetComponent<RectTransform>();
//            hRt.offsetMin = new Vector2(15, 6);
//            hRt.offsetMax = new Vector2(-15, -6);

//            Button openPrepBtn = CreateButtonObject("OpenPrepBtn", header.transform, new Vector2(0.01f, 0.15f), new Vector2(0.18f, 0.85f), "+ PREPARATION LAB", t.m_btnPrimary, Color.white);
//            CreateTextObject("Title", header.transform, new Vector2(0.20f, 0f), new Vector2(0.80f, 1f), "BARISTA WORKBENCH - PREPARATION & BEAKER DOSING", 15, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            Button blueprintsBtn = CreateButtonObject("BlueprintsBtn", header.transform, new Vector2(0.82f, 0.15f), new Vector2(0.99f, 0.85f), "BLUEPRINTS", t.m_btnPrimary, Color.white);

//            // Reagent Shelf (Left)
//            GameObject shelfPanel = CreatePanelObject("PantryShelf", root.transform, new Vector2(0.02f, 0.04f), new Vector2(0.40f, 0.89f), t.m_panelBg);
//            CreateTextObject("ShelfTitle", shelfPanel.transform, new Vector2(0.04f, 0.94f), new Vector2(0.96f, 0.99f), "PREPARED INGREDIENTS SHELF (CLICK/DRAG TO BEAKER)", 13, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject scrollObj = new GameObject("ShelfScroll", typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
//            scrollObj.transform.SetParent(shelfPanel.transform, false);
//            scrollObj.GetComponent<Image>().color = new Color(0, 0, 0, 0.2f);
//            RectTransform scrRt = scrollObj.GetComponent<RectTransform>();
//            scrRt.anchorMin = new Vector2(0.04f, 0.38f);
//            scrRt.anchorMax = new Vector2(0.96f, 0.93f);
//            scrRt.offsetMin = Vector2.zero;
//            scrRt.offsetMax = Vector2.zero;

//            GameObject gridObj = new GameObject("ShelfGrid", typeof(RectTransform), typeof(GridLayoutGroup));
//            gridObj.transform.SetParent(scrollObj.transform, false);
//            RectTransform gRt = gridObj.GetComponent<RectTransform>();
//            gRt.anchorMin = new Vector2(0, 1);
//            gRt.anchorMax = new Vector2(1, 1);
//            gRt.pivot = new Vector2(0.5f, 1);
//            gRt.sizeDelta = new Vector2(0, 600);
//            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
//            glg.cellSize = new Vector2(100, 100);
//            glg.spacing = new Vector2(8, 8);
//            glg.padding = new RectOffset(6, 6, 6, 6);
//            scrollObj.GetComponent<ScrollRect>().content = gRt;

//            GameObject emptyShelf = CreatePanelObject("EmptyShelfPrompt", shelfPanel.transform, new Vector2(0.06f, 0.45f), new Vector2(0.94f, 0.85f), t.m_wellBg);
//            CreateTextObject("PromptText", emptyShelf.transform, Vector2.zero, Vector2.one, "No prepared ingredients on shelf.\nOpen the <color=#38BDF8>PREPARATION LAB</color> above to grind, crush, froth or dissolve raw botanicals!", 12, t.m_textMuted, TextAlignmentOptions.Center, FontStyles.Normal);

//            // Detail Card (Bottom Left)
//            GameObject detailCard = CreatePanelObject("DetailCard", shelfPanel.transform, new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.35f), t.m_cardBg);
//            TextMeshProUGUI ingTitle = CreateTextObject("Title", detailCard.transform, new Vector2(0.04f, 0.72f), new Vector2(0.96f, 0.95f), "Select an ingredient...", 13, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI ingDesc = CreateTextObject("Desc", detailCard.transform, new Vector2(0.04f, 0.32f), new Vector2(0.96f, 0.68f), "Prepare an ingredient in the lab to inspect details.", 9, t.m_textSecondary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
//            TextMeshProUGUI yieldPreview = CreateTextObject("Yield", detailCard.transform, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.30f), "Yields: -", 9, t.m_accentCyan, TextAlignmentOptions.TopLeft, FontStyles.Bold);

//            // Measuring Beaker (Center)
//            GameObject wbPanel = CreatePanelObject("WorkbenchPanel", root.transform, new Vector2(0.42f, 0.04f), new Vector2(0.72f, 0.89f), t.m_panelBg);
//            CreateTextObject("WorkbenchTitle", wbPanel.transform, new Vector2(0.04f, 0.94f), new Vector2(0.96f, 0.99f), "MEASURING BEAKER (DOSING STATION)", 13, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject beakerObj = new GameObject("BeakerZone", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            beakerObj.transform.SetParent(wbPanel.transform, false);
//            UIBeakerDropZone beakerZone = beakerObj.AddComponent<UIBeakerDropZone>();
//            RectTransform bkRt = beakerObj.GetComponent<RectTransform>();
//            bkRt.anchorMin = new Vector2(0.06f, 0.18f);
//            bkRt.anchorMax = new Vector2(0.94f, 0.92f);
//            bkRt.offsetMin = Vector2.zero;
//            bkRt.offsetMax = Vector2.zero;

//            GameObject yieldCard = CreatePanelObject("YieldCard", wbPanel.transform, new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.16f), t.m_cardBg);
//            TextMeshProUGUI totalYield = CreateTextObject("TotalYield", yieldCard.transform, Vector2.zero, Vector2.one, "Total Molecules in Beaker: 0", 9, t.m_accentCyan, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Machine Dock Zone (Right)
//            GameObject dockPanel = CreatePanelObject("DockPanel", root.transform, new Vector2(0.74f, 0.04f), new Vector2(0.98f, 0.89f), t.m_panelBg);
//            CreateTextObject("DockTitle", dockPanel.transform, new Vector2(0.04f, 0.94f), new Vector2(0.96f, 0.99f), "SYNTHESIZER CRUCIBLE", 13, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject dockObj = new GameObject("MachineDockZone", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            dockObj.transform.SetParent(dockPanel.transform, false);
//            UIMachineDockZone dockZone = dockObj.AddComponent<UIMachineDockZone>();
//            RectTransform dkRt = dockObj.GetComponent<RectTransform>();
//            dkRt.anchorMin = new Vector2(0.06f, 0.18f);
//            dkRt.anchorMax = new Vector2(0.94f, 0.92f);
//            dkRt.offsetMin = Vector2.zero;
//            dkRt.offsetMax = Vector2.zero;

//            // Blueprint Modal
//            GameObject recipeModal = CreatePanelObject("RecipeBookModal", root.transform, new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.92f), t.m_panelBg);
//            recipeModal.SetActive(false);
//            GameObject recipeScroll = new GameObject("RecipeScroll", typeof(RectTransform), typeof(ScrollRect));
//            recipeScroll.transform.SetParent(recipeModal.transform, false);
//            RectTransform rscRt = recipeScroll.GetComponent<RectTransform>();
//            rscRt.anchorMin = new Vector2(0.04f, 0.08f);
//            rscRt.anchorMax = new Vector2(0.96f, 0.90f);
//            rscRt.offsetMin = Vector2.zero;
//            rscRt.offsetMax = Vector2.zero;
//            GameObject recipeGrid = new GameObject("RecipeCardsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
//            recipeGrid.transform.SetParent(recipeScroll.transform, false);
//            recipeScroll.GetComponent<ScrollRect>().content = recipeGrid.GetComponent<RectTransform>();

//            // Preparation Lab Modal
//            UIPreparationStationModal prepModal = BuildPreparationStationModal(root.transform, t);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(screen);
//            so.FindProperty("_shelfContainer").objectReferenceValue = gridObj.transform;
//            so.FindProperty("_emptyShelfPanel").objectReferenceValue = emptyShelf;
//            so.FindProperty("_beakerZone").objectReferenceValue = beakerZone;
//            so.FindProperty("_machineDockZone").objectReferenceValue = dockZone;
//            so.FindProperty("_prepModal").objectReferenceValue = prepModal;
//            so.FindProperty("_recipeBookPanel").objectReferenceValue = recipeModal;
//            so.FindProperty("_recipeCardsContainer").objectReferenceValue = recipeGrid.transform;
//            so.FindProperty("_openPrepBtn").objectReferenceValue = openPrepBtn;
//            so.FindProperty("_blueprintsBtn").objectReferenceValue = blueprintsBtn;
//            so.FindProperty("_ingredientTitleText").objectReferenceValue = ingTitle;
//            so.FindProperty("_ingredientDescText").objectReferenceValue = ingDesc;
//            so.FindProperty("_moleculeYieldPreviewText").objectReferenceValue = yieldPreview;
//            so.FindProperty("_totalYieldText").objectReferenceValue = totalYield;
//            so.ApplyModifiedProperties();

//            return screen;
//        }

//        private static UIPreparationStationModal BuildPreparationStationModal(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_PreparationStationModal", parent);
//            UIPreparationStationModal modal = root.AddComponent<UIPreparationStationModal>();
//            root.AddComponent<CanvasGroup>();

//            // Dark Backdrop
//            CreatePanelObject("Backdrop", root.transform, Vector2.zero, Vector2.one, new Color(0.02f, 0.04f, 0.06f, 0.90f));

//            // Modal Card Frame
//            GameObject card = CreatePanelObject("ModalCard", root.transform, new Vector2(0.08f, 0.06f), new Vector2(0.92f, 0.94f), t.m_panelBg);

//            // Header Banner
//            GameObject header = CreatePanelObject("Header", card.transform, new Vector2(0f, 0.90f), new Vector2(1f, 1f), t.m_headerBg);
//            CreateTextObject("Title", header.transform, new Vector2(0.04f, 0f), new Vector2(0.80f, 1f), "BOTANICAL PREPARATION LAB • GRIND, CRUSH, FROTH & DISSOLVE", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            Button closeBtn = CreateButtonObject("CloseBtn", header.transform, new Vector2(0.92f, 0.15f), new Vector2(0.98f, 0.85f), "X", t.m_btnDanger, Color.white);

//            // Left Apparatus Tabs
//            GameObject tabsPanel = CreatePanelObject("ApparatusTabs", card.transform, new Vector2(0.02f, 0.04f), new Vector2(0.24f, 0.88f), t.m_cardBg);
//            CreateTextObject("TabsTitle", tabsPanel.transform, new Vector2(0.05f, 0.92f), new Vector2(0.95f, 0.98f), "STATION APPARATUS", 12, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
//            Button tabGrinder = CreateButtonObject("Tab_Grinder", tabsPanel.transform, new Vector2(0.05f, 0.70f), new Vector2(0.95f, 0.88f), "COFFEE GRINDER", t.m_btnPrimary, Color.white);
//            Button tabMortar = CreateButtonObject("Tab_Mortar", tabsPanel.transform, new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.66f), "BOTANICAL MORTAR", t.m_btnSecondary, Color.white);
//            Button tabFrother = CreateButtonObject("Tab_Frother", tabsPanel.transform, new Vector2(0.05f, 0.26f), new Vector2(0.95f, 0.44f), "STEAM FROTHER", t.m_btnSecondary, Color.white);
//            Button tabSolvent = CreateButtonObject("Tab_Solvent", tabsPanel.transform, new Vector2(0.05f, 0.04f), new Vector2(0.95f, 0.22f), "SOLVENT KETTLE", t.m_btnSecondary, Color.white);

//            // Central Refining Station
//            GameObject refinePanel = CreatePanelObject("RefineStation", card.transform, new Vector2(0.26f, 0.04f), new Vector2(0.68f, 0.88f), t.m_wellBg);
//            TextMeshProUGUI appTitle = CreateTextObject("AppTitle", refinePanel.transform, new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.96f), "ACTIVE STATION: COFFEE GRINDER", 14, t.m_accentCyan, TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject iconObj = CreatePanelObject("AppIcon", refinePanel.transform, new Vector2(0.35f, 0.52f), new Vector2(0.65f, 0.84f), new Color(0.2f, 0.6f, 0.8f, 0.3f), UIStyleHelper.CircleSprite);
//            Image appIcon = iconObj.GetComponent<Image>();

//            TextMeshProUGUI appStatus = CreateTextObject("AppStatus", refinePanel.transform, new Vector2(0.05f, 0.44f), new Vector2(0.95f, 0.50f), "Select an ingredient from raw pantry below to begin", 11, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Normal);

//            // Progress Bar
//            GameObject barBg = CreatePanelObject("ProgressBarBg", refinePanel.transform, new Vector2(0.10f, 0.34f), new Vector2(0.90f, 0.40f), t.m_cardBg);
//            GameObject barFill = CreatePanelObject("Fill", barBg.transform, Vector2.zero, new Vector2(0f, 1f), t.m_accentCyan);
//            RectTransform fillRt = barFill.GetComponent<RectTransform>();
//            TextMeshProUGUI progLabel = CreateTextObject("ProgressLabel", refinePanel.transform, new Vector2(0.10f, 0.26f), new Vector2(0.90f, 0.32f), "READY (0%)", 11, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Action Button
//            Button actionBtn = CreateButtonObject("ActionTriggerBtn", refinePanel.transform, new Vector2(0.20f, 0.08f), new Vector2(0.80f, 0.22f), "HOLD TO GRIND", t.m_btnPrimary, Color.white);
//            TextMeshProUGUI actionTxt = actionBtn.GetComponentInChildren<TextMeshProUGUI>();

//            // Raw Pantry Scroll List (Inside Refine Station Lower)
//            GameObject rawScroll = new GameObject("RawPantryScroll", typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
//            rawScroll.transform.SetParent(refinePanel.transform, false);
//            rawScroll.GetComponent<Image>().color = new Color(0, 0, 0, 0.2f);
//            RectTransform rswRt = rawScroll.GetComponent<RectTransform>();
//            rswRt.anchorMin = new Vector2(0.05f, 0.02f);
//            rswRt.anchorMax = new Vector2(0.95f, 0.06f);
//            rswRt.offsetMin = Vector2.zero;
//            rswRt.offsetMax = Vector2.zero;

//            GameObject rawContainer = new GameObject("RawContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
//            rawContainer.transform.SetParent(rawScroll.transform, false);

//            // Right Staged Batch Basket
//            GameObject basketPanel = CreatePanelObject("StagedBasket", card.transform, new Vector2(0.70f, 0.04f), new Vector2(0.98f, 0.88f), t.m_cardBg);
//            TextMeshProUGUI bskTitle = CreateTextObject("BasketTitle", basketPanel.transform, new Vector2(0.05f, 0.92f), new Vector2(0.95f, 0.98f), "REFINED BATCH BASKET", 12, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject stagedScroll = new GameObject("StagedScroll", typeof(RectTransform), typeof(ScrollRect));
//            stagedScroll.transform.SetParent(basketPanel.transform, false);
//            RectTransform sscRt = stagedScroll.GetComponent<RectTransform>();
//            sscRt.anchorMin = new Vector2(0.05f, 0.16f);
//            sscRt.anchorMax = new Vector2(0.95f, 0.90f);
//            sscRt.offsetMin = Vector2.zero;
//            sscRt.offsetMax = Vector2.zero;

//            GameObject stagedContainer = new GameObject("StagedListContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
//            stagedContainer.transform.SetParent(stagedScroll.transform, false);

//            TextMeshProUGUI emptyBsk = CreateTextObject("EmptyBasketPrompt", basketPanel.transform, new Vector2(0.05f, 0.40f), new Vector2(0.95f, 0.60f), "Basket is empty.\nRefined items appear here.", 10, t.m_textMuted, TextAlignmentOptions.Center, FontStyles.Normal);
//            Button collectBtn = CreateButtonObject("CollectAllBtn", basketPanel.transform, new Vector2(0.05f, 0.04f), new Vector2(0.95f, 0.14f), "TRANSFER TO SHELF", t.m_btnPrimary, Color.white);
//            TextMeshProUGUI collectTxt = collectBtn.GetComponentInChildren<TextMeshProUGUI>();

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(modal);
//            so.FindProperty("_tabGrinderBtn").objectReferenceValue = tabGrinder;
//            so.FindProperty("_tabMortarBtn").objectReferenceValue = tabMortar;
//            so.FindProperty("_tabFrotherBtn").objectReferenceValue = tabFrother;
//            so.FindProperty("_tabSolventBtn").objectReferenceValue = tabSolvent;
//            so.FindProperty("_rawListContainer").objectReferenceValue = rawContainer.transform;
//            so.FindProperty("_apparatusHeaderTitle").objectReferenceValue = appTitle;
//            so.FindProperty("_apparatusGraphicIcon").objectReferenceValue = appIcon;
//            so.FindProperty("_apparatusStatusText").objectReferenceValue = appStatus;
//            so.FindProperty("_progressBarFill").objectReferenceValue = fillRt;
//            so.FindProperty("_progressLabelText").objectReferenceValue = progLabel;
//            so.FindProperty("_actionTriggerButton").objectReferenceValue = actionBtn;
//            so.FindProperty("_actionTriggerText").objectReferenceValue = actionTxt;
//            so.FindProperty("_basketHeaderTitle").objectReferenceValue = bskTitle;
//            so.FindProperty("_stagedListContainer").objectReferenceValue = stagedContainer.transform;
//            so.FindProperty("_emptyBasketText").objectReferenceValue = emptyBsk;
//            so.FindProperty("_collectAllButton").objectReferenceValue = collectBtn;
//            so.FindProperty("_collectAllButtonText").objectReferenceValue = collectTxt;
//            so.FindProperty("_closeButton").objectReferenceValue = closeBtn;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return modal;
//        }

//        private static UIMolecularWorkspaceScreen BuildMolecularWorkspaceScreen(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_MolecularWorkspace", parent);
//            UIMolecularWorkspaceScreen screen = root.AddComponent<UIMolecularWorkspaceScreen>();
//            root.AddComponent<CanvasGroup>();

//            // Background
//            CreatePanelObject("Bg", root.transform, Vector2.zero, Vector2.one, t.m_background);

//            // Header Bar
//            GameObject header = CreatePanelObject("Header", root.transform, new Vector2(0f, 0.91f), new Vector2(1f, 1f), t.m_headerBg);
//            Button backBtn = CreateButtonObject("BackBtn", header.transform, new Vector2(0.01f, 0.15f), new Vector2(0.15f, 0.85f), "< WORKBENCH", t.m_btnSecondary, Color.white);
//            CreateTextObject("Title", header.transform, new Vector2(0.18f, 0f), new Vector2(0.82f, 1f), "MOLECULAR CRUCIBLE • COVALENT LINKING & STABILIZATION", 15, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            Button bpBtn = CreateButtonObject("BlueprintsBtn", header.transform, new Vector2(0.85f, 0.15f), new Vector2(0.99f, 0.85f), "BLUEPRINTS", t.m_btnPrimary, Color.white);

//            // Center Crucible Chamber Viewport
//            GameObject chamberPanel = CreatePanelObject("CrucibleChamber", root.transform, new Vector2(0.02f, 0.08f), new Vector2(0.68f, 0.89f), t.m_panelBg);
//            GameObject lensBezel = CreatePanelObject("LensBezel", chamberPanel.transform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f), t.m_lensBezel, UIStyleHelper.CircleSprite);
//            GameObject lensGlass = CreatePanelObject("LensGlass", lensBezel.transform, new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.98f), t.m_lensGlass, UIStyleHelper.CircleSprite);

//            GameObject linesCont = new GameObject("LinesContainer", typeof(RectTransform));
//            linesCont.transform.SetParent(lensGlass.transform, false);
//            RectTransform lRt = linesCont.GetComponent<RectTransform>();
//            lRt.anchorMin = Vector2.zero;
//            lRt.anchorMax = Vector2.one;
//            lRt.offsetMin = Vector2.zero;
//            lRt.offsetMax = Vector2.zero;

//            GameObject nodesCont = new GameObject("NodesContainer", typeof(RectTransform));
//            nodesCont.transform.SetParent(lensGlass.transform, false);
//            RectTransform nRt = nodesCont.GetComponent<RectTransform>();
//            nRt.anchorMin = Vector2.zero;
//            nRt.anchorMax = Vector2.one;
//            nRt.offsetMin = Vector2.zero;
//            nRt.offsetMax = Vector2.zero;

//            // Crucible Chamber Floating Controls
//            Button freezeBtn = CreateButtonObject("FreezeToggleBtn", chamberPanel.transform, new Vector2(0.04f, 0.88f), new Vector2(0.24f, 0.96f), "FREEZE MOLECULE: OFF", t.m_btnSecondary, Color.white);
//            TextMeshProUGUI freezeTxt = freezeBtn.GetComponentInChildren<TextMeshProUGUI>();

//            Button rescatterBtn = CreateButtonObject("RescatterBtn", chamberPanel.transform, new Vector2(0.26f, 0.88f), new Vector2(0.44f, 0.96f), "RE-SCATTER", t.m_btnSecondary, Color.white);
//            Button clearBondsBtn = CreateButtonObject("ClearBondsBtn", chamberPanel.transform, new Vector2(0.46f, 0.88f), new Vector2(0.64f, 0.96f), "CLEAR BONDS", t.m_btnDanger, Color.white);
//            TextMeshProUGUI linkCountTxt = CreateTextObject("BondCounter", chamberPanel.transform, new Vector2(0.66f, 0.88f), new Vector2(0.96f, 0.96f), "BONDS: 0", 12, t.m_accentCyan, TextAlignmentOptions.Right, FontStyles.Bold);

//            // Match Ribbon & Effect Banner
//            TextMeshProUGUI matchTxt = CreateTextObject("RecipeMatchText", chamberPanel.transform, new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.08f), "REACTION STABILITY: SCATTERED • NO FORMULA MATCHED", 11, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject bannerObj = CreatePanelObject("LiveEffectBanner", chamberPanel.transform, new Vector2(0.04f, 0.09f), new Vector2(0.96f, 0.16f), new Color(0.12f, 0.55f, 0.40f, 0.90f));
//            bannerObj.SetActive(false);
//            TextMeshProUGUI bannerTxt = CreateTextObject("EffectText", bannerObj.transform, Vector2.zero, Vector2.one, "POTENT BIO-EFFECT ACTIVE", 11, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Right Bio-Stats Meters Panel
//            GameObject statsPanel = CreatePanelObject("StatsPanel", root.transform, new Vector2(0.70f, 0.08f), new Vector2(0.98f, 0.89f), t.m_panelBg);
//            CreateTextObject("StatsTitle", statsPanel.transform, new Vector2(0.04f, 0.93f), new Vector2(0.96f, 0.98f), "REAL-TIME BIO-STAT TELEMETRY", 13, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject metersList = new GameObject("MetersList", typeof(RectTransform), typeof(VerticalLayoutGroup));
//            metersList.transform.SetParent(statsPanel.transform, false);
//            RectTransform mlRt = metersList.GetComponent<RectTransform>();
//            mlRt.anchorMin = new Vector2(0.04f, 0.16f);
//            mlRt.anchorMax = new Vector2(0.96f, 0.91f);
//            mlRt.offsetMin = Vector2.zero;
//            mlRt.offsetMax = Vector2.zero;
//            VerticalLayoutGroup vlg = metersList.GetComponent<VerticalLayoutGroup>();
//            vlg.spacing = 14;
//            vlg.childForceExpandHeight = false;

//            UIStatMeter mNeural = BuildStatMeterObject("Meter_NeuralCalm", metersList.transform, "Neural Calm (Anxiety)", new Color(0.05f, 0.70f, 0.60f, 1f));
//            UIStatMeter mSedation = BuildStatMeterObject("Meter_Sedation", metersList.transform, "Sedation (Deep Sleep)", new Color(0.35f, 0.25f, 0.80f, 1f));
//            UIStatMeter mFocus = BuildStatMeterObject("Meter_Focus", metersList.transform, "Cognitive Focus (Energy)", new Color(0.95f, 0.55f, 0.15f, 1f));
//            UIStatMeter mEuphoria = BuildStatMeterObject("Meter_Euphoria", metersList.transform, "Euphoria (Mood Lift)", new Color(0.90f, 0.20f, 0.45f, 1f));
//            UIStatMeter mSweet = BuildStatMeterObject("Meter_Sweetness", metersList.transform, "Sweetness Balance", new Color(0.95f, 0.75f, 0.20f, 1f));
//            UIStatMeter mBitter = BuildStatMeterObject("Meter_Bitterness", metersList.transform, "Bitterness / Astringency", new Color(0.40f, 0.30f, 0.20f, 1f));

//            // Bottom Validate Bar
//            Button validateBtn = CreateButtonObject("ValidateSynthesisBtn", statsPanel.transform, new Vector2(0.04f, 0.02f), new Vector2(0.96f, 0.12f), "SYNTHESIZE & EVALUATE", t.m_btnPrimary, Color.white);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(screen);
//            so.FindProperty("_chamberViewport").objectReferenceValue = lensGlass.transform;
//            so.FindProperty("_linesContainer").objectReferenceValue = linesCont.transform;
//            so.FindProperty("_nodesContainer").objectReferenceValue = nodesCont.transform;
//            so.FindProperty("_freezeToggleBtn").objectReferenceValue = freezeBtn;
//            so.FindProperty("_freezeToggleText").objectReferenceValue = freezeTxt;
//            so.FindProperty("_rescatterBtn").objectReferenceValue = rescatterBtn;
//            so.FindProperty("_clearLinksBtn").objectReferenceValue = clearBondsBtn;
//            so.FindProperty("_validateBtn").objectReferenceValue = validateBtn;
//            so.FindProperty("_backBtn").objectReferenceValue = backBtn;
//            so.FindProperty("_blueprintsBtn").objectReferenceValue = bpBtn;
//            so.FindProperty("_activeLinkCountText").objectReferenceValue = linkCountTxt;
//            so.FindProperty("_liveEffectBannerPanel").objectReferenceValue = bannerObj;
//            so.FindProperty("_liveEffectBannerText").objectReferenceValue = bannerTxt;
//            so.FindProperty("_liveRecipeMatchText").objectReferenceValue = matchTxt;
//            so.FindProperty("_meterNeuralCalm").objectReferenceValue = mNeural;
//            so.FindProperty("_meterSedation").objectReferenceValue = mSedation;
//            so.FindProperty("_meterFocus").objectReferenceValue = mFocus;
//            so.FindProperty("_meterEuphoria").objectReferenceValue = mEuphoria;
//            so.FindProperty("_meterSweetness").objectReferenceValue = mSweet;
//            so.FindProperty("_meterBitterness").objectReferenceValue = mBitter;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return screen;
//        }

//        private static UISynthesisLoadingOverlay BuildSynthesisLoadingOverlay(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_SynthesisLoadingOverlay", parent);
//            UISynthesisLoadingOverlay overlay = root.AddComponent<UISynthesisLoadingOverlay>();
//            root.AddComponent<CanvasGroup>();

//            // Dark Backdrop
//            CreatePanelObject("Backdrop", root.transform, Vector2.zero, Vector2.one, new Color(0.02f, 0.04f, 0.06f, 0.95f));

//            // Spinner Rings
//            GameObject ring1 = CreatePanelObject("Ring1", root.transform, new Vector2(0.40f, 0.40f), new Vector2(0.60f, 0.60f), new Color(0.2f, 0.8f, 0.9f, 0.3f), UIStyleHelper.CircleSprite);
//            GameObject ring2 = CreatePanelObject("Ring2", root.transform, new Vector2(0.43f, 0.43f), new Vector2(0.57f, 0.57f), new Color(0.9f, 0.3f, 0.6f, 0.3f), UIStyleHelper.CircleSprite);

//            TextMeshProUGUI titleTxt = CreateTextObject("Title", root.transform, new Vector2(0.20f, 0.65f), new Vector2(0.80f, 0.72f), "SYNTHESIZING MOLECULAR BREW...", 18, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Progress Bar
//            GameObject barBg = CreatePanelObject("ProgressBarBg", root.transform, new Vector2(0.30f, 0.32f), new Vector2(0.70f, 0.36f), t.m_wellBg);
//            GameObject barFill = CreatePanelObject("Fill", barBg.transform, Vector2.zero, new Vector2(0f, 1f), t.m_accentCyan);
//            RectTransform fillRt = barFill.GetComponent<RectTransform>();

//            TextMeshProUGUI statusTxt = CreateTextObject("Status", root.transform, new Vector2(0.20f, 0.25f), new Vector2(0.80f, 0.30f), "Stabilizing Covalent Bonds...", 12, t.m_textSecondary, TextAlignmentOptions.Center, FontStyles.Normal);
//            TextMeshProUGUI pctTxt = CreateTextObject("Pct", root.transform, new Vector2(0.40f, 0.37f), new Vector2(0.60f, 0.41f), "0%", 11, t.m_accentCyan, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(overlay);
//            so.FindProperty("_spinnerRing1").objectReferenceValue = ring1.GetComponent<RectTransform>();
//            so.FindProperty("_spinnerRing2").objectReferenceValue = ring2.GetComponent<RectTransform>();
//            so.FindProperty("_progressFillRect").objectReferenceValue = fillRt;
//            so.FindProperty("_titleText").objectReferenceValue = titleTxt;
//            so.FindProperty("_statusText").objectReferenceValue = statusTxt;
//            so.FindProperty("_percentageText").objectReferenceValue = pctTxt;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return overlay;
//        }

//        private static UIBrewResultModal BuildBrewResultModal(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_BrewResultModal", parent);
//            UIBrewResultModal modal = root.AddComponent<UIBrewResultModal>();
//            root.AddComponent<CanvasGroup>();

//            // Dark Backdrop
//            CreatePanelObject("Backdrop", root.transform, Vector2.zero, Vector2.one, new Color(0.02f, 0.04f, 0.06f, 0.90f));

//            // Result Card Frame
//            GameObject card = CreatePanelObject("ResultCard", root.transform, new Vector2(0.12f, 0.06f), new Vector2(0.88f, 0.94f), t.m_panelBg);

//            // Header Ribbon
//            GameObject header = CreatePanelObject("Header", card.transform, new Vector2(0f, 0.88f), new Vector2(1f, 1f), t.m_headerBg);
//            GameObject beakerIcon = CreatePanelObject("DrinkIcon", header.transform, new Vector2(0.02f, 0.15f), new Vector2(0.08f, 0.85f), t.m_accentCyan, UIStyleHelper.CircleSprite);
//            TextMeshProUGUI rankBadge = CreateTextObject("RankBadge", header.transform, new Vector2(0.10f, 0.15f), new Vector2(0.20f, 0.85f), "RANK: S", 14, new Color(0.98f, 0.78f, 0.15f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);
//            TextMeshProUGUI scoreTxt = CreateTextObject("ScoreText", header.transform, new Vector2(0.22f, 0.15f), new Vector2(0.35f, 0.85f), "SCORE: 98/100", 13, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI titleTxt = CreateTextObject("Title", header.transform, new Vector2(0.38f, 0.45f), new Vector2(0.98f, 0.90f), "ESPRESSO NEURALIS", 16, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI subTitleTxt = CreateTextObject("Subtitle", header.transform, new Vector2(0.38f, 0.10f), new Vector2(0.98f, 0.45f), "Perfect Covalent Balance", 11, t.m_textSecondary, TextAlignmentOptions.Left, FontStyles.Normal);

//            // Effect Banner
//            GameObject bannerObj = CreatePanelObject("EffectBanner", card.transform, new Vector2(0.04f, 0.76f), new Vector2(0.96f, 0.86f), new Color(0.15f, 0.55f, 0.40f, 0.90f));
//            TextMeshProUGUI effectTxt = CreateTextObject("EffectText", bannerObj.transform, Vector2.zero, Vector2.one, "[+] SPECIAL PROPERTY: COGNITIVE OVERCLOCK ACTIVE", 12, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            // Bio-Stats Grid
//            GameObject statsGrid = new GameObject("StatsGrid", typeof(RectTransform), typeof(GridLayoutGroup));
//            statsGrid.transform.SetParent(card.transform, false);
//            RectTransform sgRt = statsGrid.GetComponent<RectTransform>();
//            sgRt.anchorMin = new Vector2(0.04f, 0.38f);
//            sgRt.anchorMax = new Vector2(0.96f, 0.74f);
//            sgRt.offsetMin = Vector2.zero;
//            sgRt.offsetMax = Vector2.zero;
//            GridLayoutGroup glg = statsGrid.GetComponent<GridLayoutGroup>();
//            glg.cellSize = new Vector2(250, 30);
//            glg.spacing = new Vector2(16, 8);

//            UIStatMeter mNeural = BuildStatMeterObject("Meter_NeuralCalm", statsGrid.transform, "Neural Calm", new Color(0.05f, 0.70f, 0.60f, 1f));
//            UIStatMeter mSedation = BuildStatMeterObject("Meter_Sedation", statsGrid.transform, "Sedation", new Color(0.35f, 0.25f, 0.80f, 1f));
//            UIStatMeter mFocus = BuildStatMeterObject("Meter_Focus", statsGrid.transform, "Cognitive Focus", new Color(0.95f, 0.55f, 0.15f, 1f));
//            UIStatMeter mEuphoria = BuildStatMeterObject("Meter_Euphoria", statsGrid.transform, "Euphoria", new Color(0.90f, 0.20f, 0.45f, 1f));
//            UIStatMeter mSweet = BuildStatMeterObject("Meter_Sweet", statsGrid.transform, "Sweetness", new Color(0.95f, 0.75f, 0.20f, 1f));
//            UIStatMeter mBitter = BuildStatMeterObject("Meter_Bitter", statsGrid.transform, "Bitterness", new Color(0.40f, 0.30f, 0.20f, 1f));

//            // Critique & Notes Box
//            GameObject critiqueBox = CreatePanelObject("CritiqueBox", card.transform, new Vector2(0.04f, 0.15f), new Vector2(0.96f, 0.36f), t.m_wellBg);
//            TextMeshProUGUI critiqueTxt = CreateTextObject("CritiqueText", critiqueBox.transform, new Vector2(0.03f, 0.45f), new Vector2(0.97f, 0.95f), "Masterful brew! The neural calm and energy focus harmonized flawlessly.", 11, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
//            TextMeshProUGUI tagsTxt = CreateTextObject("TagsText", critiqueBox.transform, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.40f), "Tags: [CALMING] [FOCUS BOOST] [PERFECT RATIO]", 10, t.m_accentCyan, TextAlignmentOptions.BottomLeft, FontStyles.Bold);

//            // Action Buttons
//            Button tweakBtn = CreateButtonObject("TweakBtn", card.transform, new Vector2(0.04f, 0.03f), new Vector2(0.48f, 0.12f), "TWEAK SYNTHESIS", t.m_btnSecondary, Color.white);
//            Button endShiftBtn = CreateButtonObject("EndShiftBtn", card.transform, new Vector2(0.52f, 0.03f), new Vector2(0.96f, 0.12f), "COMPLETE SHIFT & PROCEED >", t.m_btnPrimary, Color.white);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(modal);
//            so.FindProperty("_titleText").objectReferenceValue = titleTxt;
//            so.FindProperty("_subtitleText").objectReferenceValue = subTitleTxt;
//            so.FindProperty("_rankBadgeText").objectReferenceValue = rankBadge;
//            so.FindProperty("_scoreText").objectReferenceValue = scoreTxt;
//            so.FindProperty("_critiqueText").objectReferenceValue = critiqueTxt;
//            so.FindProperty("_tagsText").objectReferenceValue = tagsTxt;
//            so.FindProperty("_drinkHeaderImage").objectReferenceValue = beakerIcon.GetComponent<Image>();
//            so.FindProperty("_effectBannerObj").objectReferenceValue = bannerObj;
//            so.FindProperty("_effectBannerImage").objectReferenceValue = bannerObj.GetComponent<Image>();
//            so.FindProperty("_effectBannerText").objectReferenceValue = effectTxt;
//            so.FindProperty("_tweakBtn").objectReferenceValue = tweakBtn;
//            so.FindProperty("_endOfShiftBtn").objectReferenceValue = endShiftBtn;
//            so.FindProperty("_meterNeuralCalm").objectReferenceValue = mNeural;
//            so.FindProperty("_meterSedation").objectReferenceValue = mSedation;
//            so.FindProperty("_meterFocus").objectReferenceValue = mFocus;
//            so.FindProperty("_meterEuphoria").objectReferenceValue = mEuphoria;
//            so.FindProperty("_meterSweet").objectReferenceValue = mSweet;
//            so.FindProperty("_meterBitter").objectReferenceValue = mBitter;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return modal;
//        }

//        private static UIEndOfShiftDialogue BuildEndOfShiftDialogueScreen(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_EndOfShiftDialogue", parent);
//            UIEndOfShiftDialogue diag = root.AddComponent<UIEndOfShiftDialogue>();
//            root.AddComponent<CanvasGroup>();

//            // Dark Backdrop
//            CreatePanelObject("Backdrop", root.transform, Vector2.zero, Vector2.one, new Color(0.02f, 0.04f, 0.06f, 0.85f));

//            // Dialogue Card Frame
//            GameObject card = CreatePanelObject("DialogueCard", root.transform, new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.45f), t.m_panelBg);

//            // Portrait Ring
//            GameObject portraitRing = CreatePanelObject("PortraitRing", card.transform, new Vector2(0.04f, 0.25f), new Vector2(0.18f, 0.85f), t.m_accentCyan, UIStyleHelper.CircleSprite);
//            CreateTextObject("AvatarIcon", portraitRing.transform, Vector2.zero, Vector2.one, "[BARISTA]", 13, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            CreateTextObject("SpeakerName", card.transform, new Vector2(0.22f, 0.80f), new Vector2(0.70f, 0.95f), "BARISTA LEAD (YOU)", 14, t.m_accentCyan, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI bodyTxt = CreateTextObject("DialogueText", card.transform, new Vector2(0.22f, 0.25f), new Vector2(0.96f, 0.75f), "Phew! That's the final brew order of tonight's shift...", 13, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Normal);

//            Button advanceBtn = CreateButtonObject("AdvanceBtn", card.transform, new Vector2(0.75f, 0.06f), new Vector2(0.96f, 0.20f), "CONTINUE [ENTER]", t.m_btnPrimary, Color.white);
//            TextMeshProUGUI advTxt = advanceBtn.GetComponentInChildren<TextMeshProUGUI>();

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(diag);
//            so.FindProperty("_portraitRing").objectReferenceValue = portraitRing;
//            so.FindProperty("_dialogueText").objectReferenceValue = bodyTxt;
//            so.FindProperty("_advanceButton").objectReferenceValue = advanceBtn;
//            so.FindProperty("_advanceButtonText").objectReferenceValue = advTxt;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return diag;
//        }

//        private static UIPersonaCityMapScreen BuildPersonaCityMapScreen(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_PersonaCityMap", parent);
//            UIPersonaCityMapScreen screen = root.AddComponent<UIPersonaCityMapScreen>();
//            root.AddComponent<CanvasGroup>();

//            // Map Background Panel
//            GameObject mapBg = CreatePanelObject("MapBackground", root.transform, Vector2.zero, Vector2.one, new Color(0.04f, 0.06f, 0.10f, 1f));
//            RectTransform mapRt = mapBg.GetComponent<RectTransform>();

//            // Header Banner
//            GameObject header = CreatePanelObject("Header", root.transform, new Vector2(0f, 0.91f), new Vector2(1f, 1f), t.m_headerBg);
//            CreateTextObject("Title", header.transform, new Vector2(0.03f, 0f), new Vector2(0.60f, 1f), "NEO-FREMIO METROPOLIS • DISTRICT TRANSIT NETWORK", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI ticker = CreateTextObject("TimeTicker", header.transform, new Vector2(0.70f, 0f), new Vector2(0.97f, 1f), "LOCAL TIME: 01:42 AM • NOCTURNAL SHIFT", 11, t.m_accentCyan, TextAlignmentOptions.Right, FontStyles.Bold);

//            // Track Layer
//            GameObject trackLayer = new GameObject("TrackLayer", typeof(RectTransform));
//            trackLayer.transform.SetParent(mapBg.transform, false);
//            RectTransform trackRt = trackLayer.GetComponent<RectTransform>();
//            trackRt.anchorMin = Vector2.zero;
//            trackRt.anchorMax = Vector2.one;
//            trackRt.offsetMin = Vector2.zero;
//            trackRt.offsetMax = Vector2.zero;

//            GameObject trackBg = CreatePanelObject("TrackBg", trackLayer.transform, Vector2.zero, Vector2.zero, new Color(0.2f, 0.3f, 0.4f, 0.5f));
//            GameObject trackFill = CreatePanelObject("TrackFill", trackLayer.transform, Vector2.zero, Vector2.zero, t.m_accentCyan);
//            GameObject beaconObj = CreatePanelObject("TransitBeacon", trackLayer.transform, Vector2.zero, Vector2.zero, Color.yellow, UIStyleHelper.CircleSprite);

//            // District Pins
//            GameObject labPin = CreatePanelObject("Pin_District07Lab", mapBg.transform, new Vector2(0.18f, 0.50f), new Vector2(0.26f, 0.65f), new Color(0.15f, 0.70f, 0.55f, 1f), UIStyleHelper.CircleSprite);
//            GameObject labGlow = CreatePanelObject("GlowRing", labPin.transform, new Vector2(-0.2f, -0.2f), new Vector2(1.2f, 1.2f), new Color(0.15f, 0.70f, 0.55f, 0.4f), UIStyleHelper.CircleSprite);
//            Button labBtn = labPin.AddComponent<Button>();

//            GameObject jojoPin = CreatePanelObject("Pin_District01Jojo", mapBg.transform, new Vector2(0.55f, 0.30f), new Vector2(0.63f, 0.45f), new Color(0.85f, 0.20f, 0.60f, 1f), UIStyleHelper.CircleSprite);
//            GameObject jojoGlow = CreatePanelObject("GlowRing", jojoPin.transform, new Vector2(-0.2f, -0.2f), new Vector2(1.2f, 1.2f), new Color(0.85f, 0.20f, 0.60f, 0.4f), UIStyleHelper.CircleSprite);
//            Button jojoBtn = jojoPin.AddComponent<Button>();

//            // Sidebar Panel
//            GameObject sidebar = CreatePanelObject("Sidebar", root.transform, new Vector2(0.72f, 0.04f), new Vector2(0.98f, 0.89f), t.m_panelBg);
//            TextMeshProUGUI sideTitle = CreateTextObject("SidebarTitle", sidebar.transform, new Vector2(0.06f, 0.88f), new Vector2(0.94f, 0.96f), "DISTRICT 01: JOJO'S APOTHECARY", 14, t.m_accentCyan, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI sideDesc = CreateTextObject("SidebarDesc", sidebar.transform, new Vector2(0.06f, 0.35f), new Vector2(0.94f, 0.85f), "Jojo le Barjo's underground laboratory specializes in rare bio-botanicals, catalysts, and molecular brewing blueprints.", 11, t.m_textSecondary, TextAlignmentOptions.TopLeft, FontStyles.Normal);
//            TextMeshProUGUI statusReadout = CreateTextObject("StatusReadout", sidebar.transform, new Vector2(0.06f, 0.20f), new Vector2(0.94f, 0.30f), "TRANSIT STATUS: READY TO BOARD", 10, t.m_accentGreen, TextAlignmentOptions.Center, FontStyles.Bold);

//            Button travelBtn = CreateButtonObject("TravelButton", sidebar.transform, new Vector2(0.06f, 0.09f), new Vector2(0.94f, 0.18f), "BOARD NIGHT METRO", t.m_btnPrimary, Color.white);
//            Button returnWbBtn = CreateButtonObject("ReturnWorkbenchButton", sidebar.transform, new Vector2(0.06f, 0.02f), new Vector2(0.94f, 0.08f), "< BACK TO WORKBENCH", t.m_btnSecondary, Color.white);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(screen);
//            so.FindProperty("_mapCanvasRt").objectReferenceValue = mapRt;
//            so.FindProperty("_trackLayerRt").objectReferenceValue = trackRt;
//            so.FindProperty("_labNodeObj").objectReferenceValue = labPin;
//            so.FindProperty("_jojoNodeObj").objectReferenceValue = jojoPin;
//            so.FindProperty("_labGlowRing").objectReferenceValue = labGlow;
//            so.FindProperty("_jojoGlowRing").objectReferenceValue = jojoGlow;
//            so.FindProperty("_trackBackgroundObj").objectReferenceValue = trackBg;
//            so.FindProperty("_trackBgRt").objectReferenceValue = trackBg.GetComponent<RectTransform>();
//            so.FindProperty("_trackFillObj").objectReferenceValue = trackFill;
//            so.FindProperty("_trackFillRt").objectReferenceValue = trackFill.GetComponent<RectTransform>();
//            so.FindProperty("_transitBeaconObj").objectReferenceValue = beaconObj;
//            so.FindProperty("_transitBeaconRt").objectReferenceValue = beaconObj.GetComponent<RectTransform>();
//            so.FindProperty("_travelButton").objectReferenceValue = travelBtn;
//            so.FindProperty("_returnToWorkbenchButton").objectReferenceValue = returnWbBtn;
//            so.FindProperty("_statusReadoutText").objectReferenceValue = statusReadout;
//            so.FindProperty("_sidebarDistrictTitle").objectReferenceValue = sideTitle;
//            so.FindProperty("_sidebarDescriptionText").objectReferenceValue = sideDesc;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return screen;
//        }

//        private static UIRCGShopScreen BuildRCGShopScreen(Transform parent, UIThemePalette t)
//        {
//            GameObject root = CreateFullRectObject("Screen_RCGShop", parent);
//            UIRCGShopScreen screen = root.AddComponent<UIRCGShopScreen>();
//            root.AddComponent<CanvasGroup>();

//            // Background
//            CreatePanelObject("Bg", root.transform, Vector2.zero, Vector2.one, new Color(0.08f, 0.06f, 0.14f, 1f));

//            // Header Banner
//            GameObject header = CreatePanelObject("Header", root.transform, new Vector2(0f, 0.91f), new Vector2(1f, 1f), new Color(0.12f, 0.08f, 0.20f, 0.98f));
//            CreateTextObject("Title", header.transform, new Vector2(0.03f, 0f), new Vector2(0.60f, 1f), "JOJO LE BARJO'S APOTHECARY • BLACK MARKET REAGENTS", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject creditBadge = CreatePanelObject("CreditBadge", header.transform, new Vector2(0.70f, 0.12f), new Vector2(0.97f, 0.88f), new Color(0.98f, 0.78f, 0.15f, 1f));
//            TextMeshProUGUI creditTxt = CreateTextObject("CreditText", creditBadge.transform, Vector2.zero, Vector2.one, "CREDITS: 5,000", 13, new Color(0.12f, 0.08f, 0.20f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);

//            // Left Shopkeeper Box
//            GameObject shopkeeperPanel = CreatePanelObject("ShopkeeperPanel", root.transform, new Vector2(0.02f, 0.11f), new Vector2(0.30f, 0.89f), new Color(0.16f, 0.12f, 0.25f, 0.96f));
//            GameObject avatarBox = CreatePanelObject("AvatarBox", shopkeeperPanel.transform, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.95f), new Color(0.24f, 0.18f, 0.36f, 0.95f));
//            GameObject avatarRing = CreatePanelObject("AvatarRing", avatarBox.transform, new Vector2(0.20f, 0.20f), new Vector2(0.80f, 0.85f), new Color(0.18f, 0.85f, 0.65f, 1f), UIStyleHelper.CircleSprite);
//            CreateTextObject("AvatarIcon", avatarBox.transform, Vector2.zero, Vector2.one, "[JOJO]", 18, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject speechBubble = CreatePanelObject("SpeechBubble", shopkeeperPanel.transform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.48f), new Color(0.12f, 0.09f, 0.20f, 0.98f));
//            TextMeshProUGUI speechTxt = CreateTextObject("Speech", speechBubble.transform, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.94f), "Yo Barista! Welcome to Jojo le Barjo's Apothecary!", 14, new Color(0.95f, 0.92f, 1f, 1f), TextAlignmentOptions.TopLeft, FontStyles.Normal);

//            // Right Catalog Panel
//            GameObject catalogPanel = CreatePanelObject("CatalogPanel", root.transform, new Vector2(0.32f, 0.11f), new Vector2(0.98f, 0.89f), new Color(0.14f, 0.11f, 0.22f, 0.96f));
//            CreateTextObject("CatalogTitle", catalogPanel.transform, new Vector2(0.02f, 0.94f), new Vector2(0.98f, 0.99f), "AVAILABLE REAGENT SUPPLIES • SELECT QUANTITY & BUY", 13, new Color(0.18f, 0.85f, 0.65f, 1f), TextAlignmentOptions.Left, FontStyles.Bold);

//            GameObject gridObj = new GameObject("CatalogGrid", typeof(RectTransform), typeof(GridLayoutGroup));
//            gridObj.transform.SetParent(catalogPanel.transform, false);
//            RectTransform cgRt = gridObj.GetComponent<RectTransform>();
//            cgRt.anchorMin = new Vector2(0.02f, 0.02f);
//            cgRt.anchorMax = new Vector2(0.98f, 0.93f);
//            cgRt.offsetMin = Vector2.zero;
//            cgRt.offsetMax = Vector2.zero;
//            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
//            glg.cellSize = new Vector2(210, 160);
//            glg.spacing = new Vector2(10, 10);

//            // Bottom Navigation
//            Button openMapBtn = CreateButtonObject("OpenMapBtn", root.transform, new Vector2(0.02f, 0.02f), new Vector2(0.30f, 0.09f), "< DISTRICT TRANSIT MAP", t.m_btnSecondary, Color.white);
//            Button returnWbBtn = CreateButtonObject("ReturnWorkbenchBtn", root.transform, new Vector2(0.32f, 0.02f), new Vector2(0.98f, 0.09f), "< RETURN TO LAB WORKBENCH", t.m_btnPrimary, Color.white);

//            // Wire Serialized Properties
//            SerializedObject so = new SerializedObject(screen);
//            so.FindProperty("_creditText").objectReferenceValue = creditTxt;
//            so.FindProperty("_speechText").objectReferenceValue = speechTxt;
//            so.FindProperty("_shopkeeperAvatarRing").objectReferenceValue = avatarRing;
//            so.FindProperty("_catalogContainer").objectReferenceValue = gridObj.transform;
//            so.FindProperty("_openMapBtn").objectReferenceValue = openMapBtn;
//            so.FindProperty("_returnToWorkbenchBtn").objectReferenceValue = returnWbBtn;
//            so.ApplyModifiedProperties();

//            root.SetActive(false);
//            return screen;
//        }

//        private static void BuildAndBakeStandaloneWidgets(Transform root, UIThemePalette t)
//        {
//            // 1. Widget_StatMeter
//            GameObject statMeterObj = new GameObject("Widget_StatMeter", typeof(RectTransform));
//            statMeterObj.transform.SetParent(root, false);
//            UIStatMeter statMeter = BuildStatMeterObject("Widget_StatMeter", root, "Neural Calm", t.m_accentCyan);
//            BakePrefab(statMeter.gameObject, "Assets/_/CONTENT/Prefabs/UI/Widgets/Widget_StatMeter.prefab");
//            UnityEngine.Object.DestroyImmediate(statMeter.gameObject);
//            UnityEngine.Object.DestroyImmediate(statMeterObj);

//            // 2. Widget_MoleculeNode
//            GameObject nodeObj = new GameObject("Widget_MoleculeNode", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
//            nodeObj.transform.SetParent(root, false);
//            UIMoleculeNode nodeComp = nodeObj.AddComponent<UIMoleculeNode>();
//            RectTransform nRt = nodeObj.GetComponent<RectTransform>();
//            nRt.sizeDelta = new Vector2(74, 74);
//            nodeObj.GetComponent<Image>().sprite = UIStyleHelper.CircleSprite;

//            GameObject ring = CreatePanelObject("Ring", nodeObj.transform, Vector2.zero, Vector2.one, Color.cyan, UIStyleHelper.CircleSprite);
//            ring.GetComponent<RectTransform>().sizeDelta = new Vector2(6, 6);
//            ring.transform.SetAsFirstSibling();

//            TextMeshProUGUI formulaTxt = CreateTextObject("Formula", nodeObj.transform, new Vector2(0, 0.35f), new Vector2(1, 0.85f), "MOL", 13, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
//            TextMeshProUGUI nameTxt = CreateTextObject("Name", nodeObj.transform, new Vector2(0, 0.10f), new Vector2(1, 0.40f), "Molecule", 9, Color.white, TextAlignmentOptions.Center, FontStyles.Normal);

//            SerializedObject soNode = new SerializedObject(nodeComp);
//            soNode.FindProperty("_bgImage").objectReferenceValue = nodeObj.GetComponent<Image>();
//            soNode.FindProperty("_ringImage").objectReferenceValue = ring.GetComponent<Image>();
//            soNode.FindProperty("_formulaText").objectReferenceValue = formulaTxt;
//            soNode.FindProperty("_nameText").objectReferenceValue = nameTxt;
//            soNode.FindProperty("_canvasGroup").objectReferenceValue = nodeObj.GetComponent<CanvasGroup>();
//            soNode.ApplyModifiedProperties();

//            BakePrefab(nodeObj, "Assets/_/CONTENT/Prefabs/UI/Widgets/Widget_MoleculeNode.prefab");
//            UnityEngine.Object.DestroyImmediate(nodeObj);

//            // 3. Widget_LinkRenderer
//            GameObject linkObj = new GameObject("Widget_LinkRenderer", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//            linkObj.transform.SetParent(root, false);
//            UILinkRenderer linkComp = linkObj.AddComponent<UILinkRenderer>();
//            Image linkImg = linkObj.GetComponent<Image>();
//            linkImg.sprite = UIStyleHelper.RoundedSprite;
//            linkImg.type = Image.Type.Sliced;
//            SerializedObject soLink = new SerializedObject(linkComp);
//            soLink.FindProperty("_lineImage").objectReferenceValue = linkImg;
//            soLink.ApplyModifiedProperties();

//            BakePrefab(linkObj, "Assets/_/CONTENT/Prefabs/UI/Widgets/Widget_LinkRenderer.prefab");
//            UnityEngine.Object.DestroyImmediate(linkObj);
//        }

//        private static UIStatMeter BuildStatMeterObject(string name, Transform parent, string statName, Color barColor)
//        {
//            var t = UIStyleHelper.Theme;
//            GameObject meterObj = new GameObject(name, typeof(RectTransform));
//            meterObj.transform.SetParent(parent, false);
//            UIStatMeter meter = meterObj.AddComponent<UIStatMeter>();
//            RectTransform mRt = meterObj.GetComponent<RectTransform>();
//            mRt.sizeDelta = new Vector2(240, 28);

//            GameObject header = new GameObject("Header", typeof(RectTransform));
//            header.transform.SetParent(meterObj.transform, false);
//            RectTransform hRt = header.GetComponent<RectTransform>();
//            hRt.anchorMin = new Vector2(0f, 0.5f);
//            hRt.anchorMax = new Vector2(1f, 1f);
//            hRt.offsetMin = Vector2.zero;
//            hRt.offsetMax = Vector2.zero;

//            TextMeshProUGUI lbl = CreateTextObject("Label", header.transform, new Vector2(0f, 0f), new Vector2(0.7f, 1f), statName, 12, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
//            TextMeshProUGUI val = CreateTextObject("Value", header.transform, new Vector2(0.7f, 0f), new Vector2(1f, 1f), "0", 12, barColor, TextAlignmentOptions.Right, FontStyles.Bold);

//            GameObject barBg = CreatePanelObject("BarBg", meterObj.transform, new Vector2(0f, 0f), new Vector2(1f, 0.45f), t.m_wellBg, UIStyleHelper.RoundedSprite);
//            GameObject fillObj = CreatePanelObject("Fill", barBg.transform, Vector2.zero, Vector2.one, barColor, UIStyleHelper.RoundedSprite);
//            Image fillImg = fillObj.GetComponent<Image>();
//            fillImg.type = Image.Type.Filled;
//            fillImg.fillMethod = Image.FillMethod.Horizontal;
//            fillImg.fillAmount = 0f;

//            SerializedObject so = new SerializedObject(meter);
//            so.FindProperty("_label").objectReferenceValue = lbl;
//            so.FindProperty("_valueText").objectReferenceValue = val;
//            so.FindProperty("_fillImage").objectReferenceValue = fillImg;
//            so.FindProperty("_barBgImage").objectReferenceValue = barBg.GetComponent<Image>();
//            so.ApplyModifiedProperties();

//            return meter;
//        }

//        private static GameObject CreateFullRectObject(string name, Transform parent)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform));
//            obj.transform.SetParent(parent, false);
//            RectTransform rt = obj.GetComponent<RectTransform>();
//            rt.anchorMin = Vector2.zero;
//            rt.anchorMax = Vector2.one;
//            rt.offsetMin = Vector2.zero;
//            rt.offsetMax = Vector2.zero;
//            return obj;
//        }

//        private static GameObject CreatePanelObject(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color, Sprite sprite = null)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
//            obj.transform.SetParent(parent, false);
//            RectTransform rt = obj.GetComponent<RectTransform>();
//            rt.anchorMin = anchorMin;
//            rt.anchorMax = anchorMax;
//            rt.offsetMin = Vector2.zero;
//            rt.offsetMax = Vector2.zero;

//            Image img = obj.GetComponent<Image>();
//            img.color = color;
//            if (sprite != null)
//            {
//                img.sprite = sprite;
//                img.type = Image.Type.Sliced;
//            }
//            return obj;
//        }

//        private static TextMeshProUGUI CreateTextObject(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, string text, float fontSize, Color color, TextAlignmentOptions align, FontStyles fontStyle)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
//            obj.transform.SetParent(parent, false);
//            RectTransform rt = obj.GetComponent<RectTransform>();
//            rt.anchorMin = anchorMin;
//            rt.anchorMax = anchorMax;
//            rt.offsetMin = Vector2.zero;
//            rt.offsetMax = Vector2.zero;

//            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
//            tmp.text = text;
//            tmp.fontSize = fontSize;
//            tmp.color = color;
//            tmp.alignment = align;
//            tmp.fontStyle = fontStyle;
//            tmp.raycastTarget = false;
//            return tmp;
//        }

//        private static Button CreateButtonObject(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, string text, Color bgColor, Color textColor)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
//            obj.transform.SetParent(parent, false);
//            RectTransform rt = obj.GetComponent<RectTransform>();
//            rt.anchorMin = anchorMin;
//            rt.anchorMax = anchorMax;
//            rt.offsetMin = Vector2.zero;
//            rt.offsetMax = Vector2.zero;

//            Image img = obj.GetComponent<Image>();
//            img.sprite = UIStyleHelper.RoundedSprite;
//            img.type = Image.Type.Sliced;
//            img.color = bgColor;

//            Button btn = obj.GetComponent<Button>();
//            ColorBlock cb = btn.colors;
//            cb.normalColor = bgColor;
//            cb.highlightedColor = Color.Lerp(bgColor, Color.white, 0.25f);
//            cb.pressedColor = Color.Lerp(bgColor, Color.black, 0.25f);
//            btn.colors = cb;

//            CreateTextObject("Text", obj.transform, Vector2.zero, Vector2.one, text, 12, textColor, TextAlignmentOptions.Center, FontStyles.Bold);
//            return btn;
//        }

//        private static void BakePrefab(GameObject targetObj, string assetPath)
//        {
//            try
//            {
//                PrefabUtility.SaveAsPrefabAssetAndConnect(targetObj, assetPath, InteractionMode.AutomatedAction);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogWarning($"[UIGameObjectHUDBaker] Prefab export notice for {assetPath}: {ex.Message}");
//            }
//        }

//        #endregion


//        #region Private and Protected

//        #endregion
//    }
//}

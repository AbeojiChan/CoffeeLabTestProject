#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace MolecularBrewing.Preparation.Editor
{
    public static class PreparationTabletHierarchyBaker
    {
        #region Main API

        [MenuItem("Tools/Preparation 0.2/Build 3-Chamber Machine Overlay Screen", false, 12)]
        public static void BuildMachineOverlayScreenOnly()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[PreparationTabletHierarchyBaker] No Canvas found in scene! Please open scene _Proto_Ingredients_Preparation_0.2.");
                return;
            }

            PreparationTabletScreen tabletScreen = Object.FindFirstObjectByType<PreparationTabletScreen>();
            BuildMachineOverlayScreen(canvas, tabletScreen);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green><b>[PreparationTabletHierarchyBaker]</b> Successfully built/updated 3-Chamber Machine Overlay Screen!</color>");
        }

        [MenuItem("Tools/Preparation 0.2/Build Molecular Synthesis Screen", false, 14)]
        public static void BuildMolecularSynthesisScreenOnly()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[PreparationTabletHierarchyBaker] No Canvas found in scene! Please open scene _Proto_Ingredients_Preparation_0.2.");
                return;
            }

            PreparationMachineScreen machineScreen = Object.FindFirstObjectByType<PreparationMachineScreen>();
            BuildMolecularSynthesisScreen(canvas, machineScreen);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green><b>[PreparationTabletHierarchyBaker]</b> Successfully built/updated Molecular Synthesis Screen!</color>");
        }

        public static MolecularSynthesisScreen BuildMolecularSynthesisScreen(Canvas canvas, PreparationMachineScreen machineScreen)
        {
            Transform existingSynth = canvas.transform.Find("MolecularSynthesisScreen");
            if (existingSynth != null)
            {
                Object.DestroyImmediate(existingSynth.gameObject);
            }

            // Screen Root & CanvasGroup
            GameObject synthObj = CreateUIObject("MolecularSynthesisScreen", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            synthObj.transform.SetAsLastSibling();

            CanvasGroup cg = synthObj.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // Plain black background (customizable by user)
            Image bg = synthObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 1f);

            MolecularSynthesisScreen synthCtrl = synthObj.AddComponent<MolecularSynthesisScreen>();
            synthCtrl.m_screenCanvasGroup = cg;

            // TOP BAR
            GameObject topBar = CreateUIObject("TopBar", synthObj.transform, new Vector2(0.02f, 0.93f), new Vector2(0.98f, 0.98f), Vector2.zero, Vector2.zero);
            GameObject backBtnObj = CreateButtonObject("BackButton", topBar.transform, "◀ RETOUR PRÉPARATION", new Color(0.25f, 0.32f, 0.40f, 1f));
            RectTransform backRect = backBtnObj.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0f);
            backRect.anchorMax = new Vector2(0.20f, 1f);
            synthCtrl.m_backToPrepButton = backBtnObj.GetComponent<Button>();

            CreateTextObject("SynthTitle", topBar.transform, "❖  CHAMBRE DE SYNTHÈSE MOLÉCULAIRE  ❖", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.22f, 0f), new Vector2(0.78f, 1f));

            // ====================================================
            // LEFT/CENTER: CIRCULAR REACTION CRUCIBLE (Confined Space)
            // ====================================================
            GameObject crucibleRoot = CreateUIObject("ReactionCrucibleContainer", synthObj.transform, new Vector2(0.06f, 0.08f), new Vector2(0.62f, 0.90f), Vector2.zero, Vector2.zero);
            synthCtrl.m_crucibleContainer = crucibleRoot.GetComponent<RectTransform>();
            synthCtrl.m_crucibleRadius = 340f;

            // Liquid Level Visualization Box ("on voit le liquide")
            GameObject liquidObj = CreateUIObject("LiquidLayer", crucibleRoot.transform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.85f), Vector2.zero, Vector2.zero);
            Image liquidImg = liquidObj.AddComponent<Image>();
            liquidImg.color = new Color(0.10f, 0.35f, 0.55f, 0.35f);
            synthCtrl.m_liquidLevelImage = liquidImg;

            // Circular Border / Perimeter Ring
            GameObject circleRingObj = CreateUIObject("CrucibleCircleBorder", crucibleRoot.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image circleRingImg = circleRingObj.AddComponent<Image>();
            circleRingImg.color = new Color(0.25f, 0.70f, 0.90f, 0.75f);
            circleRingImg.raycastTarget = false;
            synthCtrl.m_crucibleCircleBorder = circleRingImg;

            // ====================================================
            // RIGHT: INJECTOR APPARATUS & SYNTHESIS CONTROLS
            // ====================================================
            GameObject rightPanel = CreateUIObject("ApparatusPanel", synthObj.transform, new Vector2(0.65f, 0.08f), new Vector2(0.95f, 0.90f), Vector2.zero, Vector2.zero);
            Image rBg = rightPanel.AddComponent<Image>();
            rBg.color = new Color(0.08f, 0.11f, 0.15f, 0.90f);

            // Syringe / Injector placeholder visual
            GameObject syringeObj = CreateUIObject("InjectorVisual", rightPanel.transform, new Vector2(0.10f, 0.45f), new Vector2(0.90f, 0.92f), Vector2.zero, Vector2.zero);
            Image syringeImg = syringeObj.AddComponent<Image>();
            syringeImg.color = new Color(0.20f, 0.28f, 0.38f, 0.90f);
            synthCtrl.m_injectorApparatusImage = syringeImg;

            CreateTextObject("SyringeLabel", syringeObj.transform, "INJECTEUR & STABILISATEUR\n[ APPAREIL DE SYNTHÈSE ]", 14, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

            // Status prompt
            GameObject promptObj = CreateTextObject("StatusPrompt", rightPanel.transform, "Cliquez ou glissez entre 2 molécules pour créer des liaisons.", 14, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.32f), new Vector2(0.95f, 0.42f));
            synthCtrl.m_statusPromptText = promptObj.GetComponent<Text>();

            // Bond counter
            GameObject countObj = CreateTextObject("BondCounter", rightPanel.transform, "Liaisons: 0 / 0", 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.22f), new Vector2(0.95f, 0.30f));
            synthCtrl.m_bondCounterText = countObj.GetComponent<Text>();

            // Buttons
            GameObject clearBtnObj = CreateButtonObject("ClearBondsButton", rightPanel.transform, "EFFACER LES LIAISONS ✖", new Color(0.70f, 0.25f, 0.25f, 1f));
            RectTransform clearRect = clearBtnObj.GetComponent<RectTransform>();
            clearRect.anchorMin = new Vector2(0.05f, 0.12f);
            clearRect.anchorMax = new Vector2(0.95f, 0.20f);
            synthCtrl.m_clearBondsButton = clearBtnObj.GetComponent<Button>();

            GameObject synthBtnObj = CreateButtonObject("SynthesizeButton", rightPanel.transform, "SYNTHÉTISER LA BOISSON ➔", new Color(0.18f, 0.78f, 0.45f, 1f));
            RectTransform synthRect = synthBtnObj.GetComponent<RectTransform>();
            synthRect.anchorMin = new Vector2(0.05f, 0.02f);
            synthRect.anchorMax = new Vector2(0.95f, 0.10f);
            synthCtrl.m_synthesizeButton = synthBtnObj.GetComponent<Button>();

            // ====================================================
            // FAR RIGHT: RETRACTABLE RECIPES CODEX PULL TAB
            // ====================================================
            GameObject codexTabBtnObj = CreateButtonObject("PullTab_Codex", synthObj.transform, "RECIPES", new Color(0.30f, 0.45f, 0.60f, 1f));
            RectTransform codexTabRect = codexTabBtnObj.GetComponent<RectTransform>();
            codexTabRect.anchorMin = new Vector2(0.965f, 0.85f);
            codexTabRect.anchorMax = new Vector2(1.0f, 0.92f);
            codexTabRect.anchoredPosition = Vector2.zero;

            if (machineScreen != null)
            {
                machineScreen.m_synthesisScreen = synthCtrl;
                EditorUtility.SetDirty(machineScreen);
            }

            EditorUtility.SetDirty(synthObj);
            return synthCtrl;
        }

        public static PreparationMachineScreen BuildMachineOverlayScreen(Canvas canvas, PreparationTabletScreen tabletScreen)
        {
            Transform existingMachine = canvas.transform.Find("PreparationMachineScreen");
            if (existingMachine != null)
            {
                Object.DestroyImmediate(existingMachine.gameObject);
            }

            // Screen Root & CanvasGroup
            GameObject machineObj = CreateUIObject("PreparationMachineScreen", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            machineObj.transform.SetAsLastSibling();

            CanvasGroup cg = machineObj.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            Image bg = machineObj.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.07f, 0.09f, 0.96f);

            PreparationMachineScreen machineCtrl = machineObj.AddComponent<PreparationMachineScreen>();
            machineCtrl.m_screenCanvasGroup = cg;

            // TOP BAR: Title & Back Button
            GameObject topBar = CreateUIObject("TopBar", machineObj.transform, new Vector2(0.02f, 0.93f), new Vector2(0.98f, 0.98f), Vector2.zero, Vector2.zero);
            GameObject backBtnObj = CreateButtonObject("BackButton", topBar.transform, "◀ RETOUR TABLETTE", new Color(0.25f, 0.32f, 0.40f, 1f));
            RectTransform backRect = backBtnObj.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0f);
            backRect.anchorMax = new Vector2(0.18f, 1f);
            machineCtrl.m_backToTabletButton = backBtnObj.GetComponent<Button>();

            CreateTextObject("MachineTitle", topBar.transform, "❖  APPAREIL DE PRÉPARATION // 3 ENTRÉES  ❖", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.20f, 0f), new Vector2(0.80f, 1f));

            // Prompt Text
            GameObject promptObj = CreateTextObject("InstructionPrompt", machineObj.transform, "Placez les ingrédients dans les entrées correspondantes puis maintenez le bouton.", 16, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.92f));
            machineCtrl.m_instructionPromptText = promptObj.GetComponent<Text>();

            // ====================================================
            // MIDDLE: 3 MACHINE CHAMBERS (GRINDER, PLANTS, LIQUIDS)
            // ====================================================
            GameObject chambersArea = CreateUIObject("ChambersArea", machineObj.transform, new Vector2(0.04f, 0.40f), new Vector2(0.96f, 0.86f), Vector2.zero, Vector2.zero);
            HorizontalLayoutGroup chLayout = chambersArea.AddComponent<HorizontalLayoutGroup>();
            chLayout.spacing = 24;
            chLayout.childControlWidth = true;
            chLayout.childControlHeight = true;
            chLayout.childForceExpandWidth = true;

            // 1. Chamber Grinder (Beans & Solids)
            var chamberGrinder = CreateChamberUI(chambersArea.transform, "Chamber_Grinder", MachineChamberType.Chamber1_Grinder, "BROYEUR À GRAINS", "Grains & Solides (Café, Poussière Lunaire)", new Color(0.75f, 0.45f, 0.18f, 1f));
            machineCtrl.m_chamberGrinder = chamberGrinder;

            // 2. Chamber Plants (Botanics)
            var chamberPlants = CreateChamberUI(chambersArea.transform, "Chamber_Plants", MachineChamberType.Chamber2_HerbProcessor, "EXTRACTEUR BOTANIQUE", "Feuilles & Végétaux (Thé, Menthe, Piments)", new Color(0.22f, 0.68f, 0.38f, 1f));
            machineCtrl.m_chamberPlants = chamberPlants;

            // 3. Chamber Liquids (Solvents)
            var chamberLiquids = CreateChamberUI(chambersArea.transform, "Chamber_Liquids", MachineChamberType.Chamber3_LiquidDispenser, "DISPENSATEUR FLUIDE", "Liquides & Solvants (Eau, Lait, Nickel, Lave)", new Color(0.20f, 0.55f, 0.85f, 1f));
            machineCtrl.m_chamberLiquids = chamberLiquids;

            // ====================================================
            // LOWER-MID: STAGING DECK (Incoming Ingredients)
            // ====================================================
            GameObject deckSection = CreateUIObject("StagingDeckSection", machineObj.transform, new Vector2(0.04f, 0.20f), new Vector2(0.60f, 0.38f), Vector2.zero, Vector2.zero);
            Image deckBg = deckSection.AddComponent<Image>();
            deckBg.color = new Color(0.10f, 0.13f, 0.17f, 0.90f);

            CreateTextObject("DeckHeader", deckSection.transform, "PLATEAU D'ARRIVÉE", 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Vector2(0.03f, 0.78f), new Vector2(0.50f, 0.98f));

            GameObject autoDistBtnObj = CreateButtonObject("AutoDistributeButton", deckSection.transform, "DISTRIBUER TOUT ➔", new Color(0.25f, 0.40f, 0.55f, 1f));
            RectTransform distRect = autoDistBtnObj.GetComponent<RectTransform>();
            distRect.anchorMin = new Vector2(0.60f, 0.76f);
            distRect.anchorMax = new Vector2(0.97f, 0.96f);
            machineCtrl.m_autoDistributeButton = autoDistBtnObj.GetComponent<Button>();

            GameObject deckGridObj = CreateUIObject("DeckGrid", deckSection.transform, new Vector2(0.02f, 0.05f), new Vector2(0.98f, 0.72f), Vector2.zero, Vector2.zero);
            GridLayoutGroup deckGrid = deckGridObj.AddComponent<GridLayoutGroup>();
            deckGrid.cellSize = new Vector2(90, 80);
            deckGrid.spacing = new Vector2(10, 10);
            deckGrid.childAlignment = TextAnchor.MiddleLeft;
            machineCtrl.m_stagingDeckContainer = deckGridObj.transform;

            // ====================================================
            // RIGHT: OUTPUT BEAKER & PROCEED
            // ====================================================
            GameObject beakerSection = CreateUIObject("OutputBeakerSection", machineObj.transform, new Vector2(0.62f, 0.20f), new Vector2(0.96f, 0.38f), Vector2.zero, Vector2.zero);
            Image beakerBg = beakerSection.AddComponent<Image>();
            beakerBg.color = new Color(0.08f, 0.11f, 0.15f, 0.95f);

            GameObject beakerSummary = CreateTextObject("BeakerSummaryText", beakerSection.transform, "BÉCHER D'EXTRACTION\n<i>En attente du processus...</i>", 15, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.92f));
            machineCtrl.m_beakerSummaryText = beakerSummary.GetComponent<Text>();
            machineCtrl.m_beakerVisualObject = beakerSummary;

            // ====================================================
            // BOTTOM CONTROLS: HOLD TO PROCESS & PROCEED SYNTHESIS
            // ====================================================
            GameObject bottomControls = CreateUIObject("BottomControls", machineObj.transform, new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.16f), Vector2.zero, Vector2.zero);
            HorizontalLayoutGroup botLayout = bottomControls.AddComponent<HorizontalLayoutGroup>();
            botLayout.spacing = 20;
            botLayout.childControlWidth = true;
            botLayout.childControlHeight = true;
            botLayout.childForceExpandWidth = true;

            // Hold Process Button
            GameObject holdBtnObj = CreateUIObject("HoldProcessButton", bottomControls.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image holdBgImg = holdBtnObj.AddComponent<Image>();
            holdBgImg.color = new Color(0.12f, 0.16f, 0.20f, 1f);

            GameObject fillImgObj = CreateUIObject("ProgressFill", holdBtnObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image fillImg = fillImgObj.AddComponent<Image>();
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 0f;
            fillImg.color = new Color(0.20f, 0.70f, 0.40f, 1f);

            GameObject holdLabel = CreateTextObject("HoldLabel", holdBtnObj.transform, "HOLD TO PROCESS", 20, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

            HoldToProcessButton holdComp = holdBtnObj.AddComponent<HoldToProcessButton>();
            holdComp.m_fillProgressImage = fillImg;
            holdComp.m_statusText = holdLabel.GetComponent<Text>();
            holdComp.m_holdDuration = 2.0f;
            machineCtrl.m_holdProcessButton = holdComp;

            // Proceed to Synthesis Button
            GameObject proceedSynthObj = CreateButtonObject("ProceedSynthesisButton", bottomControls.transform, "VERS LA SYNTHÈSE ➔", new Color(0.20f, 0.75f, 0.40f, 1f));
            machineCtrl.m_proceedToSynthesisButton = proceedSynthObj.GetComponent<Button>();
            machineCtrl.m_proceedToSynthesisButton.interactable = false;

            if (tabletScreen != null)
            {
                tabletScreen.m_machineScreen = machineCtrl;
                EditorUtility.SetDirty(tabletScreen);
            }

            EditorUtility.SetDirty(machineObj);
            return machineCtrl;
        }

        private static PreparationMachineChamber CreateChamberUI(Transform parent, string name, MachineChamberType type, string title, string hint, Color accent)
        {
            GameObject obj = CreateUIObject(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.09f, 0.12f, 0.16f, 0.95f);

            // Header Banner
            GameObject header = CreateUIObject("ChamberHeader", obj.transform, new Vector2(0f, 0.85f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            Image hImg = header.AddComponent<Image>();
            hImg.color = accent * 0.4f;

            GameObject tObj = CreateTextObject("Title", header.transform, title, 16, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            Text t = tObj.GetComponent<Text>();
            t.color = accent;

            // Hint
            CreateTextObject("Hint", obj.transform, hint, 12, FontStyle.Italic, TextAnchor.MiddleCenter, new Vector2(0.04f, 0.75f), new Vector2(0.96f, 0.84f));

            // Indicator / Blade graphic
            GameObject indObj = CreateUIObject("ProcessingIndicator", obj.transform, new Vector2(0.05f, 0.68f), new Vector2(0.95f, 0.74f), Vector2.zero, Vector2.zero);
            Image indImg = indObj.AddComponent<Image>();
            indImg.color = accent;
            indObj.SetActive(false);

            // Items Grid
            GameObject itemsGridObj = CreateUIObject("ItemsGrid", obj.transform, new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.66f), Vector2.zero, Vector2.zero);
            GridLayoutGroup grid = itemsGridObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(90, 85);
            grid.spacing = new Vector2(10, 10);
            grid.childAlignment = TextAnchor.UpperCenter;

            // Count label
            GameObject countObj = CreateTextObject("CountLabel", obj.transform, "Chargé: 0", 14, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0f, 0.01f), new Vector2(1f, 0.07f));

            PreparationMachineChamber chamber = obj.AddComponent<PreparationMachineChamber>();
            chamber.m_chamberType = type;
            chamber.m_chamberTitle = title;
            chamber.m_acceptedCategoryHint = hint;
            chamber.m_themeAccent = accent;
            chamber.m_chamberTitleText = t;
            chamber.m_countText = countObj.GetComponent<Text>();
            chamber.m_loadedItemsContainer = itemsGridObj.transform;
            chamber.m_processingIndicator = indImg;
            chamber.m_chamberBackground = bg;

            return chamber;
        }

        #endregion


        #region Tools and Utilities

        private static GameObject CreateUIObject(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return obj;
        }

        private static GameObject CreateTextObject(string name, Transform parent, string text, int fontSize, FontStyle style, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject obj = CreateUIObject(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            Text t = obj.AddComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.alignment = align;
            t.color = Color.white;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            return obj;
        }

        private static GameObject CreateButtonObject(string name, Transform parent, string label, Color bgColor)
        {
            GameObject obj = CreateUIObject(name, parent, new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            Image img = obj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = obj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.highlightedColor = bgColor * 1.15f;
            cb.pressedColor = bgColor * 0.85f;
            btn.colors = cb;

            CreateTextObject("Label", obj.transform, label, 16, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            return obj;
        }

        private static List<RecipeData> LoadAllRecipeAssets()
        {
            List<RecipeData> list = new List<RecipeData>();
            string[] guids = AssetDatabase.FindAssets("t:RecipeData", new[] { "Assets/_/DATABASE/Recipes" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                RecipeData asset = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
                if (asset != null) list.Add(asset);
            }
            return list;
        }

        private static List<AdditiveEffectData> LoadAllAdditiveEffectAssets()
        {
            List<AdditiveEffectData> list = new List<AdditiveEffectData>();
            string[] guids = AssetDatabase.FindAssets("t:AdditiveEffectData", new[] { "Assets/_/DATABASE/AdditiveEffects" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AdditiveEffectData asset = AssetDatabase.LoadAssetAtPath<AdditiveEffectData>(path);
                if (asset != null) list.Add(asset);
            }
            return list;
        }

        #endregion
    }
}
#endif

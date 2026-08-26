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

        [MenuItem("Tools/Preparation 0.2/Build Preparation Tablet Hierarchy in Scene", false, 10)]
        public static void BuildHierarchyInCurrentScene()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("PreparationTabletCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            EventSystem es = Object.FindFirstObjectByType<EventSystem>();
            if (es == null)
            {
                GameObject esObj = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
                esObj.AddComponent<InputSystemUIInputModule>();
#else
                esObj.AddComponent<StandaloneInputModule>();
#endif
            }

            List<RawIngredientItemData> ingredients = EnsureDefaultIngredientAssets();

            Transform existingScreen = canvas.transform.Find("PreparationTabletScreen");
            if (existingScreen != null)
            {
                Object.DestroyImmediate(existingScreen.gameObject);
            }

            GameObject screenObj = CreateUIObject("PreparationTabletScreen", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            PreparationTabletScreen tabletScreen = screenObj.AddComponent<PreparationTabletScreen>();

            Image bgImg = screenObj.AddComponent<Image>();
            bgImg.color = new Color(0.07f, 0.09f, 0.12f, 1f);
            tabletScreen.m_tabletBackground = bgImg;

            Sprite wallpaper = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_/CONTENT/External/Sprite(s)/neo_fremio_midnight_blue.png");
            if (wallpaper != null)
            {
                bgImg.sprite = wallpaper;
                bgImg.type = Image.Type.Simple;
            }

            // CENTER: Tray Panel
            GameObject trayPanelObj = CreateUIObject("Center_TrayPanel", screenObj.transform, new Vector2(0.35f, 0.08f), new Vector2(0.65f, 0.92f), Vector2.zero, Vector2.zero);
            Image trayBg = trayPanelObj.AddComponent<Image>();
            trayBg.color = new Color(0.12f, 0.15f, 0.20f, 0.90f);

            GameObject trayHeader = CreateTextObject("TrayHeader", trayPanelObj.transform, "PLATEAU D'INGRÉDIENTS", 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0f, 0.90f), new Vector2(1f, 1f));

            GameObject trayContainerObj = CreateUIObject("TrayItemsContainer", trayPanelObj.transform, new Vector2(0.05f, 0.20f), new Vector2(0.95f, 0.88f), Vector2.zero, Vector2.zero);
            GridLayoutGroup trayGrid = trayContainerObj.AddComponent<GridLayoutGroup>();
            trayGrid.cellSize = new Vector2(140, 140);
            trayGrid.spacing = new Vector2(16, 16);
            trayGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            trayGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            trayGrid.childAlignment = TextAnchor.MiddleCenter;

            GameObject capLabel = CreateTextObject("CapacityLabel", trayPanelObj.transform, "0 / 7", 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0f, 0.12f), new Vector2(1f, 0.18f));

            GameObject btnContainer = CreateUIObject("BottomButtons", trayPanelObj.transform, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.10f), Vector2.zero, Vector2.zero);
            HorizontalLayoutGroup btnLayout = btnContainer.AddComponent<HorizontalLayoutGroup>();
            btnLayout.spacing = 20;
            btnLayout.childControlWidth = true;
            btnLayout.childControlHeight = true;
            btnLayout.childForceExpandWidth = true;

            GameObject resetBtnObj = CreateButtonObject("ResetButton", btnContainer.transform, "RESET", new Color(0.80f, 0.25f, 0.25f, 1f));
            GameObject proceedBtnObj = CreateButtonObject("ProceedButton", btnContainer.transform, "PROCEED >", new Color(0.20f, 0.75f, 0.40f, 1f));

            PreparationTrayController trayCtrl = trayPanelObj.AddComponent<PreparationTrayController>();
            trayCtrl.m_maxCapacity = 7;
            trayCtrl.m_itemsContainer = trayContainerObj.transform;
            trayCtrl.m_resetButton = resetBtnObj.GetComponent<Button>();
            trayCtrl.m_proceedButton = proceedBtnObj.GetComponent<Button>();
            trayCtrl.m_capacityLabel = capLabel.GetComponent<Text>();
            tabletScreen.m_trayController = trayCtrl;

            // LEFT: Ingredients Panel
            GameObject leftPanelObj = CreateUIObject("Left_IngredientsPanel", screenObj.transform, new Vector2(0f, 0.05f), new Vector2(0.32f, 0.95f), Vector2.zero, Vector2.zero);
            Image leftBg = leftPanelObj.AddComponent<Image>();
            leftBg.color = new Color(0.10f, 0.13f, 0.17f, 0.95f);

            GameObject leftTabBtnObj = CreateButtonObject("PullTab_Left", leftPanelObj.transform, "< >", new Color(0.25f, 0.35f, 0.45f, 1f));
            RectTransform leftTabRect = leftTabBtnObj.GetComponent<RectTransform>();
            leftTabRect.anchorMin = new Vector2(1f, 0.5f);
            leftTabRect.anchorMax = new Vector2(1f, 0.5f);
            leftTabRect.pivot = new Vector2(0f, 0.5f);
            leftTabRect.anchoredPosition = new Vector2(4, 0);
            leftTabRect.sizeDelta = new Vector2(40, 90);

            RetractableSidePanel leftDrawer = leftPanelObj.AddComponent<RetractableSidePanel>();
            leftDrawer.m_direction = RetractDirection.SlideLeft;
            leftDrawer.m_retractDistance = 600f;
            leftDrawer.m_startOpened = false;
            leftDrawer.m_pullTabButton = leftTabBtnObj.GetComponent<Button>();
            tabletScreen.m_leftDrawer = leftDrawer;

            CreateTextObject("PantryHeader", leftPanelObj.transform, "INGRÉDIENTS", 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.92f), new Vector2(0.95f, 0.98f));

            // Tabs for All, Bases, Additives
            GameObject tabsContainer = CreateUIObject("TabsContainer", leftPanelObj.transform, new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.91f), Vector2.zero, Vector2.zero);
            HorizontalLayoutGroup tabLayout = tabsContainer.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 6;
            tabLayout.childControlWidth = true;
            tabLayout.childControlHeight = true;
            tabLayout.childForceExpandWidth = true;

            GameObject tabAll = CreateButtonObject("Tab_All", tabsContainer.transform, "All (10)", new Color(0.25f, 0.32f, 0.42f, 1f));
            GameObject tabBases = CreateButtonObject("Tab_Bases", tabsContainer.transform, "Bases (6)", new Color(0.22f, 0.28f, 0.35f, 1f));
            GameObject tabAdditives = CreateButtonObject("Tab_Additives", tabsContainer.transform, "Additives (4)", new Color(0.22f, 0.28f, 0.35f, 1f));

            GameObject pantryGridObj = CreateUIObject("IngredientsGrid", leftPanelObj.transform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.82f), Vector2.zero, Vector2.zero);
            GridLayoutGroup pantryGrid = pantryGridObj.AddComponent<GridLayoutGroup>();
            pantryGrid.cellSize = new Vector2(240, 160);
            pantryGrid.spacing = new Vector2(16, 16);
            pantryGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            pantryGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            pantryGrid.childAlignment = TextAnchor.UpperCenter;

            IngredientPantryPanel pantryCtrl = leftPanelObj.AddComponent<IngredientPantryPanel>();
            pantryCtrl.m_trayController = trayCtrl;
            pantryCtrl.m_cardsGridContainer = pantryGridObj.transform;
            pantryCtrl.m_tabAllButton = tabAll.GetComponent<Button>();
            pantryCtrl.m_tabBasesButton = tabBases.GetComponent<Button>();
            pantryCtrl.m_tabAdditivesButton = tabAdditives.GetComponent<Button>();
            pantryCtrl.m_availableIngredients = ingredients;
            tabletScreen.m_pantryPanel = pantryCtrl;

            // RIGHT: Recipes Codex Panel
            GameObject rightPanelObj = CreateUIObject("Right_RecipesPanel", screenObj.transform, new Vector2(0.68f, 0.05f), new Vector2(1.0f, 0.95f), Vector2.zero, Vector2.zero);
            Image rightBg = rightPanelObj.AddComponent<Image>();
            rightBg.color = new Color(0.10f, 0.13f, 0.17f, 0.95f);

            GameObject rightTabBtnObj = CreateButtonObject("PullTab_Right", rightPanelObj.transform, "> <", new Color(0.25f, 0.35f, 0.45f, 1f));
            RectTransform rightTabRect = rightTabBtnObj.GetComponent<RectTransform>();
            rightTabRect.anchorMin = new Vector2(0f, 0.5f);
            rightTabRect.anchorMax = new Vector2(0f, 0.5f);
            rightTabRect.pivot = new Vector2(1f, 0.5f);
            rightTabRect.anchoredPosition = new Vector2(-4, 0);
            rightTabRect.sizeDelta = new Vector2(40, 90);

            RetractableSidePanel rightDrawer = rightPanelObj.AddComponent<RetractableSidePanel>();
            rightDrawer.m_direction = RetractDirection.SlideRight;
            rightDrawer.m_retractDistance = 600f;
            rightDrawer.m_startOpened = false;
            rightDrawer.m_pullTabButton = rightTabBtnObj.GetComponent<Button>();
            tabletScreen.m_rightDrawer = rightDrawer;

            CreateTextObject("RecipesHeader", rightPanelObj.transform, "BLUEPRINTS & CODEX RECETTES", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.02f, 0.94f), new Vector2(0.98f, 0.99f));

            // --- UPPER SECTION: 3 Columns for Base Drinks ---
            CreateTextObject("BaseDrinksTitle", rightPanelObj.transform, "― RECETTES DE BASE ―", 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.02f, 0.90f), new Vector2(0.98f, 0.94f));

            // Column 1: Neo Fremio
            GameObject colNeo = CreateUIObject("Column_Neo", rightPanelObj.transform, new Vector2(0.02f, 0.54f), new Vector2(0.33f, 0.90f), Vector2.zero, Vector2.zero);
            CreateTextObject("Header_Neo", colNeo.transform, "Neo Fremio", 14, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0f, 0.88f), new Vector2(1f, 1f));
            GameObject gridNeo = CreateUIObject("Grid_Neo", colNeo.transform, new Vector2(0f, 0f), new Vector2(1f, 0.86f), Vector2.zero, Vector2.zero);
            VerticalLayoutGroup vNeo = gridNeo.AddComponent<VerticalLayoutGroup>();
            vNeo.spacing = 8;
            vNeo.childControlWidth = true;
            vNeo.childControlHeight = false;

            // Column 2: Mix
            GameObject colMix = CreateUIObject("Column_Mix", rightPanelObj.transform, new Vector2(0.35f, 0.54f), new Vector2(0.65f, 0.90f), Vector2.zero, Vector2.zero);
            CreateTextObject("Header_Mix", colMix.transform, "Mix (Tous)", 14, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0f, 0.88f), new Vector2(1f, 1f));
            GameObject gridMix = CreateUIObject("Grid_Mix", colMix.transform, new Vector2(0f, 0f), new Vector2(1f, 0.86f), Vector2.zero, Vector2.zero);
            VerticalLayoutGroup vMix = gridMix.AddComponent<VerticalLayoutGroup>();
            vMix.spacing = 8;
            vMix.childControlWidth = true;
            vMix.childControlHeight = false;

            // Column 3: Heartopia
            GameObject colHeart = CreateUIObject("Column_Heart", rightPanelObj.transform, new Vector2(0.67f, 0.54f), new Vector2(0.98f, 0.90f), Vector2.zero, Vector2.zero);
            CreateTextObject("Header_Heart", colHeart.transform, "Heartopia", 14, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0f, 0.88f), new Vector2(1f, 1f));
            GameObject gridHeart = CreateUIObject("Grid_Heart", colHeart.transform, new Vector2(0f, 0f), new Vector2(1f, 0.86f), Vector2.zero, Vector2.zero);
            VerticalLayoutGroup vHeart = gridHeart.AddComponent<VerticalLayoutGroup>();
            vHeart.spacing = 8;
            vHeart.childControlWidth = true;
            vHeart.childControlHeight = false;

            // --- LOWER SECTION: Additives Synergy Effects ---
            CreateTextObject("AdditivesTitle", rightPanelObj.transform, "― EFFETS & SYNERGIES ADDITIFS ―", 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.02f, 0.48f), new Vector2(0.98f, 0.52f));

            GameObject addGridObj = CreateUIObject("AdditivesGrid", rightPanelObj.transform, new Vector2(0.02f, 0.24f), new Vector2(0.98f, 0.47f), Vector2.zero, Vector2.zero);
            GridLayoutGroup addGrid = addGridObj.AddComponent<GridLayoutGroup>();
            addGrid.cellSize = new Vector2(150, 60);
            addGrid.spacing = new Vector2(8, 8);
            addGrid.childAlignment = TextAnchor.UpperCenter;

            // --- BOTTOM: Info / Formulation Box ---
            GameObject infoBox = CreateUIObject("RecipeInfoBox", rightPanelObj.transform, new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.22f), Vector2.zero, Vector2.zero);
            Image infoBg = infoBox.AddComponent<Image>();
            infoBg.color = new Color(0.06f, 0.08f, 0.10f, 0.90f);

            GameObject infoText = CreateTextObject("InfoText", infoBox.transform, "Sélectionnez une recette de base ou un effet additif pour consulter la formule.", 15, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.95f));

            RecipeDataGeneratorWindow.GenerateAllDemoData();
            List<RecipeData> recipeAssets = LoadAllRecipeAssets();
            List<AdditiveEffectData> effectAssets = LoadAllAdditiveEffectAssets();

            RecipeCodexPanel codexCtrl = rightPanelObj.AddComponent<RecipeCodexPanel>();
            codexCtrl.m_columnNeoContainer = gridNeo.transform;
            codexCtrl.m_columnMixContainer = gridMix.transform;
            codexCtrl.m_columnHeartContainer = gridHeart.transform;
            codexCtrl.m_additivesGridContainer = addGridObj.transform;
            codexCtrl.m_selectedRecipeInfoText = infoText.GetComponent<Text>();
            codexCtrl.m_recipes = recipeAssets;
            codexCtrl.m_additiveEffects = effectAssets;
            tabletScreen.m_recipeCodexPanel = codexCtrl;

            // ==========================================
            // GOLDEN FRAME & BEZEL OVERLAY
            // ==========================================
            CreateGoldenFrame(screenObj.transform, tabletScreen);

            EditorUtility.SetDirty(screenObj);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("<color=green><b>[PreparationTabletHierarchyBaker]</b> Successfully created full Preparation 0.2 Tablet Hierarchy with 3 Columns & Additive Section!</color>");
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

            CreateTextObject("Label", obj.transform, label, 18, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            return obj;
        }

        private static void CreateGoldenFrame(Transform parent, PreparationTabletScreen tabletScreen)
        {
            GameObject frameRoot = CreateUIObject("GoldenTabletFrame", parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            frameRoot.transform.SetAsLastSibling();

            Color goldPrimary = new Color(0.85f, 0.68f, 0.22f, 1f); // #D9AD38 Metallic Gold
            Color goldAccent = new Color(1f, 0.84f, 0.35f, 1f);  // #FFD759 Bright Gold
            Color goldDark = new Color(0.45f, 0.32f, 0.08f, 0.90f);  // Dark Brass Shadow

            // 1. Top Bar / Header Bezel
            GameObject topBar = CreateUIObject("Bezel_Top", frameRoot.transform, new Vector2(0f, 0.965f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            Image topImg = topBar.AddComponent<Image>();
            topImg.color = goldDark;
            topImg.raycastTarget = false;

            GameObject topRim = CreateUIObject("TopRimHighlight", topBar.transform, new Vector2(0f, 0f), new Vector2(1f, 0.15f), Vector2.zero, Vector2.zero);
            Image rimImg = topRim.AddComponent<Image>();
            rimImg.color = goldPrimary;
            rimImg.raycastTarget = false;

            GameObject titleTxt = CreateTextObject("TabletTitle", topBar.transform, "❖  COFFEE LAB // TABLETTE CYBER-CHIMIE v0.2  ❖", 14, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            Text tComp = titleTxt.GetComponent<Text>();
            tComp.color = goldAccent;
            tComp.raycastTarget = false;

            // 2. Bottom Bar
            GameObject bottomBar = CreateUIObject("Bezel_Bottom", frameRoot.transform, new Vector2(0f, 0f), new Vector2(1f, 0.025f), Vector2.zero, Vector2.zero);
            Image btmImg = bottomBar.AddComponent<Image>();
            btmImg.color = goldDark;
            btmImg.raycastTarget = false;

            GameObject btmRim = CreateUIObject("BottomRimHighlight", bottomBar.transform, new Vector2(0f, 0.85f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            Image btmRimImg = btmRim.AddComponent<Image>();
            btmRimImg.color = goldPrimary;
            btmRimImg.raycastTarget = false;

            // 3. Left Border
            GameObject leftBar = CreateUIObject("Bezel_Left", frameRoot.transform, new Vector2(0f, 0f), new Vector2(0.008f, 1f), Vector2.zero, Vector2.zero);
            Image leftImg = leftBar.AddComponent<Image>();
            leftImg.color = goldPrimary;
            leftImg.raycastTarget = false;

            // 4. Right Border
            GameObject rightBar = CreateUIObject("Bezel_Right", frameRoot.transform, new Vector2(0.992f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            Image rightImg = rightBar.AddComponent<Image>();
            rightImg.color = goldPrimary;
            rightImg.raycastTarget = false;

            // 5. Corner Brackets / Accents
            CreateCornerBracket("Corner_TopLeft", frameRoot.transform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0, -18), new Vector2(36, 36), goldAccent);
            CreateCornerBracket("Corner_TopRight", frameRoot.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-36, -18), new Vector2(36, 36), goldAccent);
            CreateCornerBracket("Corner_BottomLeft", frameRoot.transform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0, 0), new Vector2(36, 36), goldAccent);
            CreateCornerBracket("Corner_BottomRight", frameRoot.transform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-36, 0), new Vector2(36, 36), goldAccent);

            if (tabletScreen != null)
            {
                tabletScreen.m_tabletBezel = topImg;
            }
        }

        private static void CreateCornerBracket(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
        {
            GameObject corner = CreateUIObject(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            RectTransform rt = corner.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            Image img = corner.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
        }

        private static List<RawIngredientItemData> EnsureDefaultIngredientAssets()
        {
            string folder = "Assets/_/DATABASE/RawIngredients";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            List<RawIngredientItemData> list = new List<RawIngredientItemData>();

            // --- 6 Base Ingredients ---
            CreateOrLoadIngredient(folder, "Ing_TeaLeaves", "Tea Leaves", RawIngredientType.Base, CityOrigin.Neo, MachineChamberType.Chamber2_HerbProcessor, new Color(0.30f, 0.70f, 0.35f, 1f), new List<string> { "L-Theanine", "Chlorogenic Acid", "BioFlavonoid" }, list);
            CreateOrLoadIngredient(folder, "Ing_MintLeaves", "Mint Leaves", RawIngredientType.Base, CityOrigin.Heart, MachineChamberType.Chamber2_HerbProcessor, new Color(0.20f, 0.85f, 0.65f, 1f), new List<string> { "Aroma", "Menthol", "BioFlavonoid" }, list);
            CreateOrLoadIngredient(folder, "Ing_Water", "Water", RawIngredientType.Base, CityOrigin.Mix, MachineChamberType.Chamber3_LiquidDispenser, new Color(0.25f, 0.60f, 0.90f, 1f), new List<string> { "Water", "Water", "Water" }, list);
            CreateOrLoadIngredient(folder, "Ing_Milk", "Milk", RawIngredientType.Base, CityOrigin.Heart, MachineChamberType.Chamber3_LiquidDispenser, new Color(0.95f, 0.95f, 0.90f, 1f), new List<string> { "Lipid", "Sucrose", "Emulsion" }, list);
            CreateOrLoadIngredient(folder, "Ing_CoffeeBeans", "Coffee Beans", RawIngredientType.Base, CityOrigin.Neo, MachineChamberType.Chamber1_Grinder, new Color(0.65f, 0.40f, 0.20f, 1f), new List<string> { "Caffeine", "Caffeine", "Chlorogenic Acid" }, list);
            CreateOrLoadIngredient(folder, "Ing_LiquidNickel", "Liquid Nickel", RawIngredientType.Base, CityOrigin.Neo, MachineChamberType.Chamber3_LiquidDispenser, new Color(0.75f, 0.80f, 0.85f, 1f), new List<string> { "Catalyst", "NeuralOverclock", "MineralSalt" }, list);

            // --- 4 Additive Ingredients ---
            CreateOrLoadIngredient(folder, "Ing_LunarDust", "Lunar Dust", RawIngredientType.Additive, CityOrigin.Neo, MachineChamberType.Chamber1_Grinder, new Color(0.85f, 0.85f, 0.95f, 1f), new List<string> { "Somnin", "Somnin", "GlitchEndorphin" }, list);
            CreateOrLoadIngredient(folder, "Ing_Dragontears", "Dragontears", RawIngredientType.Additive, CityOrigin.Heart, MachineChamberType.Chamber3_LiquidDispenser, new Color(0.90f, 0.40f, 0.60f, 1f), new List<string> { "HyperGinsenoside", "NeuroLotus", "Euphoria" }, list);
            CreateOrLoadIngredient(folder, "Ing_BluePeppers", "BluePeppers", RawIngredientType.Additive, CityOrigin.Heart, MachineChamberType.Chamber2_HerbProcessor, new Color(0.35f, 0.50f, 0.95f, 1f), new List<string> { "Cinnamaldehyde", "Sharpness", "ThermalTannin" }, list);
            CreateOrLoadIngredient(folder, "Ing_IcedLava", "Iced-Lava", RawIngredientType.Additive, CityOrigin.Mix, MachineChamberType.Chamber3_LiquidDispenser, new Color(0.95f, 0.55f, 0.25f, 1f), new List<string> { "ThermalCatalyst", "CryoCompound", "NanoScavenger" }, list);

            AssetDatabase.SaveAssets();
            return list;
        }

        private static void CreateOrLoadIngredient(string folder, string filename, string displayName, RawIngredientType type, CityOrigin origin, MachineChamberType chamber, Color color, List<string> molecules, List<RawIngredientItemData> list)
        {
            string path = $"{folder}/{filename}.asset";
            RawIngredientItemData asset = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<RawIngredientItemData>();
                asset.m_id = filename.ToLowerInvariant();
                asset.m_displayName = displayName;
                asset.m_ingredientType = type;
                asset.m_origin = origin;
                asset.m_targetChamber = chamber;
                asset.m_themeColor = color;
                asset.m_yieldMoleculeNames = molecules;
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                asset.m_displayName = displayName;
                asset.m_ingredientType = type;
                asset.m_origin = origin;
                asset.m_targetChamber = chamber;
                asset.m_themeColor = color;
                asset.m_yieldMoleculeNames = molecules;
                EditorUtility.SetDirty(asset);
            }
            list.Add(asset);
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

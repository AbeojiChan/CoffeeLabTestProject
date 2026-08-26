#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MolecularBrewing.Preparation.Editor
{
    public class RecipeDataGeneratorWindow : EditorWindow
    {
        #region Publics

        [MenuItem("Tools/Preparation 0.2/Recipe & Additive Generator (GD)", false, 20)]
        public static void OpenWindow()
        {
            var window = GetWindow<RecipeDataGeneratorWindow>("Recipe & Additive Generator");
            window.minSize = new Vector2(500, 650);
            window.Show();
        }

        #endregion


        #region Unity API

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("🛠️ COFFEE LAB - RECIPE & ADDITIVE GENERATOR", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Upper section: Base Drinks in 3 City Columns (Neo / Mix / Heart).\nLower section: Additive Synergy Effects linkable to base drinks.", MessageType.Info);
            EditorGUILayout.Space(10);

            _selectedTab = GUILayout.Toolbar(_selectedTab, new[] { "1. Base Recipe Creator", "2. Additive Effect Creator", "3. Batch Generator" });
            EditorGUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_selectedTab == 0)
            {
                DrawBaseRecipeCreator();
            }
            else if (_selectedTab == 1)
            {
                DrawAdditiveEffectCreator();
            }
            else
            {
                DrawBatchGenerator();
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion


        #region Main API

        private void DrawBaseRecipeCreator()
        {
            EditorGUILayout.LabelField("Base Recipe Details", EditorStyles.boldLabel);
            _recipeName = EditorGUILayout.TextField("Recipe Name", _recipeName);
            _cityOrigin = (CityOrigin)EditorGUILayout.EnumPopup("Target Audience / Origin", _cityOrigin);
            _isUnlocked = EditorGUILayout.Toggle("Unlocked By Default", _isUnlocked);
            _description = EditorGUILayout.TextField("Description", _description);

            EditorGUILayout.Space(5);
            _hasSpecialEffect = EditorGUILayout.Toggle("Has Inherent Effect", _hasSpecialEffect);
            if (_hasSpecialEffect)
            {
                _inherentEffectName = EditorGUILayout.TextField("Inherent Effect Name", _inherentEffectName);
                _inherentEffectDesc = EditorGUILayout.TextField("Inherent Effect Desc", _inherentEffectDesc);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Base Ingredients (Placed on Tray)", EditorStyles.boldLabel);
            int count = EditorGUILayout.IntSlider("Ingredient Count", _selectedIngredients.Count, 0, 7);

            while (_selectedIngredients.Count < count) _selectedIngredients.Add(null);
            while (_selectedIngredients.Count > count) _selectedIngredients.RemoveAt(_selectedIngredients.Count - 1);

            for (int i = 0; i < _selectedIngredients.Count; i++)
            {
                _selectedIngredients[i] = (RawIngredientItemData)EditorGUILayout.ObjectField($"Ingredient {i + 1}", _selectedIngredients[i], typeof(RawIngredientItemData), false);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Calculated Synthesis Molecules (Auto)", EditorStyles.boldLabel);
            Dictionary<string, int> moleculeYield = CalculateMolecules(_selectedIngredients);

            if (moleculeYield.Count == 0)
            {
                EditorGUILayout.HelpBox("No ingredients selected yet.", MessageType.None);
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                int totalMolecules = 0;
                foreach (var kvp in moleculeYield)
                {
                    EditorGUILayout.LabelField($"• {kvp.Value}x {kvp.Key}");
                    totalMolecules += kvp.Value;
                }
                EditorGUILayout.LabelField($"<b>Total Molecules:</b> {totalMolecules}", new GUIStyle(EditorStyles.label) { richText = true });
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(15);
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f, 1f);
            if (GUILayout.Button("💾 Save Base Recipe Asset", GUILayout.Height(36)))
            {
                SaveBaseRecipeAsset();
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawAdditiveEffectCreator()
        {
            EditorGUILayout.LabelField("Additive Synergy Effect Details", EditorStyles.boldLabel);
            _effectName = EditorGUILayout.TextField("Effect Name", _effectName);
            _effectUnlocked = EditorGUILayout.Toggle("Unlocked By Default", _effectUnlocked);
            _effectDesc = EditorGUILayout.TextField("Effect Description", _effectDesc);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Required Additives (Single or Combination)", EditorStyles.boldLabel);
            int count = EditorGUILayout.IntSlider("Additives Count", _selectedAdditives.Count, 0, 4);

            while (_selectedAdditives.Count < count) _selectedAdditives.Add(null);
            while (_selectedAdditives.Count > count) _selectedAdditives.RemoveAt(_selectedAdditives.Count - 1);

            for (int i = 0; i < _selectedAdditives.Count; i++)
            {
                _selectedAdditives[i] = (RawIngredientItemData)EditorGUILayout.ObjectField($"Additive {i + 1}", _selectedAdditives[i], typeof(RawIngredientItemData), false);
            }

            EditorGUILayout.Space(15);
            GUI.backgroundColor = new Color(0.2f, 0.7f, 0.8f, 1f);
            if (GUILayout.Button("💾 Save Additive Effect Asset", GUILayout.Height(36)))
            {
                SaveAdditiveEffectAsset();
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawBatchGenerator()
        {
            EditorGUILayout.LabelField("Demo Presets Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Click below to automatically generate all 6 Base Recipes across the 3 Columns (Neo / Mix / Heart) and all 5 Additive Effect Rules!", MessageType.Info);

            EditorGUILayout.Space(15);
            if (GUILayout.Button("⚡ Generate All Default Recipes & Additive Effects", GUILayout.Height(42)))
            {
                GenerateAllDemoData();
            }
        }

        #endregion


        #region Tools and Utilities

        private Dictionary<string, int> CalculateMolecules(List<RawIngredientItemData> ingredients)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (ingredients == null) return dict;

            foreach (var ing in ingredients)
            {
                if (ing == null || ing.m_yieldMoleculeNames == null) continue;
                foreach (var mol in ing.m_yieldMoleculeNames)
                {
                    if (string.IsNullOrEmpty(mol)) continue;
                    if (!dict.ContainsKey(mol)) dict[mol] = 0;
                    dict[mol]++;
                }
            }
            return dict;
        }

        private void SaveBaseRecipeAsset()
        {
            if (string.IsNullOrEmpty(_recipeName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a Recipe Name.", "OK");
                return;
            }

            string folder = "Assets/_/DATABASE/Recipes";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string cleanName = _recipeName.Replace(" ", "_").Replace("-", "_");
            string path = $"{folder}/Recipe_{cleanName}.asset";

            RecipeData asset = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
            if (asset == null)
            {
                asset = CreateInstance<RecipeData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.m_recipeId = $"recipe_{cleanName.ToLowerInvariant()}";
            asset.m_recipeName = _recipeName;
            asset.m_cityOrigin = _cityOrigin;
            asset.m_description = _description;
            asset.m_isUnlocked = _isUnlocked;
            asset.m_hasSpecialEffect = _hasSpecialEffect;
            asset.m_inherentEffectName = _inherentEffectName;
            asset.m_inherentEffectDescription = _inherentEffectDesc;
            asset.m_requiredRawIngredients = new List<RawIngredientItemData>(_selectedIngredients);
            asset.AutoCalculateMoleculesFromIngredients();

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Base recipe saved at:\n{path}", "OK");
        }

        private void SaveAdditiveEffectAsset()
        {
            if (string.IsNullOrEmpty(_effectName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter an Effect Name.", "OK");
                return;
            }

            string folder = "Assets/_/DATABASE/AdditiveEffects";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string cleanName = _effectName.Replace(" ", "_").Replace("-", "_");
            string path = $"{folder}/Effect_{cleanName}.asset";

            AdditiveEffectData asset = AssetDatabase.LoadAssetAtPath<AdditiveEffectData>(path);
            if (asset == null)
            {
                asset = CreateInstance<AdditiveEffectData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.m_effectId = $"effect_{cleanName.ToLowerInvariant()}";
            asset.m_effectName = _effectName;
            asset.m_effectDescription = _effectDesc;
            asset.m_isUnlocked = _effectUnlocked;
            asset.m_requiredAdditives = new List<RawIngredientItemData>(_selectedAdditives);

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Additive effect saved at:\n{path}", "OK");
        }

        public static void GenerateAllDemoData()
        {
            string rawFolder = "Assets/_/DATABASE/RawIngredients";
            string recipeFolder = "Assets/_/DATABASE/Recipes";
            string effectFolder = "Assets/_/DATABASE/AdditiveEffects";

            if (!Directory.Exists(recipeFolder)) Directory.CreateDirectory(recipeFolder);
            if (!Directory.Exists(effectFolder)) Directory.CreateDirectory(effectFolder);

            var cafe = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_CoffeeBeans.asset");
            var the = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_TeaLeaves.asset");
            var eau = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_Water.asset");
            var lait = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_Milk.asset");
            var menthe = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_MintLeaves.asset");
            var nickel = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_LiquidNickel.asset");

            var lunardust = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_LunarDust.asset");
            var dragontears = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_Dragontears.asset");
            var bluepeppers = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_BluePeppers.asset");
            var icedlava = AssetDatabase.LoadAssetAtPath<RawIngredientItemData>($"{rawFolder}/Ing_IcedLava.asset");

            // 1. Column Neo (Neo Fremio)
            CreateBaseRecipe(recipeFolder, "Espresso Overclock", CityOrigin.Neo, "Double dose de caféine pure pour les esprits survoltés de Neo Fremio.", true, false, "", "", new List<RawIngredientItemData> { cafe, cafe, eau });
            CreateBaseRecipe(recipeFolder, "Cyber Tonic Nickel", CityOrigin.Neo, "Infusion tonifiante enrichie en ions conducteurs.", false, true, "Conduction Neuronale", "Accélère le traitement de l'information.", new List<RawIngredientItemData> { cafe, nickel, eau });

            // 2. Column Mix (Both Cities)
            CreateBaseRecipe(recipeFolder, "Thermal Glitch Brew", CityOrigin.Mix, "Café infusé à la menthe cybernétique, apprécié des deux cultures.", true, false, "", "", new List<RawIngredientItemData> { cafe, menthe, eau });
            CreateBaseRecipe(recipeFolder, "Universal Harmony", CityOrigin.Mix, "Alliance subtile de thé et café pour réconcilier les citadins.", false, false, "", "", new List<RawIngredientItemData> { cafe, the, eau });

            // 3. Column Heart (Heartopia)
            CreateBaseRecipe(recipeFolder, "Matcha Neuro-Latte", CityOrigin.Heart, "Infusion végétale émulsionnée avec solvant lacté d'Heartopia.", true, false, "", "", new List<RawIngredientItemData> { the, lait, eau });
            CreateBaseRecipe(recipeFolder, "Sweet Mint Elixir", CityOrigin.Heart, "Élixir rafraîchissant à base de menthe pure d'Heartopia.", false, true, "Sérénité Végétale", "Procure un état d'apaisement absolu.", new List<RawIngredientItemData> { the, menthe, eau });

            // 4. Additive Synergy Effects
            CreateAdditiveEffect(effectFolder, "Transe Onirique", "Plonge le buveur dans une transe créative profonde.", true, new List<RawIngredientItemData> { lunardust });
            CreateAdditiveEffect(effectFolder, "Extase Mystique", "Déclenche une vague d'euphorie et de clairvoyance.", true, new List<RawIngredientItemData> { dragontears });
            CreateAdditiveEffect(effectFolder, "Choc Thermique", "Génère une chaleur intense stimulant le métabolisme.", true, new List<RawIngredientItemData> { bluepeppers });
            CreateAdditiveEffect(effectFolder, "Cryo-Stase", "Refroidit instantanément le système et élimine les toxines.", true, new List<RawIngredientItemData> { icedlava });
            CreateAdditiveEffect(effectFolder, "Éveil Céleste", "Synergie puissante alliant rêve lucide et béatitude mystique.", false, new List<RawIngredientItemData> { lunardust, dragontears });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green><b>[RecipeDataGenerator]</b> Successfully generated Base Recipes and Additive Effects in DATABASE!</color>");
        }

        private static void CreateBaseRecipe(string folder, string name, CityOrigin origin, string desc, bool unlocked, bool hasEffect, string effName, string effDesc, List<RawIngredientItemData> ingredients)
        {
            string cleanName = name.Replace(" ", "_");
            string path = $"{folder}/Recipe_{cleanName}.asset";

            RecipeData asset = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
            if (asset == null)
            {
                asset = CreateInstance<RecipeData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.m_recipeId = $"recipe_{cleanName.ToLowerInvariant()}";
            asset.m_recipeName = name;
            asset.m_cityOrigin = origin;
            asset.m_description = desc;
            asset.m_isUnlocked = unlocked;
            asset.m_hasSpecialEffect = hasEffect;
            asset.m_inherentEffectName = effName;
            asset.m_inherentEffectDescription = effDesc;
            asset.m_requiredRawIngredients = new List<RawIngredientItemData>(ingredients);
            asset.AutoCalculateMoleculesFromIngredients();

            EditorUtility.SetDirty(asset);
        }

        private static void CreateAdditiveEffect(string folder, string name, string desc, bool unlocked, List<RawIngredientItemData> additives)
        {
            string cleanName = name.Replace(" ", "_");
            string path = $"{folder}/Effect_{cleanName}.asset";

            AdditiveEffectData asset = AssetDatabase.LoadAssetAtPath<AdditiveEffectData>(path);
            if (asset == null)
            {
                asset = CreateInstance<AdditiveEffectData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.m_effectId = $"effect_{cleanName.ToLowerInvariant()}";
            asset.m_effectName = name;
            asset.m_effectDescription = desc;
            asset.m_isUnlocked = unlocked;
            asset.m_requiredAdditives = new List<RawIngredientItemData>(additives);

            EditorUtility.SetDirty(asset);
        }

        #endregion


        #region Private and Protected

        private Vector2 _scrollPos;
        private int _selectedTab = 0;

        // Base Recipe Form
        private string _recipeName = "New Cyber Drink";
        private CityOrigin _cityOrigin = CityOrigin.Neo;
        private string _description = "Description de la formulation.";
        private bool _isUnlocked = false;
        private bool _hasSpecialEffect = false;
        private string _inherentEffectName = "";
        private string _inherentEffectDesc = "";
        private List<RawIngredientItemData> _selectedIngredients = new List<RawIngredientItemData>();

        // Additive Effect Form
        private string _effectName = "New Additive Synergy";
        private string _effectDesc = "Description de l'effet produit par cet additif.";
        private bool _effectUnlocked = false;
        private List<RawIngredientItemData> _selectedAdditives = new List<RawIngredientItemData>();

        #endregion
    }
}
#endif

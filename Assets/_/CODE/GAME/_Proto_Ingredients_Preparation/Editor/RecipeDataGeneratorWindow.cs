#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            window.minSize = new Vector2(550, 750);
            window.Show();
        }

        #endregion


        #region Unity API

        private void OnEnable()
        {
            if (_customMolecules.Count == 0)
            {
                _customMolecules.Add(new MoleculeRequirement { m_moleculeName = "Caffeine", m_requiredCount = 2 });
                _customMolecules.Add(new MoleculeRequirement { m_moleculeName = "Water", m_requiredCount = 3 });
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("🛠️ COFFEE LAB - RECIPE & ADDITIVE GENERATOR", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure Base Drinks (with precise molecule requirements and sprites) and Additive Synergy Effects.", MessageType.Info);
            EditorGUILayout.Space(10);

            _selectedTab = GUILayout.Toolbar(_selectedTab, new[] { "1. Base Recipe Creator", "2. Additive Effect Creator" });
            EditorGUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_selectedTab == 0)
            {
                DrawBaseRecipeCreator();
            }
            else
            {
                DrawAdditiveEffectCreator();
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion


        #region Main API

        private void DrawBaseRecipeCreator()
        {
            EditorGUILayout.LabelField("Base Recipe Identity", EditorStyles.boldLabel);
            _recipeName = EditorGUILayout.TextField("Recipe Name", _recipeName);
            _recipeSprite = (Sprite)EditorGUILayout.ObjectField("Drink Sprite (Icon)", _recipeSprite, typeof(Sprite), false);
            _cityOrigin = (CityOrigin)EditorGUILayout.EnumPopup("Target Audience / Origin", _cityOrigin);
            _isUnlocked = EditorGUILayout.Toggle("Unlocked By Default", _isUnlocked);
            _description = EditorGUILayout.TextField("Description", _description);

            EditorGUILayout.Space(8);
            _hasSpecialEffect = EditorGUILayout.Toggle("Has Inherent Effect", _hasSpecialEffect);
            if (_hasSpecialEffect)
            {
                _inherentEffectName = EditorGUILayout.TextField("Inherent Effect Name", _inherentEffectName);
                _inherentEffectDesc = EditorGUILayout.TextField("Inherent Effect Desc", _inherentEffectDesc);
            }

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Base Raw Ingredients (Placed on Tray)", EditorStyles.boldLabel);
            int count = EditorGUILayout.IntSlider("Ingredient Count", _selectedIngredients.Count, 0, 7);

            while (_selectedIngredients.Count < count) _selectedIngredients.Add(null);
            while (_selectedIngredients.Count > count) _selectedIngredients.RemoveAt(_selectedIngredients.Count - 1);

            for (int i = 0; i < _selectedIngredients.Count; i++)
            {
                _selectedIngredients[i] = (RawIngredientItemData)EditorGUILayout.ObjectField($"Ingredient {i + 1}", _selectedIngredients[i], typeof(RawIngredientItemData), false);
            }

            EditorGUILayout.Space(14);
            EditorGUILayout.LabelField("Synthesis Molecule Composition & Count", EditorStyles.boldLabel);

            Dictionary<string, int> availableMolecules = CalculateMolecules(_selectedIngredients);
            List<string> availableList = availableMolecules.Keys.ToList();

            _manualMoleculeOverride = EditorGUILayout.ToggleLeft("<b>Custom Precision Molecule Definition (Override auto-calculation)</b>", _manualMoleculeOverride, new GUIStyle(EditorStyles.label) { richText = true });

            if (_manualMoleculeOverride)
            {
                EditorGUILayout.HelpBox("Pick directly from the molecules provided by the selected ingredients above or enter custom names.", MessageType.None);

                // Quick add buttons for available molecules from ingredients
                if (availableList.Count > 0)
                {
                    EditorGUILayout.LabelField("Quick Add from Ingredients Yield:", EditorStyles.miniBoldLabel);
                    EditorGUILayout.BeginHorizontal();
                    foreach (var kvp in availableMolecules)
                    {
                        if (GUILayout.Button($"+ {kvp.Key} (x{kvp.Value})", EditorStyles.miniButton))
                        {
                            var existing = _customMolecules.Find(m => m.m_moleculeName == kvp.Key);
                            if (existing != null)
                            {
                                existing.m_requiredCount++;
                            }
                            else
                            {
                                _customMolecules.Add(new MoleculeRequirement { m_moleculeName = kvp.Key, m_requiredCount = kvp.Value });
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                int indexToRemove = -1;
                for (int i = 0; i < _customMolecules.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    // If we have available molecules from ingredients, show dropdown picker
                    if (availableList.Count > 0)
                    {
                        List<string> options = new List<string>(availableList) { "[Custom Text...]" };
                        int currentIndex = options.IndexOf(_customMolecules[i].m_moleculeName);
                        if (currentIndex == -1) currentIndex = options.Count - 1; // Custom

                        int newIndex = EditorGUILayout.Popup(currentIndex, options.ToArray(), GUILayout.Width(170));
                        if (newIndex < availableList.Count)
                        {
                            _customMolecules[i].m_moleculeName = availableList[newIndex];
                        }
                        else if (currentIndex != options.Count - 1)
                        {
                            _customMolecules[i].m_moleculeName = "CustomMolecule";
                        }
                    }

                    _customMolecules[i].m_moleculeName = EditorGUILayout.TextField(_customMolecules[i].m_moleculeName);
                    _customMolecules[i].m_requiredCount = EditorGUILayout.IntField("Count", Mathf.Max(1, _customMolecules[i].m_requiredCount), GUILayout.Width(110));

                    if (GUILayout.Button("✖", GUILayout.Width(25)))
                    {
                        indexToRemove = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (indexToRemove >= 0 && indexToRemove < _customMolecules.Count)
                {
                    _customMolecules.RemoveAt(indexToRemove);
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Molecule Row", GUILayout.Height(24)))
                {
                    string defaultName = availableList.Count > 0 ? availableList[0] : "Caffeine";
                    _customMolecules.Add(new MoleculeRequirement { m_moleculeName = defaultName, m_requiredCount = 1 });
                }
                if (GUILayout.Button("🔄 Sync All from Ingredients", GUILayout.Height(24)))
                {
                    SyncCustomFromIngredients();
                }
                if (GUILayout.Button("🗑️ Clear", GUILayout.Width(60), GUILayout.Height(24)))
                {
                    _customMolecules.Clear();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else
            {
                if (availableMolecules.Count == 0)
                {
                    EditorGUILayout.HelpBox("No ingredients selected yet.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    int totalMolecules = 0;
                    foreach (var kvp in availableMolecules)
                    {
                        EditorGUILayout.LabelField($"• {kvp.Value}x {kvp.Key}");
                        totalMolecules += kvp.Value;
                    }
                    EditorGUILayout.LabelField($"<b>Total Auto Molecules:</b> {totalMolecules}", new GUIStyle(EditorStyles.label) { richText = true });
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space(18);
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f, 1f);
            if (GUILayout.Button("💾 Save Base Recipe Asset", GUILayout.Height(40)))
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

            EditorGUILayout.Space(18);
            GUI.backgroundColor = new Color(0.2f, 0.7f, 0.8f, 1f);
            if (GUILayout.Button("💾 Save Additive Effect Asset", GUILayout.Height(40)))
            {
                SaveAdditiveEffectAsset();
            }
            GUI.backgroundColor = Color.white;
        }

        #endregion


        #region Tools and Utilities

        private void SyncCustomFromIngredients()
        {
            _customMolecules.Clear();
            Dictionary<string, int> dict = CalculateMolecules(_selectedIngredients);
            foreach (var kvp in dict)
            {
                _customMolecules.Add(new MoleculeRequirement
                {
                    m_moleculeName = kvp.Key,
                    m_requiredCount = kvp.Value
                });
            }
        }

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
            asset.m_icon = _recipeSprite;
            asset.m_cityOrigin = _cityOrigin;
            asset.m_description = _description;
            asset.m_isUnlocked = _isUnlocked;
            asset.m_hasSpecialEffect = _hasSpecialEffect;
            asset.m_inherentEffectName = _inherentEffectName;
            asset.m_inherentEffectDescription = _inherentEffectDesc;
            asset.m_requiredRawIngredients = new List<RawIngredientItemData>(_selectedIngredients);

            if (_manualMoleculeOverride && _customMolecules.Count > 0)
            {
                asset.m_requiredMolecules = new List<MoleculeRequirement>(_customMolecules);
            }
            else
            {
                asset.AutoCalculateMoleculesFromIngredients();
            }

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

        #endregion


        #region Private and Protected

        private int _selectedTab = 0;
        private Vector2 _scrollPos;

        // Base Recipe Creator state
        private string _recipeName = "New Drink Recipe";
        private Sprite _recipeSprite;
        private CityOrigin _cityOrigin = CityOrigin.Neo;
        private bool _isUnlocked = false;
        private string _description = "Description de la boisson...";
        private bool _hasSpecialEffect = false;
        private string _inherentEffectName = "";
        private string _inherentEffectDesc = "";
        private readonly List<RawIngredientItemData> _selectedIngredients = new List<RawIngredientItemData>();

        // Custom Molecule requirements
        private bool _manualMoleculeOverride = false;
        private readonly List<MoleculeRequirement> _customMolecules = new List<MoleculeRequirement>();

        // Additive Effect Creator state
        private string _effectName = "New Additive Effect";
        private bool _effectUnlocked = false;
        private string _effectDesc = "Description de l'effet...";
        private readonly List<RawIngredientItemData> _selectedAdditives = new List<RawIngredientItemData>();

        #endregion
    }
}
#endif

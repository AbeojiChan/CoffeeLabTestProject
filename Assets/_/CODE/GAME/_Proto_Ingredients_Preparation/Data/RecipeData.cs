using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Preparation
{
    [Serializable]
    public class MoleculeRequirement
    {
        public string m_moleculeName = "Caffeine";
        public int m_requiredCount = 2;
    }

    [CreateAssetMenu(fileName = "NewRecipeData", menuName = "CoffeeLab/Preparation/Recipe Data", order = 2)]
    public class RecipeData : ScriptableObject
    {
        #region Publics

        [Header("Recipe Identity")]
        public string m_recipeId = "recipe_espresso_overclock";
        public string m_recipeName = "Espresso Overclock";
        public CityOrigin m_cityOrigin = CityOrigin.Neo; // Neo (Neo Fremio), Heart (Heartopia), Mix (Both)
        public Sprite m_icon;
        [TextArea(2, 4)] public string m_description = "Double stimulation neuronale par extraction intense de caféine.";

        [Header("Unlock Status (Unlocked on 1st successful preparation)")]
        public bool m_isUnlocked = false;

        [Header("Effect Properties")]
        public bool m_hasSpecialEffect = false;
        public string m_inherentEffectName = "";
        [TextArea(1, 3)] public string m_inherentEffectDescription = "";

        [Header("Required Raw Ingredients (Base + Optional Additives)")]
        [Tooltip("Base raw ingredients placed on the tray")]
        public List<RawIngredientItemData> m_requiredRawIngredients = new List<RawIngredientItemData>();

        [Header("Linked Additive Synergies")]
        [Tooltip("Additive effect rules that can be linked to this base drink")]
        public List<AdditiveEffectData> m_linkedAdditiveEffects = new List<AdditiveEffectData>();

        [Header("Synthesis Molecule Breakdown")]
        [Tooltip("Target molecules required in the synthesis machine, derived from prepared ingredients")]
        public List<MoleculeRequirement> m_requiredMolecules = new List<MoleculeRequirement>();

        public string CategoryTag => m_cityOrigin.ToString();

        #endregion


        #region Unity API
        #endregion


        #region Main API

        public void Unlock()
        {
            m_isUnlocked = true;
        }

        public void Lock()
        {
            m_isUnlocked = false;
        }

        public void AutoCalculateMoleculesFromIngredients()
        {
            m_requiredMolecules.Clear();
            if (m_requiredRawIngredients == null) return;

            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (var ing in m_requiredRawIngredients)
            {
                if (ing == null || ing.m_yieldMoleculeNames == null) continue;

                foreach (var mol in ing.m_yieldMoleculeNames)
                {
                    if (string.IsNullOrEmpty(mol)) continue;

                    if (!counts.ContainsKey(mol))
                    {
                        counts[mol] = 0;
                    }
                    counts[mol]++;
                }
            }

            foreach (var kvp in counts)
            {
                m_requiredMolecules.Add(new MoleculeRequirement
                {
                    m_moleculeName = kvp.Key,
                    m_requiredCount = kvp.Value
                });
            }
        }

        #endregion


        #region Tools and Utilities
        #endregion


        #region Private and Protected
        #endregion
    }
}

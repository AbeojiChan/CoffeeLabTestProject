using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Preparation
{
    public enum RawIngredientType
    {
        Base,
        Additive
    }

    [CreateAssetMenu(fileName = "NewRawIngredient", menuName = "CoffeeLab/Preparation/Raw Ingredient Data", order = 1)]
    public class RawIngredientItemData : ScriptableObject
    {
        #region Publics

        [Header("Identity & Visuals")]
        public string m_id = "coffee_beans";
        public string m_displayName = "Coffee Beans";
        public RawIngredientType m_ingredientType = RawIngredientType.Base;
        public CityOrigin m_origin = CityOrigin.Neo; // Neo Fremio, Heartopia, or Mix
        public Sprite m_sprite;
        public Sprite m_icon => m_sprite;
        public Color m_themeColor = Color.white;

        [Header("Preparation Machine Target")]
        public MachineChamberType m_targetChamber = MachineChamberType.Chamber1_Grinder;
        public float m_processingDuration = 2.5f;

        [Header("Synthesis Molecule Yield (3 Molecules)")]
        [Tooltip("List of 3 molecules produced when processed and placed inside the Synthesis Machine")]
        public List<string> m_yieldMoleculeNames = new List<string> { "Caffeine", "Caffeine", "Chlorogenic Acid" };

        #endregion


        #region Unity API
        #endregion


        #region Main API

        public Sprite GetSprite() => m_sprite;

        #endregion


        #region Tools and Utilities
        #endregion


        #region Private and Protected
        #endregion
    }
}

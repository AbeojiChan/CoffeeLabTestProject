using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Preparation
{
    [CreateAssetMenu(fileName = "NewAdditiveEffect", menuName = "CoffeeLab/Preparation/Additive Effect Rule", order = 3)]
    public class AdditiveEffectData : ScriptableObject
    {
        #region Publics

        [Header("Effect Identity")]
        public string m_effectId = "effect_neural_overclock";
        public string m_effectName = "Overclock Neural";
        public Sprite m_icon;
        [TextArea(2, 4)] public string m_effectDescription = "Augmente drastiquement les réflexes et la concentration neuronale.";

        [Header("Unlock Status")]
        public bool m_isUnlocked = false;

        [Header("Required Additives (Single or Combination)")]
        [Tooltip("One or more additive ingredients required to produce this effect")]
        public List<RawIngredientItemData> m_requiredAdditives = new List<RawIngredientItemData>();

        #endregion


        #region Unity API
        #endregion


        #region Main API

        public void Unlock() => m_isUnlocked = true;
        public void Lock() => m_isUnlocked = false;

        public bool MatchesAdditives(List<RawIngredientItemData> currentAdditives)
        {
            if (m_requiredAdditives == null || currentAdditives == null) return false;
            if (m_requiredAdditives.Count != currentAdditives.Count) return false;

            List<string> reqIds = new List<string>();
            foreach (var a in m_requiredAdditives) if (a != null) reqIds.Add(a.m_id);

            List<string> curIds = new List<string>();
            foreach (var c in currentAdditives) if (c != null) curIds.Add(c.m_id);

            reqIds.Sort();
            curIds.Sort();

            for (int i = 0; i < reqIds.Count; i++)
            {
                if (reqIds[i] != curIds[i]) return false;
            }

            return true;
        }

        #endregion


        #region Tools and Utilities
        #endregion


        #region Private and Protected
        #endregion
    }
}

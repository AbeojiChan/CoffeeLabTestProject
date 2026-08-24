//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MolecularBrewing.Runtime
//{
//    public enum SpecialEffectType
//    {
//        None,
//        AnxietyCure,        // Cures chronic cyber-anxiety & calms neural tremors
//        DeepSleep,          // Induces immediate restorative REM sleep
//        NeuralOverclock,    // Hyper-focus, accelerated reflex & cognitive boost
//        Euphoria,           // Intense mood elevation & anti-depressive surge
//        CellularDetox,      // Clears metabolic toxins, cyber-burnout & hangover
//        LucidClarity        // Clears sensory fog & enhances mental sharpness
//    }

//    [Serializable]
//    public class SpecialEffectDefinition
//    {
//        #region Publics

//        public SpecialEffectType m_type;
//        public string m_displayName;
//        public string m_tagline;
//        [TextArea] public string m_description;
//        public Color m_badgeColor;

//        [Header("Activation Trigger Requirements")]
//        public MoleculeType m_requiredMolecule;
//        public int m_minCount;
//        public MoleculeType m_requiredBondWith;
//        public bool m_requiresBond;

//        #endregion


//        #region Main API

//        public SpecialEffectDefinition(
//            SpecialEffectType type,
//            string displayName,
//            string tagline,
//            string description,
//            Color badgeColor,
//            MoleculeType requiredMolecule,
//            int minCount,
//            bool requiresBond = false,
//            MoleculeType bondWith = MoleculeType.Water)
//        {
//            m_type = type;
//            m_displayName = displayName;
//            m_tagline = tagline;
//            m_description = description;
//            m_badgeColor = badgeColor;
//            m_requiredMolecule = requiredMolecule;
//            m_minCount = minCount;
//            m_requiresBond = requiresBond;
//            m_requiredBondWith = bondWith;
//        }

//        #endregion


//        #region Tools and Utilities

//        public static List<SpecialEffectDefinition> CreateDefaultEffects()
//        {
//            return new List<SpecialEffectDefinition>
//            {
//                new SpecialEffectDefinition(
//                    SpecialEffectType.AnxietyCure,
//                    "Anxiolytic Neuro-Calm",
//                    "Cures Cyber-Anxiety & Panic",
//                    "Dissolves cognitive dread and stabilizes neurotransmitter tremors, unlocking tranquil narrative responses.",
//                    new Color(0.20f, 0.85f, 0.75f, 1f),
//                    MoleculeType.NeuroLotusExtract,
//                    2,
//                    requiresBond: true,
//                    bondWith: MoleculeType.Water
//                )
//            };
//        }

//        #endregion
//    }
//}

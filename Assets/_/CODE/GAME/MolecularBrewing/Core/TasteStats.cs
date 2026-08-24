using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    [Serializable]
    public struct TasteStats
    {
        #region Publics

        [Header("Sensory Flavor Stats")]
        [Range(0, 100)] public float m_sweetness;
        [Range(0, 100)] public float m_bitterness;
        [Range(0, 100)] public float m_acidity;
        [Range(0, 100)] public float m_creaminess;
        [Range(0, 100)] public float m_caffeine;
        [Range(0, 100)] public float m_aroma;

        [Header("Cyber-Medicinal / Neural Bio-Stats")]
        [Range(0, 100)] public float m_neuralCalm;
        [Range(0, 100)] public float m_sedation;
        [Range(0, 100)] public float m_focus;
        [Range(0, 100)] public float m_euphoria;

        public static TasteStats Zero => new TasteStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        #endregion


        #region Main API

        public TasteStats(
            float sweetness,
            float bitterness,
            float acidity,
            float creaminess,
            float caffeine,
            float aroma,
            float neuralCalm = 0f,
            float sedation = 0f,
            float focus = 0f,
            float euphoria = 0f)
        {
            m_sweetness = Mathf.Clamp(sweetness, 0f, 100f);
            m_bitterness = Mathf.Clamp(bitterness, 0f, 100f);
            m_acidity = Mathf.Clamp(acidity, 0f, 100f);
            m_creaminess = Mathf.Clamp(creaminess, 0f, 100f);
            m_caffeine = Mathf.Clamp(caffeine, 0f, 100f);
            m_aroma = Mathf.Clamp(aroma, 0f, 100f);
            m_neuralCalm = Mathf.Clamp(neuralCalm, 0f, 100f);
            m_sedation = Mathf.Clamp(sedation, 0f, 100f);
            m_focus = Mathf.Clamp(focus, 0f, 100f);
            m_euphoria = Mathf.Clamp(euphoria, 0f, 100f);
        }

        public string GetPrimaryFlavorDescription()
        {
            List<string> notes = new List<string>();

            if (m_neuralCalm > 50f) notes.Add("Anxiety-Suppressive Calm");
            if (m_sedation > 50f) notes.Add("Deep Sleep Induction");
            if (m_focus > 50f) notes.Add("Accelerated Synaptic Focus");
            if (m_euphoria > 50f) notes.Add("Euphoric Warmth");

            if (m_sweetness > 60f) notes.Add("Sweet Nectar");
            if (m_bitterness > 65f) notes.Add("Dark Roasted");
            if (m_creaminess > 60f) notes.Add("Velvety Silk");
            if (m_caffeine > 70f) notes.Add("High Voltage Kick");
            if (m_acidity > 50f) notes.Add("Crisp Brightness");
            if (m_aroma > 60f) notes.Add("Fragrant Bouquet");

            if (notes.Count == 0) return "Clean & Balanced Molecular Extraction";
            return string.Join(" | ", notes);
        }

        #endregion


        #region Tools and Utilities

        public static TasteStats Calculate(
            IReadOnlyList<MoleculeType> placedMolecules,
            IReadOnlyList<(MoleculeType a, MoleculeType b)> activeBonds,
            Func<MoleculeType, MoleculeDefinition> defLookup)
        {
            if (placedMolecules == null || placedMolecules.Count == 0)
                return Zero;

            float sweet = 0f;
            float bitter = 0f;
            float acid = 0f;
            float cream = 0f;
            float caff = 0f;
            float arom = 0f;
            float calm = 0f;
            float sed = 0f;
            float foc = 0f;
            float euph = 0f;

            // 1. Base Molecule contributions
            foreach (var mol in placedMolecules)
            {
                var def = defLookup(mol);
                if (def != null)
                {
                    sweet += def.m_sweetnessImpact;
                    bitter += def.m_bitternessImpact;
                    acid += def.m_acidityImpact;
                    cream += def.m_creaminessImpact;
                    caff += def.m_caffeineImpact;
                    arom += def.m_aromaImpact;
                    calm += def.m_neuralCalmImpact;
                    sed += def.m_sedationImpact;
                    foc += def.m_focusImpact;
                    euph += def.m_euphoriaImpact;
                }
            }

            // 2. Drawn Link Synergies (Covalent Bond Reactions)
            if (activeBonds != null)
            {
                foreach (var bond in activeBonds)
                {
                    var a = bond.a;
                    var b = bond.b;

                    // Neuro-Lotus + Water (Activates liquid anxiolytic cure)
                    if (IsBondPair(a, b, MoleculeType.NeuroLotusExtract, MoleculeType.Water))
                    {
                        calm += 25f;
                        sweet += 5f;
                        arom += 10f;
                    }
                    // Somnin + Lipid (Stabilizes slow-release sleep sedative)
                    else if (IsBondPair(a, b, MoleculeType.Somnin, MoleculeType.Lipid))
                    {
                        sed += 30f;
                        calm += 15f;
                        cream += 10f;
                    }
                    // Hyper-Ginseng + Caffeine (Super-stimulant focus overclock)
                    else if (IsBondPair(a, b, MoleculeType.HyperGinsenoside, MoleculeType.Caffeine))
                    {
                        foc += 35f;
                        caff += 15f;
                        bitter += 5f;
                    }
                    // Glitch-Endorphin + Sucrose (Sugar-carrier euphoric trigger)
                    else if (IsBondPair(a, b, MoleculeType.GlitchEndorphin, MoleculeType.Sucrose))
                    {
                        euph += 30f;
                        sweet += 10f;
                        arom += 10f;
                    }
                    // Nano-Scavenger + Water (Full metabolic purge)
                    else if (IsBondPair(a, b, MoleculeType.NanoScavenger, MoleculeType.Water))
                    {
                        calm += 15f;
                        acid -= 5f;
                    }
                    // Caffeine + Sucrose (Caramelized softening)
                    else if (IsBondPair(a, b, MoleculeType.Caffeine, MoleculeType.Sucrose))
                    {
                        sweet += 6f;
                        bitter -= 4f;
                        arom += 4f;
                    }
                    // Caffeine + Lipid (Micro-foam emulsion)
                    else if (IsBondPair(a, b, MoleculeType.Caffeine, MoleculeType.Lipid))
                    {
                        cream += 10f;
                        bitter -= 5f;
                    }
                    // Lipid + Water (Stable micro-emulsion)
                    else if (IsBondPair(a, b, MoleculeType.Lipid, MoleculeType.Water))
                    {
                        cream += 12f;
                        sweet += 3f;
                    }
                    // Cocoa + Caffeine (Mocha harmony)
                    else if (IsBondPair(a, b, MoleculeType.CocoaFlavonoid, MoleculeType.Caffeine))
                    {
                        caff += 8f;
                        bitter += 6f;
                        arom += 8f;
                    }
                }
            }

            return new TasteStats(sweet, bitter, acid, cream, caff, arom, calm, sed, foc, euph);
        }

        private static bool IsBondPair(MoleculeType a, MoleculeType b, MoleculeType t1, MoleculeType t2)
        {
            return (a == t1 && b == t2) || (a == t2 && b == t1);
        }

        #endregion
    }
}

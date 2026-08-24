using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    public enum IngredientCategory
    {
        CoffeeBase,
        DairyPlant,
        Sweetener,
        MedicinalFlora,     // Cyberpunk medicinal & neural plants
        Solvent
    }

    [Serializable]
    public class MoleculeYield
    {
        #region Publics

        public MoleculeType m_type;
        public int m_count;

        #endregion


        #region Main API

        public MoleculeYield(MoleculeType type, int count)
        {
            m_type = type;
            m_count = count;
        }

        #endregion
    }

    [Serializable]
    public class IngredientDefinition
    {
        #region Publics

        public string m_id;
        public string m_displayName;
        public IngredientCategory m_category;
        public Color m_iconColor;
        [TextArea] public string m_description;
        public List<MoleculeYield> m_moleculeYields = new List<MoleculeYield>();

        #endregion


        #region Main API

        public IngredientDefinition(string id, string displayName, IngredientCategory category, Color iconColor, string description, params MoleculeYield[] yields)
        {
            m_id = id;
            m_displayName = displayName;
            m_category = category;
            m_iconColor = iconColor;
            m_description = description;
            if (yields != null)
            {
                m_moleculeYields.AddRange(yields);
            }
        }

        #endregion


        #region Tools and Utilities

        public static List<IngredientDefinition> CreateDefaultIngredients()
        {
            return new List<IngredientDefinition>
            {
                // --- Classic Coffee & Dairy ---
                new IngredientDefinition(
                    "dark_roast",
                    "Synthetic Dark Roast",
                    IngredientCategory.CoffeeBase,
                    new Color(0.24f, 0.15f, 0.10f, 1f),
                    "High-density roasted beans offering heavy caffeine and deep roast tannins.",
                    new MoleculeYield(MoleculeType.Caffeine, 3),
                    new MoleculeYield(MoleculeType.CaramelTannin, 2),
                    new MoleculeYield(MoleculeType.ChlorogenicAcid, 1)
                ),
                new IngredientDefinition(
                    "light_roast",
                    "Quantum Ethiopian Beans",
                    IngredientCategory.CoffeeBase,
                    new Color(0.50f, 0.32f, 0.20f, 1f),
                    "Bright, aromatic bean variant packed with chlorogenic acid and clean caffeine.",
                    new MoleculeYield(MoleculeType.Caffeine, 2),
                    new MoleculeYield(MoleculeType.ChlorogenicAcid, 3),
                    new MoleculeYield(MoleculeType.Water, 1)
                ),
                new IngredientDefinition(
                    "reconstituted_milk",
                    "Reconstituted Whole Milk",
                    IngredientCategory.DairyPlant,
                    new Color(0.96f, 0.94f, 0.88f, 1f),
                    "Micro-foamed dairy rich in lipids and natural milk sugars for silky texture.",
                    new MoleculeYield(MoleculeType.Lipid, 3),
                    new MoleculeYield(MoleculeType.Sucrose, 1),
                    new MoleculeYield(MoleculeType.Water, 1)
                ),
                new IngredientDefinition(
                    "oat_emulsion",
                    "Barista Oat Emulsion",
                    IngredientCategory.DairyPlant,
                    new Color(0.91f, 0.84f, 0.72f, 1f),
                    "Creamy plant-based emulsion providing clean lipids and gentle sweetness.",
                    new MoleculeYield(MoleculeType.Lipid, 2),
                    new MoleculeYield(MoleculeType.Sucrose, 2),
                    new MoleculeYield(MoleculeType.Water, 1)
                ),
                new IngredientDefinition(
                    "cane_sugar",
                    "Demerara Sucrose",
                    IngredientCategory.Sweetener,
                    new Color(1.00f, 0.95f, 0.80f, 1f),
                    "Purified crystalline sugar to soften bitterness and elevate flavor harmony.",
                    new MoleculeYield(MoleculeType.Sucrose, 4),
                    new MoleculeYield(MoleculeType.CaramelTannin, 1)
                ),
                new IngredientDefinition(
                    "distilled_solvent",
                    "De-ionized Brew Water",
                    IngredientCategory.Solvent,
                    new Color(0.40f, 0.75f, 0.95f, 1f),
                    "Ultra-pure solvent optimized for molecular extraction and stable dilution.",
                    new MoleculeYield(MoleculeType.Water, 4)
                ),

                // --- Cyberpunk Medicinal & Neural Flora ---
                new IngredientDefinition(
                    "neuro_lotus",
                    "Bioluminescent Neuro-Lotus",
                    IngredientCategory.MedicinalFlora,
                    new Color(0.20f, 0.85f, 0.75f, 1f),
                    "[MEDICINAL FLORA] Genetically modified aquatic flower yielding concentrated anxiolytic Neuro-Lotus extract to cure cyber-anxiety.",
                    new MoleculeYield(MoleculeType.NeuroLotusExtract, 3),
                    new MoleculeYield(MoleculeType.Water, 1)
                ),
                new IngredientDefinition(
                    "somna_moss",
                    "Nightfall Somna-Moss",
                    IngredientCategory.MedicinalFlora,
                    new Color(0.35f, 0.25f, 0.75f, 1f),
                    "[MEDICINAL FLORA] Subterranean bioluminescent moss rich in Somnin. Calms neurotransmitters and induces restorative sleep.",
                    new MoleculeYield(MoleculeType.Somnin, 3),
                    new MoleculeYield(MoleculeType.Lipid, 1)
                ),
                new IngredientDefinition(
                    "hyper_ginseng",
                    "Hyper-Ginseng Root",
                    IngredientCategory.MedicinalFlora,
                    new Color(0.95f, 0.30f, 0.35f, 1f),
                    "[MEDICINAL FLORA] Cyber-enhanced adaptogen root. Accelerates synaptic speed, focus, and neural reaction times.",
                    new MoleculeYield(MoleculeType.HyperGinsenoside, 3),
                    new MoleculeYield(MoleculeType.Caffeine, 1)
                ),
                new IngredientDefinition(
                    "glitch_flower",
                    "Glitch-Root Blossom",
                    IngredientCategory.MedicinalFlora,
                    new Color(0.95f, 0.40f, 0.85f, 1f),
                    "[MEDICINAL FLORA] Rare wild cyber-flora bursting with Glitch-Endorphin to trigger warmth, mood elevation, and euphoria.",
                    new MoleculeYield(MoleculeType.GlitchEndorphin, 3),
                    new MoleculeYield(MoleculeType.Sucrose, 1)
                ),
                new IngredientDefinition(
                    "nano_algae",
                    "Emerald Nano-Algae",
                    IngredientCategory.MedicinalFlora,
                    new Color(0.30f, 0.85f, 0.40f, 1f),
                    "[MEDICINAL FLORA] Bio-engineered microscopic algae yielding active Nano-Scavengers that purge toxins and cure cyber-hangover.",
                    new MoleculeYield(MoleculeType.NanoScavenger, 3),
                    new MoleculeYield(MoleculeType.Water, 1)
                ),
                new IngredientDefinition(
                    "raw_cocoa",
                    "Cyber-Cacao Nibs",
                    IngredientCategory.CoffeeBase,
                    new Color(0.38f, 0.20f, 0.15f, 1f),
                    "Crushed organic cacao loaded with roasted cocoa flavonoids and gentle stimulation.",
                    new MoleculeYield(MoleculeType.CocoaFlavonoid, 3),
                    new MoleculeYield(MoleculeType.Lipid, 1)
                )
            };
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    public enum ApparatusType
    {
        CoffeeGrinder,     
        BotanicalMortar,   
        SteamFrother,     
        SolventKettle    
    }

    [Serializable]
    public class RawIngredientData
    {
        #region Publics

        public string m_rawId;
        public string m_rawName;
        public ApparatusType m_requiredApparatus;
        public Color m_rawColor;
        [TextArea] public string m_rawDescription;
        public IngredientDefinition m_preparedOutput;

        #endregion


        #region Main API

        public RawIngredientData(
            string rawId,
            string rawName,
            ApparatusType requiredApparatus,
            Color rawColor,
            string rawDescription,
            IngredientDefinition preparedOutput)
        {
            m_rawId = rawId;
            m_rawName = rawName;
            m_rawDescription = rawDescription;
            m_requiredApparatus = requiredApparatus;
            m_rawColor = rawColor;
            m_preparedOutput = preparedOutput;
        }

        #endregion


        #region Tools and Utilities

        public static List<RawIngredientData> CreateDefaultRawPantry()
        {
            var ingredients = IngredientDefinition.CreateDefaultIngredients();
            var dict = new Dictionary<string, IngredientDefinition>();
            foreach (var ing in ingredients)
            {
                dict[ing.m_id] = ing;
            }

            return new List<RawIngredientData>
            {
                
                new RawIngredientData(
                    "raw_dark_beans",
                    "Whole Synthetic Dark Beans",
                    ApparatusType.CoffeeGrinder,
                    new Color(0.24f, 0.15f, 0.10f, 1f),
                    "Whole dense coffee beans. Needs precision burr grinding to release caffeine & roast tannins.",
                    dict["dark_roast"]
                ),
                new RawIngredientData(
                    "raw_light_beans",
                    "Whole Quantum Ethiopian Beans",
                    ApparatusType.CoffeeGrinder,
                    new Color(0.50f, 0.32f, 0.20f, 1f),
                    "Whole lightly roasted beans. Grinding releases bright chlorogenic acid and caffeine.",
                    dict["light_roast"]
                ),
                new RawIngredientData(
                    "raw_cocoa_nibs",
                    "Raw Cyber-Cacao Nibs",
                    ApparatusType.CoffeeGrinder,
                    new Color(0.38f, 0.20f, 0.15f, 1f),
                    "Hard cacao nibs. Grinding breaks them into aromatic cocoa powder.",
                    dict["raw_cocoa"]
                ),

                
                new RawIngredientData(
                    "raw_neuro_lotus",
                    "Fresh Neuro-Lotus Blossom",
                    ApparatusType.BotanicalMortar,
                    new Color(0.20f, 0.85f, 0.75f, 1f),
                    "[RAW CYBER-PLANT] Whole aquatic petals. Must be crushed in mortar to extract concentrated anxiolytic Neuro-Lotus.",
                    dict["neuro_lotus"]
                ),
                new RawIngredientData(
                    "raw_somna_moss",
                    "Fresh Nightfall Somna-Moss",
                    ApparatusType.BotanicalMortar,
                    new Color(0.35f, 0.25f, 0.75f, 1f),
                    "[RAW CYBER-PLANT] Subterranean bioluminescent moss. Pulverizing in mortar activates deep sleep Somnin.",
                    dict["somna_moss"]
                ),
                new RawIngredientData(
                    "raw_hyper_ginseng",
                    "Raw Hyper-Ginseng Root",
                    ApparatusType.BotanicalMortar,
                    new Color(0.95f, 0.30f, 0.35f, 1f),
                    "[RAW CYBER-PLANT] Fibrous cybernetic root. Requires heavy crushing into fine powder to release focus ginsenosides.",
                    dict["hyper_ginseng"]
                ),
                new RawIngredientData(
                    "raw_glitch_root",
                    "Wild Glitch-Root Blossom",
                    ApparatusType.BotanicalMortar,
                    new Color(0.95f, 0.40f, 0.85f, 1f),
                    "[RAW CYBER-PLANT] Rare wild flora. Pestle grinding releases pure euphoric endorphins.",
                    dict["glitch_flower"]
                ),
                new RawIngredientData(
                    "raw_nano_algae",
                    "Raw Emerald Nano-Algae",
                    ApparatusType.BotanicalMortar,
                    new Color(0.30f, 0.85f, 0.40f, 1f),
                    "[RAW CYBER-PLANT] Clustered microscopic algae colonies. Crushing releases cellular detox scavengers.",
                    dict["nano_algae"]
                ),

                
                new RawIngredientData(
                    "raw_whole_milk",
                    "Cold Whole Milk Flask",
                    ApparatusType.SteamFrother,
                    new Color(0.96f, 0.94f, 0.88f, 1f),
                    "Chilled un-aerated dairy. Steam wand frothing transforms it into silky micro-foam lipids.",
                    dict["reconstituted_milk"]
                ),
                new RawIngredientData(
                    "raw_oat_emulsion",
                    "Cold Barista Oat Base",
                    ApparatusType.SteamFrother,
                    new Color(0.91f, 0.84f, 0.72f, 1f),
                    "Raw plant emulsion. Frothing aerates smooth lipids and natural sweetness.",
                    dict["oat_emulsion"]
                ),

                
                new RawIngredientData(
                    "raw_cane_crystals",
                    "Raw Demerara Sugar Crystals",
                    ApparatusType.SolventKettle,
                    new Color(1.00f, 0.95f, 0.80f, 1f),
                    "Unrefined crystalline sucrose. Dissolving and tempering makes it immediately water-soluble.",
                    dict["cane_sugar"]
                ),
                new RawIngredientData(
                    "raw_distilled_h2o",
                    "Raw De-ionized Solvent",
                    ApparatusType.SolventKettle,
                    new Color(0.40f, 0.75f, 0.95f, 1f),
                    "Ambient purified solvent. Thermal kettle preparation optimizes extraction temperature.",
                    dict["distilled_solvent"]
                )
            };
        }
        #endregion
    }
}

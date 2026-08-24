//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MolecularBrewing.Runtime
//{
//    [Serializable]
//    public class RecipeMoleculeRequirement
//    {
//        #region Publics

//        public MoleculeType m_type;
//        public int m_minCount;

//        #endregion


//        #region Unity API
//        // Data model
//        #endregion


//        #region Main API

//        public RecipeMoleculeRequirement(MoleculeType type, int minCount)
//        {
//            m_type = type;
//            m_minCount = minCount;
//        }

//        #endregion


//        #region Tools and Utilities
//        #endregion


//        #region Private and Protected
//        #endregion
//    }

//    [Serializable]
//    public class RecipeBondRequirement
//    {
//        #region Publics

//        public MoleculeType m_typeA;
//        public MoleculeType m_typeB;

//        #endregion


//        #region Unity API
//        // Data model
//        #endregion


//        #region Main API

//        public RecipeBondRequirement(MoleculeType typeA, MoleculeType typeB)
//        {
//            m_typeA = typeA;
//            m_typeB = typeB;
//        }

//        public bool Matches(MoleculeType a, MoleculeType b)
//        {
//            return (m_typeA == a && m_typeB == b) || (m_typeA == b && m_typeB == a);
//        }

//        #endregion


//        #region Tools and Utilities
//        #endregion


//        #region Private and Protected
//        #endregion
//    }

//    [Serializable]
//    public class DrinkRecipePattern
//    {
//        #region Publics

//        public string m_id;
//        public string m_displayName;
//        public string m_tagline;
//        [TextArea] public string m_description;
//        public string m_hint;
//        public Color m_themeColor;
//        public bool m_isDiscoveredByDefault;

//        public List<RecipeMoleculeRequirement> m_requiredMolecules = new List<RecipeMoleculeRequirement>();
//        public List<RecipeBondRequirement> m_requiredBonds = new List<RecipeBondRequirement>();

//        [Header("Ideal Taste Profile")]
//        public float m_idealSweetness;
//        public float m_idealBitterness;
//        public float m_idealAcidity;
//        public float m_idealCreaminess;
//        public float m_idealCaffeine;
//        public float m_idealAroma;

//        #endregion


//        #region Unity API
//        // Data model
//        #endregion


//        #region Main API

//        public DrinkRecipePattern(
//            string id,
//            string displayName,
//            string tagline,
//            string description,
//            Color themeColor,
//            float sweetness,
//            float bitterness,
//            float acidity,
//            float creaminess,
//            float caffeine,
//            float aroma,
//            List<RecipeMoleculeRequirement> reqMolecules,
//            List<RecipeBondRequirement> reqBonds = null,
//            bool isDiscoveredByDefault = true,
//            string hint = "")
//        {
//            m_id = id;
//            m_displayName = displayName;
//            m_tagline = tagline;
//            m_description = description;
//            m_themeColor = themeColor;
//            m_idealSweetness = sweetness;
//            m_idealBitterness = bitterness;
//            m_idealAcidity = acidity;
//            m_idealCreaminess = creaminess;
//            m_idealCaffeine = caffeine;
//            m_idealAroma = aroma;
//            m_requiredMolecules = reqMolecules ?? new List<RecipeMoleculeRequirement>();
//            m_requiredBonds = reqBonds ?? new List<RecipeBondRequirement>();
//            m_isDiscoveredByDefault = isDiscoveredByDefault;
//            m_hint = hint;
//        }

//        #endregion


//        #region Tools and Utilities

//        public static List<DrinkRecipePattern> CreateDefaultRecipes()
//        {
//            return new List<DrinkRecipePattern>
//            {
//                new DrinkRecipePattern(
//                    "classic_latte",
//                    "Classic Café Latte",
//                    "Silky Espresso & Micro-Foam",
//                    "A timeless fusion of rich espresso solubles suspended in velvety steamed milk micro-foam.",
//                    new Color(0.88f, 0.72f, 0.55f, 1f),
//                    45f, 30f, 15f, 80f, 45f, 40f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Caffeine, 2),
//                        new RecipeMoleculeRequirement(MoleculeType.Lipid, 1),
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 1)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Caffeine, MoleculeType.Lipid)
//                    },
//                    isDiscoveredByDefault: true,
//                    hint: "Standard cafe formulation."
//                ),

//                new DrinkRecipePattern(
//                    "cyber_espresso",
//                    "Cyber-Espresso Doppio",
//                    "Double-Shot Neural Extract",
//                    "A dense, unyielding blast of concentrated dark roast caffeine for immediate cognitive overclocking.",
//                    new Color(0.96f, 0.52f, 0.15f, 1f),
//                    10f, 85f, 60f, 15f, 95f, 80f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Caffeine, 3),
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 1)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Caffeine, MoleculeType.Water)
//                    },
//                    isDiscoveredByDefault: true,
//                    hint: "Dense espresso concentrate."
//                ),
//                new DrinkRecipePattern(
//                    "neuro_calm_latte",
//                    "Neuro-Calm Lotus Latte",
//                    "Targeted Anxiolytic Cyber-Coffee",
//                    "A soothing medicinal latte infused with purified Neuro-Lotus alkaloids that dissolves cyber-anxiety and stabilizes tremors.",
//                    new Color(0.25f, 0.90f, 0.80f, 1f),
//                    40f, 20f, 10f, 75f, 30f, 85f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Caffeine, 2),
//                        new RecipeMoleculeRequirement(MoleculeType.Lipid, 1),
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 1),
//                        new RecipeMoleculeRequirement(MoleculeType.NeuroLotusExtract, 2)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Caffeine, MoleculeType.Lipid),
//                        new RecipeBondRequirement(MoleculeType.NeuroLotusExtract, MoleculeType.Water)
//                    },
//                    isDiscoveredByDefault: true,
//                    hint: "Anxiolytic lotus latte."
//                ),

//                new DrinkRecipePattern(
//                    "hyper_overclock_brew",
//                    "Hyper-Overclock Neural Brew",
//                    "Max-Capacity Synaptic Overdrive",
//                    "An extreme high-frequency coffee concoction that triples neural throughput and overclock clock speeds.",
//                    new Color(0.85f, 0.25f, 0.30f, 1f),
//                    15f, 90f, 70f, 20f, 100f, 90f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Caffeine, 3),
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 2),
//                        new RecipeMoleculeRequirement(MoleculeType.NeuroLotusExtract, 1)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Caffeine, MoleculeType.Caffeine),
//                        new RecipeBondRequirement(MoleculeType.Caffeine, MoleculeType.Water)
//                    },
//                    isDiscoveredByDefault: false,
//                    hint: "Link multiple high-voltage Caffeine molecules directly together with pure Water in the reaction chamber."
//                ),

//                new DrinkRecipePattern(
//                    "deep_sedation_elixir",
//                    "Deep Sedation Somnolent Elixir",
//                    "Bio-Stabilized Dream Inducer",
//                    "A velvety nighttime elixir designed to induce deep restorative delta-wave sleep and repair synaptic fatigue.",
//                    new Color(0.35f, 0.25f, 0.85f, 1f),
//                    50f, 10f, 5f, 85f, 5f, 90f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.NeuroLotusExtract, 3),
//                        new RecipeMoleculeRequirement(MoleculeType.Lipid, 2),
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 1)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.NeuroLotusExtract, MoleculeType.Lipid),
//                        new RecipeBondRequirement(MoleculeType.NeuroLotusExtract, MoleculeType.Water)
//                    },
//                    isDiscoveredByDefault: false,
//                    hint: "Fuse concentrated Neuro-Lotus alkaloids with dense velvet Lipids and pure Water."
//                ),

//                new DrinkRecipePattern(
//                    "euphoria_velvet_cloud",
//                    "Euphoria Velvet Cloud",
//                    "Sensory-Enhanced Glitch-Joy",
//                    "A dessert-grade molecular cloud beverage that floods neural pathways with intense warmth and euphoria.",
//                    new Color(0.92f, 0.35f, 0.78f, 1f),
//                    80f, 15f, 10f, 90f, 35f, 95f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Lipid, 2),
//                        new RecipeMoleculeRequirement(MoleculeType.Caffeine, 1),
//                        new RecipeMoleculeRequirement(MoleculeType.NeuroLotusExtract, 2)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Lipid, MoleculeType.Lipid),
//                        new RecipeBondRequirement(MoleculeType.NeuroLotusExtract, MoleculeType.Caffeine)
//                    },
//                    isDiscoveredByDefault: false,
//                    hint: "Form a dual-lipid cloud matrix bonded directly to Caffeine and Neuro-Lotus essence."
//                ),

//                new DrinkRecipePattern(
//                    "quantum_solvent_infusion",
//                    "Quantum Bio-Solvent Infusion",
//                    "Pure Cell-Purifying Tonic",
//                    "A crystal-clear medicinal tonic using pure solvent polarities to flush bio-toxins and refresh cognitive baseline.",
//                    new Color(0.20f, 0.75f, 0.98f, 1f),
//                    30f, 25f, 40f, 10f, 20f, 85f,
//                    new List<RecipeMoleculeRequirement>
//                    {
//                        new RecipeMoleculeRequirement(MoleculeType.Water, 3),
//                        new RecipeMoleculeRequirement(MoleculeType.NeuroLotusExtract, 2)
//                    },
//                    new List<RecipeBondRequirement>
//                    {
//                        new RecipeBondRequirement(MoleculeType.Water, MoleculeType.Water),
//                        new RecipeBondRequirement(MoleculeType.NeuroLotusExtract, MoleculeType.Water)
//                    },
//                    isDiscoveredByDefault: false,
//                    hint: "Chain multiple pure Water molecules together and anchor them to active Neuro-Lotus catalysts."
//                )
//            };
//        }

//        #endregion


//        #region Private and Protected
//        #endregion
//    }
//}

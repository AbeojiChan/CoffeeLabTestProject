using System;
using UnityEngine;

namespace MolecularBrewing.Runtime
{
    public enum MoleculeType
    {
        // --- Classic Coffee & Flavor Compounds ---
        Water,              // H2O - Base solvent & hydration
        Caffeine,           // C8H10N4O2 - Stimulant, sharpness & energy
        Sucrose,            // C12H22O11 - Sweetness & mouthfeel
        Lipid,              // Triglyceride - Creaminess, velvety body & emulsion
        ChlorogenicAcid,    // C16H18O9 - Crisp fruity acidity
        CaramelTannin,      // C6H6O3 - Deep roast bitterness & color
        CocoaFlavonoid,     // C15H14O6 - Deep chocolate aroma
        Cinnamaldehyde,     // C9H8O - Spiced warmth & aroma

        // --- Cyberpunk Medicinal & Neural Plant Molecules ---
        NeuroLotusExtract,  // C19H21NO4 - Powerful anxiolytic (Cures Anxiety & Calms Nerves)
        Somnin,             // C13H16N2O2 - Deep sleep inducer & neural sedative (Helps Sleep)
        HyperGinsenoside,   // C42H72O14 - Neural overclock & reflex acceleration (Hyper-Focus)
        GlitchEndorphin,    // C21H30O2 - Synthetic neurotransmitter trigger (Euphoria & Joy)
        NanoScavenger       // C30H48O3 - Bio-active cellular antioxidant (Cellular Detox & Cure)
    }

    [Serializable]
    public class MoleculeDefinition
    {
        #region Publics

        public MoleculeType m_type;
        public string m_displayName;
        public string m_formula;
        public Color m_primaryColor;
        public Color m_accentColor;
        [TextArea] public string m_description;

        [Header("Bio-Active / Medicinal Flag")]
        public bool m_isCyberPlant;
        public string m_medicinalEffect;

        [Header("Taste & Neural Impacts")]
        public float m_sweetnessImpact;
        public float m_bitternessImpact;
        public float m_acidityImpact;
        public float m_creaminessImpact;
        public float m_caffeineImpact;
        public float m_aromaImpact;
        public float m_neuralCalmImpact;
        public float m_sedationImpact;
        public float m_focusImpact;
        public float m_euphoriaImpact;

        #endregion


        #region Main API

        public MoleculeDefinition(
            MoleculeType type,
            string displayName,
            string formula,
            Color primaryColor,
            Color accentColor,
            string description,
            bool isCyberPlant,
            string medicinalEffect,
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
            m_type = type;
            m_displayName = displayName;
            m_formula = formula;
            m_primaryColor = primaryColor;
            m_accentColor = accentColor;
            m_description = description;
            m_isCyberPlant = isCyberPlant;
            m_medicinalEffect = medicinalEffect;
            m_sweetnessImpact = sweetness;
            m_bitternessImpact = bitterness;
            m_acidityImpact = acidity;
            m_creaminessImpact = creaminess;
            m_caffeineImpact = caffeine;
            m_aromaImpact = aroma;
            m_neuralCalmImpact = neuralCalm;
            m_sedationImpact = sedation;
            m_focusImpact = focus;
            m_euphoriaImpact = euphoria;
        }

        #endregion


        #region Tools and Utilities

        public static MoleculeDefinition CreateDefault(MoleculeType type)
        {
            switch (type)
            {
                case MoleculeType.Water:
                    return new MoleculeDefinition(
                        type, "Water", "H<sub>2</sub>O",
                        new Color(0.20f, 0.60f, 0.90f, 1f),
                        new Color(0.60f, 0.85f, 1.00f, 1f),
                        "Universal solvent. Hydrates and stabilizes extraction.",
                        false, "",
                        0f, 0f, 2f, 2f, 0f, 0f
                    );
                case MoleculeType.Caffeine:
                    return new MoleculeDefinition(
                        type, "Caffeine", "C<sub>8</sub>H<sub>10</sub>N<sub>4</sub>O<sub>2</sub>",
                        new Color(0.25f, 0.15f, 0.10f, 1f),
                        new Color(0.95f, 0.55f, 0.15f, 1f),
                        "Potent alkaloid stimulant offering sharp bitterness and neural wakefulness.",
                        false, "",
                        0f, 25f, 5f, 0f, 35f, 5f, 0f, 0f, 20f, 0f
                    );
                case MoleculeType.Sucrose:
                    return new MoleculeDefinition(
                        type, "Sucrose", "C<sub>12</sub>H<sub>22</sub>O<sub>11</sub>",
                        new Color(0.95f, 0.85f, 0.70f, 1f),
                        new Color(1.00f, 0.92f, 0.50f, 1f),
                        "Crystalline carbohydrate imparting clean sweetness and smooth body.",
                        false, "",
                        30f, -5f, -2f, 5f, 0f, 2f
                    );
                case MoleculeType.Lipid:
                    return new MoleculeDefinition(
                        type, "Lipids / Milk Fat", "C<sub>55</sub>H<sub>98</sub>O<sub>6</sub>",
                        new Color(0.95f, 0.90f, 0.80f, 1f),
                        new Color(0.98f, 0.80f, 0.40f, 1f),
                        "Micro-fats providing velvety micro-foam and smooth mouthfeel.",
                        false, "",
                        5f, -8f, -5f, 35f, 0f, 5f
                    );
                case MoleculeType.ChlorogenicAcid:
                    return new MoleculeDefinition(
                        type, "Chlorogenic Acid", "C<sub>16</sub>H<sub>18</sub>O<sub>9</sub>",
                        new Color(0.85f, 0.30f, 0.35f, 1f),
                        new Color(1.00f, 0.55f, 0.60f, 1f),
                        "Key organic acid delivering fruity brightness and crispness.",
                        false, "",
                        2f, 8f, 30f, 0f, 5f, 10f
                    );
                case MoleculeType.CaramelTannin:
                    return new MoleculeDefinition(
                        type, "Caramel Tannin", "C<sub>6</sub>H<sub>6</sub>O<sub>3</sub>",
                        new Color(0.55f, 0.28f, 0.10f, 1f),
                        new Color(0.85f, 0.50f, 0.20f, 1f),
                        "Roast reaction byproduct lending deep roasted aroma and amber color.",
                        false, "",
                        6f, 18f, 5f, 2f, 5f, 20f
                    );
                case MoleculeType.CocoaFlavonoid:
                    return new MoleculeDefinition(
                        type, "Cocoa Flavonoid", "C<sub>15</sub>H<sub>14</sub>O<sub>6</sub>",
                        new Color(0.40f, 0.20f, 0.15f, 1f),
                        new Color(0.70f, 0.45f, 0.35f, 1f),
                        "Rich cacao polyphenol imparting chocolate depth.",
                        false, "",
                        4f, 12f, 2f, 8f, 10f, 25f
                    );
                case MoleculeType.Cinnamaldehyde:
                    return new MoleculeDefinition(
                        type, "Cinnamaldehyde", "C<sub>9</sub>H<sub>8</sub>O",
                        new Color(0.80f, 0.40f, 0.15f, 1f),
                        new Color(1.00f, 0.65f, 0.30f, 1f),
                        "Spiced essential aldehyde offering warmth and sweet fragrance.",
                        false, "",
                        8f, 2f, 0f, 0f, 0f, 35f
                    );

                case MoleculeType.NeuroLotusExtract:
                    return new MoleculeDefinition(
                        type, "Neuro-Lotus (Anxiolytic)", "C<sub>19</sub>H<sub>21</sub>NO<sub>4</sub>",
                        new Color(0.20f, 0.85f, 0.75f, 1f),
                        new Color(0.60f, 1.00f, 0.90f, 1f),
                        "Bio-engineered alkaloid that regulates cortisol and calms chronic neural anxiety.",
                        true, "Cures Anxiety / Calms Nerves",
                        5f, -5f, 0f, 10f, -10f, 25f,
                        neuralCalm: 45f, sedation: 10f, focus: 5f, euphoria: 10f
                    );
                case MoleculeType.Somnin:
                    return new MoleculeDefinition(
                        type, "Somnin (Sedative)", "C<sub>13</sub>H<sub>16</sub>N<sub>2</sub>O<sub>2</sub>",
                        new Color(0.35f, 0.25f, 0.75f, 1f),
                        new Color(0.70f, 0.60f, 1.00f, 1f),
                        "Slows synaptic firing to induce deep, restful circadian sleep.",
                        true, "Induces Deep Sleep / Sedative",
                        0f, 5f, -5f, 15f, -30f, 15f,
                        neuralCalm: 35f, sedation: 50f, focus: -15f, euphoria: 5f
                    );
                case MoleculeType.HyperGinsenoside:
                    return new MoleculeDefinition(
                        type, "Hyper-Ginsenoside", "C<sub>42</sub>H<sub>72</sub>O<sub>14</sub>",
                        new Color(0.95f, 0.25f, 0.35f, 1f),
                        new Color(1.00f, 0.60f, 0.70f, 1f),
                        "Cyber-enhanced adaptogen accelerating neural synapse velocity and reaction time.",
                        true, "Neural Overclock / Hyper-Focus",
                        -5f, 15f, 10f, 0f, 25f, 20f,
                        neuralCalm: -10f, sedation: -25f, focus: 50f, euphoria: 15f
                    );
                case MoleculeType.GlitchEndorphin:
                    return new MoleculeDefinition(
                        type, "Glitch-Endorphin", "C<sub>21</sub>H<sub>30</sub>O<sub>2</sub>",
                        new Color(0.95f, 0.40f, 0.85f, 1f),
                        new Color(1.00f, 0.75f, 0.95f, 1f),
                        "Euphoriant synthesized from wild cyber-flora. Elevates mood and triggers warmth.",
                        true, "Euphoria / Anti-Depressive",
                        15f, -5f, 5f, 5f, 10f, 30f,
                        neuralCalm: 20f, sedation: 0f, focus: 15f, euphoria: 55f
                    );
                case MoleculeType.NanoScavenger:
                default:
                    return new MoleculeDefinition(
                        type, "Nano-Scavenger", "C<sub>30</sub>H<sub>48</sub>O<sub>3</sub>",
                        new Color(0.30f, 0.80f, 0.35f, 1f),
                        new Color(0.65f, 1.00f, 0.70f, 1f),
                        "Microscopic plant enzyme that neutralizes toxic metabolic residues and purges impurities.",
                        true, "Cellular Detox / Hangover Cure",
                        0f, 5f, 15f, 5f, 0f, 15f,
                        neuralCalm: 20f, sedation: 5f, focus: 10f, euphoria: 10f
                    );
            }
        }

        #endregion
    }
}

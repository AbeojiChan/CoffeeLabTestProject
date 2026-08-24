//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace MolecularBrewing.Runtime
//{
//    [Serializable]
//    public class DrinkEvaluationResult
//    {
//        #region Publics
//        public string m_drinkName;
//        public string m_subtitle;
//        public string m_rank;
//        public float m_matchScore;
//        public Color m_primaryColor;
//        public TasteStats m_stats;
//        public DrinkRecipePattern m_matchedRecipe;
//        public List<SpecialEffectDefinition> m_unlockedEffects = new List<SpecialEffectDefinition>();
//        [TextArea] public string m_baristaNotes;
//        public List<string> m_highlightedTags = new List<string>();

//        #endregion
//    }

//    public static class DrinkEvaluator
//    {
//        #region Main API

//        public static DrinkEvaluationResult Evaluate(
//            IReadOnlyList<MoleculeType> placedMolecules,
//            IReadOnlyList<(MoleculeType a, MoleculeType b)> activeBonds,
//            IReadOnlyList<DrinkRecipePattern> recipeDatabase,
//            IReadOnlyList<SpecialEffectDefinition> effectDatabase,
//            Func<MoleculeType, MoleculeDefinition> defLookup)
//        {
//            var stats = TasteStats.Calculate(placedMolecules, activeBonds, defLookup);

//            if (placedMolecules == null || placedMolecules.Count == 0)
//            {
//                return new DrinkEvaluationResult
//                {
//                    m_drinkName = "Empty Crucible",
//                    m_subtitle = "No molecules in synthesis chamber",
//                    m_rank = "F",
//                    m_matchScore = 0f,
//                    m_primaryColor = Color.gray,
//                    m_stats = stats,
//                    m_baristaNotes = "The microscopic chamber was empty. No drink was synthesized.",
//                    m_highlightedTags = new List<string> { "[Empty]" }
//                };
//            }
//            DrinkRecipePattern bestMatch = null;
//            float bestScore = -1f;

//            if (recipeDatabase != null)
//            {
//                foreach (var recipe in recipeDatabase)
//                {
//                    float score = CalculateRecipeScore(placedMolecules, activeBonds, stats, recipe);
//                    if (score > bestScore && score >= 45f)
//                    {
//                        bestScore = score;
//                        bestMatch = recipe;
//                    }
//                }
//            }
//            List<SpecialEffectDefinition> unlockedEffects = new List<SpecialEffectDefinition>();
//            var counts = placedMolecules.GroupBy(m => m).ToDictionary(g => g.Key, g => g.Count());

//            if (effectDatabase != null)
//            {
//                foreach (var eff in effectDatabase)
//                {
//                    if (counts.TryGetValue(eff.m_requiredMolecule, out int count) && count >= eff.m_minCount)
//                    {
//                        bool bondSatisfied = !eff.m_requiresBond;
//                        if (eff.m_requiresBond && activeBonds != null)
//                        {
//                            bondSatisfied = activeBonds.Any(b =>
//                                (b.a == eff.m_requiredMolecule && b.b == eff.m_requiredBondWith) ||
//                                (b.a == eff.m_requiredBondWith && b.b == eff.m_requiredMolecule));
//                        }

//                        if (bondSatisfied)
//                        {
//                            unlockedEffects.Add(eff);
//                        }
//                    }
//                }
//            }

//            string drinkName;
//            string subtitle;
//            Color themeColor;
//            string rank;
//            List<string> tags = new List<string>();

//            if (bestMatch != null)
//            {
//                string prefix = GetDynamicPrefix(stats, unlockedEffects);
//                drinkName = string.IsNullOrEmpty(prefix) ? bestMatch.m_displayName : $"{prefix} {bestMatch.m_displayName}";
//                subtitle = bestMatch.m_tagline;
//                themeColor = bestMatch.m_themeColor;
//                rank = GetRankFromScore(bestScore);
//            }
//            else
//            {
//                bestScore = 65f;
//                drinkName = GetEmergentDrinkName(stats, unlockedEffects);
//                subtitle = "Custom Neural Infusion";
//                themeColor = unlockedEffects.Count > 0 ? unlockedEffects[0].m_badgeColor : new Color(0.6f, 0.45f, 0.35f, 1f);
//                rank = "A";
//            }
//            foreach (var eff in unlockedEffects)
//            {
//                tags.Add($"[{eff.m_displayName}]");
//            }

//            if (stats.m_caffeine >= 70f) tags.Add("[High Voltage]");
//            if (stats.m_sweetness >= 65f) tags.Add("[Sweet Nectar]");
//            if (stats.m_creaminess >= 65f) tags.Add("[Silky Microfoam]");
//            if (stats.m_aroma >= 60f) tags.Add("[Aromatic Bouquet]");

//            string baristaNotes = GenerateBaristaCritique(drinkName, stats, rank, bestMatch, unlockedEffects);

//            return new DrinkEvaluationResult
//            {
//                m_drinkName = drinkName,
//                m_subtitle = subtitle,
//                m_rank = rank,
//                m_matchScore = Mathf.Round(bestScore),
//                m_primaryColor = themeColor,
//                m_stats = stats,
//                m_matchedRecipe = bestMatch,
//                m_unlockedEffects = unlockedEffects,
//                m_baristaNotes = baristaNotes,
//                m_highlightedTags = tags
//            };
//        }

//        #endregion


//        #region Tools and Utilities

//        private static float CalculateRecipeScore(
//            IReadOnlyList<MoleculeType> placedMolecules,
//            IReadOnlyList<(MoleculeType a, MoleculeType b)> activeBonds,
//            TasteStats stats,
//            DrinkRecipePattern recipe)
//        {
//            float score = 0f;
//            var counts = placedMolecules.GroupBy(m => m).ToDictionary(g => g.Key, g => g.Count());
//            float moleculeRatioSum = 0f;
//            foreach (var req in recipe.m_requiredMolecules)
//            {
//                if (counts.TryGetValue(req.m_type, out int count) && count >= req.m_minCount)
//                {
//                    moleculeRatioSum += 1f;
//                }
//            }
//            float moleculeScore = recipe.m_requiredMolecules.Count > 0 ? (moleculeRatioSum / recipe.m_requiredMolecules.Count) * 40f : 40f;
//            score += moleculeScore;

//            if (recipe.m_requiredMolecules.Count > 0 && moleculeRatioSum < recipe.m_requiredMolecules.Count)
//                return 0f;
//            if (recipe.m_requiredBonds != null && recipe.m_requiredBonds.Count > 0)
//            {
//                int matchedBonds = 0;
//                foreach (var reqBond in recipe.m_requiredBonds)
//                {
//                    bool found = activeBonds != null && activeBonds.Any(b => reqBond.Matches(b.a, b.b));
//                    if (found) matchedBonds++;
//                }
//                score += ((float)matchedBonds / recipe.m_requiredBonds.Count) * 20f;
//            }
//            else
//            {
//                score += 20f;
//            }
//            float diff = 0f;
//            diff += Mathf.Abs(stats.m_sweetness - recipe.m_idealSweetness);
//            diff += Mathf.Abs(stats.m_bitterness - recipe.m_idealBitterness);
//            diff += Mathf.Abs(stats.m_acidity - recipe.m_idealAcidity);
//            diff += Mathf.Abs(stats.m_creaminess - recipe.m_idealCreaminess);
//            diff += Mathf.Abs(stats.m_caffeine - recipe.m_idealCaffeine);
//            diff += Mathf.Abs(stats.m_aroma - recipe.m_idealAroma);

//            float avgDiff = diff / 6f;
//            float statScore = Mathf.Clamp(40f - (avgDiff * 0.8f), 0f, 40f);
//            score += statScore;

//            return Mathf.Clamp(score, 0f, 100f);
//        }

//        private static string GetDynamicPrefix(TasteStats stats, List<SpecialEffectDefinition> effects)
//        {
//            if (effects.Any(e => e.m_type == SpecialEffectType.AnxietyCure)) return "Anxiolytic";
//            if (effects.Any(e => e.m_type == SpecialEffectType.DeepSleep)) return "Somnolent";
//            if (effects.Any(e => e.m_type == SpecialEffectType.NeuralOverclock)) return "Overclocked";
//            if (effects.Any(e => e.m_type == SpecialEffectType.Euphoria)) return "Euphoric";
//            if (effects.Any(e => e.m_type == SpecialEffectType.CellularDetox)) return "Bio-Scavenged";

//            if (stats.m_sweetness >= 70f && stats.m_creaminess >= 65f) return "Velvety Sweet";
//            if (stats.m_caffeine >= 80f) return "Hyper-Charged";
//            if (stats.m_aroma >= 70f) return "Aromatic";
//            if (stats.m_bitterness >= 75f) return "Obsidian Dark";
//            return "";
//        }

//        private static string GetEmergentDrinkName(TasteStats stats, List<SpecialEffectDefinition> effects)
//        {
//            if (effects.Any(e => e.m_type == SpecialEffectType.AnxietyCure)) return "Neuro-Lotus Calm Elixir";
//            if (effects.Any(e => e.m_type == SpecialEffectType.DeepSleep)) return "Somna Dream Catalyst";
//            if (effects.Any(e => e.m_type == SpecialEffectType.NeuralOverclock)) return "Synaptic Reflex Infusion";
//            if (effects.Any(e => e.m_type == SpecialEffectType.Euphoria)) return "Neon Endorphin Cascade";
//            if (effects.Any(e => e.m_type == SpecialEffectType.CellularDetox)) return "Nano-Detox Rejuvenator";

//            if (stats.m_sweetness > 60f) return "Sweet Neural Nectar";
//            if (stats.m_caffeine > 60f) return "Kinetic Energy Compound";
//            return "Synthesized Molecular Blend #" + UnityEngine.Random.Range(101, 999);
//        }

//        private static string GetRankFromScore(float score)
//        {
//            if (score >= 90f) return "S";
//            if (score >= 80f) return "A";
//            if (score >= 65f) return "B";
//            if (score >= 50f) return "C";
//            return "D";
//        }

//        private static string GenerateBaristaCritique(
//            string name,
//            TasteStats stats,
//            string rank,
//            DrinkRecipePattern recipe,
//            List<SpecialEffectDefinition> effects)
//        {
//            string critique = $"\"{name}\" - ";
//            if (effects.Count > 0)
//            {
//                critique += $"Active Medical/Neural Effect Unlocked: {effects[0].m_displayName}. {effects[0].m_description} ";
//            }

//            if (rank == "S")
//            {
//                critique += "An alchemical masterpiece! Perfect molecular bonding and potent extraction balance.";
//            }
//            else if (rank == "A")
//            {
//                critique += "Excellently synthesized! Clean extraction with rich mouthfeel and active bio-compounds.";
//            }
//            else
//            {
//                critique += "A functional molecular compound. Fine-tuning the linkage bonds could elevate potency.";
//            }

//            critique += $"\n\nFlavor Profile: {stats.GetPrimaryFlavorDescription()}";
//            return critique;
//        }
//        #endregion
//    }
//}

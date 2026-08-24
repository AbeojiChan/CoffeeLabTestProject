//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MolecularBrewing.Runtime
//{
//    [Serializable]
//    public class ShopItemData
//    {
//        #region Publics

//        public string m_itemId;
//        public string m_displayName;
//        public string m_description;
//        public string m_categoryTag;
//        public int m_price;
//        public Color m_itemColor;
//        public IngredientDefinition m_preparedIngredient;
//        public RawIngredientData m_rawIngredient;
//        public int m_stockCount;

//        #endregion


//        #region Unity API
//        #endregion


//        #region Main API

//        public static List<ShopItemData> CreateDefaultShopCatalog()
//        {
//            var rawPantry = RawIngredientData.CreateDefaultRawPantry();
//            var rawCoffee = rawPantry.Find(r => r.m_rawId == "raw_beans");
//            var rawLotus = rawPantry.Find(r => r.m_rawId == "raw_lotus");
//            var rawMilk = rawPantry.Find(r => r.m_rawId == "raw_milk");
//            var rawSolvent = rawPantry.Find(r => r.m_rawId == "raw_solvent");

//            var preparedList = IngredientDefinition.CreateDefaultIngredients();
//            var prepEspresso = preparedList.Find(i => i.m_id == "espresso_shot");
//            var prepMilk = preparedList.Find(i => i.m_id == "steamed_milk");
//            var prepWater = preparedList.Find(i => i.m_id == "pure_water");
//            var prepLotus = preparedList.Find(i => i.m_id == "lotus_extract");

//            return new List<ShopItemData>
//            {
//                new ShopItemData
//                {
//                    m_itemId = "shop_raw_beans",
//                    m_displayName = "Ethiopian Cyber-Beans",
//                    m_description = "High-altitude beans with double alkaloid concentration. Grind in Preparation Lab.",
//                    m_categoryTag = "RAW BEANS",
//                    m_price = 180,
//                    m_itemColor = new Color(0.96f, 0.62f, 0.15f, 1f),
//                    m_rawIngredient = rawCoffee,
//                    m_preparedIngredient = prepEspresso,
//                    m_stockCount = 99
//                },
//                new ShopItemData
//                {
//                    m_itemId = "shop_raw_lotus",
//                    m_displayName = "Bio-Lotus Blossoms",
//                    m_description = "Fresh hydroponic medicinal flora. Crush in Botanical Mortar for anxiolytic lotus powder.",
//                    m_categoryTag = "MEDICINAL FLORA",
//                    m_price = 320,
//                    m_itemColor = new Color(0.18f, 0.82f, 0.55f, 1f),
//                    m_rawIngredient = rawLotus,
//                    m_preparedIngredient = prepLotus,
//                    m_stockCount = 99
//                },
//                new ShopItemData
//                {
//                    m_itemId = "shop_raw_milk",
//                    m_displayName = "Velvet Micro-Foam Jug",
//                    m_description = "Dense dairy emulsion for smooth covalent fatty acid suspension. Aerate in Steam Frother.",
//                    m_categoryTag = "DAIRY EMULSION",
//                    m_price = 150,
//                    m_itemColor = new Color(0.92f, 0.95f, 0.98f, 1f),
//                    m_rawIngredient = rawMilk,
//                    m_preparedIngredient = prepMilk,
//                    m_stockCount = 99
//                },
//                new ShopItemData
//                {
//                    m_itemId = "shop_pure_water",
//                    m_displayName = "Deionized Distilled H2O",
//                    m_description = "Triple-filtered molecular solvent base. Ready to dose directly into synthesis beaker.",
//                    m_categoryTag = "SOLVENT BASE",
//                    m_price = 90,
//                    m_itemColor = new Color(0.25f, 0.78f, 0.98f, 1f),
//                    m_rawIngredient = rawSolvent,
//                    m_preparedIngredient = prepWater,
//                    m_stockCount = 99
//                },
//                new ShopItemData
//                {
//                    m_itemId = "shop_synth_espresso",
//                    m_displayName = "Instant Pre-Pulled Espresso",
//                    m_description = "Pre-extracted dense dark espresso concentrate for instant workbench dosing.",
//                    m_categoryTag = "READY REAGENT",
//                    m_price = 260,
//                    m_itemColor = new Color(0.85f, 0.45f, 0.12f, 1f),
//                    m_preparedIngredient = prepEspresso,
//                    m_stockCount = 99
//                },
//                new ShopItemData
//                {
//                    m_itemId = "shop_neuro_powder",
//                    m_displayName = "Purified Lotus Crystal Pack",
//                    m_description = "Pre-refined ultra-pure anxiolytic crystal powder. Directly dosable into beaker.",
//                    m_categoryTag = "MEDICINAL ESSENCE",
//                    m_price = 450,
//                    m_itemColor = new Color(0.35f, 0.95f, 0.75f, 1f),
//                    m_preparedIngredient = prepLotus,
//                    m_stockCount = 99
//                }
//            };
//        }

//        #endregion


//        #region Tools and Utilities
//        #endregion


//        #region Private and Protected
//        #endregion
//    }
//}

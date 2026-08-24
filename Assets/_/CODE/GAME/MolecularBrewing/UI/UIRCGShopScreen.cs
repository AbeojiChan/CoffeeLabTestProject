//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    public class UIRCGShopScreen : MonoBehaviour
//    {
//        #region Publics

//        public int m_playerCredits = 5000;
//        public event Action<ShopItemData, int> OnItemPurchased;
//        public event Action OnReturnToWorkbenchRequested;
//        public event Action OnOpenMapRequested;

//        #endregion


//        #region Unity API

//        private void Update()
//        {
//            if (gameObject.activeSelf && _shopkeeperAvatarRing != null)
//            {
//                float bob = 1f + Mathf.Sin(Time.time * 4f) * 0.05f;
//                _shopkeeperAvatarRing.transform.localScale = new Vector3(bob, bob, 1f);
//            }
//        }

//        #endregion


//        #region Main API

//        public void Initialize(int startingCredits = 5000)
//        {
//            m_playerCredits = startingCredits;
//            _shopCatalog = ShopItemData.CreateDefaultShopCatalog();

//            if (!HasSerializedBindings())
//            {
//                BuildUI();
//            }
//            else
//            {
//                BindExistingHierarchy();
//            }

//            gameObject.SetActive(false);
//        }

//        public void OpenShop(int currentCredits)
//        {
//            m_playerCredits = currentCredits;
//            UpdateCreditDisplay();
//            gameObject.SetActive(true);
//            transform.SetAsLastSibling();

//            SetShopkeeperSpeech("Yo Barista! Welcome to Jojo le Barjo's Humble Abode! Com'Ere 'n Hav' a look! 'N r'member! All sales are final!");
//            PopulateCatalog();
//        }

//        #endregion


//        #region Tools and Utilities

//        private void BuildUI()
//        {
//            RectTransform rootRt = GetComponent<RectTransform>();
//            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
//            rootRt.anchorMin = Vector2.zero;
//            rootRt.anchorMax = Vector2.one;
//            rootRt.offsetMin = Vector2.zero;
//            rootRt.offsetMax = Vector2.zero;

//            GameObject bgObj = UIStyleHelper.CreatePanel(transform, "RCGBg", new Color(0.10f, 0.08f, 0.16f, 1f));
//            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
//            bgRt.anchorMin = Vector2.zero;
//            bgRt.anchorMax = Vector2.one;
//            bgRt.offsetMin = Vector2.zero;
//            bgRt.offsetMax = Vector2.zero;

//            GameObject headerBanner = UIStyleHelper.CreatePanel(transform, "HeaderBanner", new Color(0.85f, 0.18f, 0.55f, 0.98f));
//            RectTransform hbRt = headerBanner.GetComponent<RectTransform>();
//            hbRt.anchorMin = new Vector2(0f, 0.91f);
//            hbRt.anchorMax = new Vector2(1f, 1f);
//            hbRt.offsetMin = new Vector2(12, 6);
//            hbRt.offsetMax = new Vector2(-12, -6);

//            UIStyleHelper.CreateText(headerBanner.transform, "ShopName", "[JOJO LE BARJO'S SHOP]", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform snRt = headerBanner.transform.Find("ShopName").GetComponent<RectTransform>();
//            snRt.anchorMin = new Vector2(0.02f, 0.1f);
//            snRt.anchorMax = new Vector2(0.60f, 0.9f);
//            snRt.offsetMin = Vector2.zero;
//            snRt.offsetMax = Vector2.zero;

//            GameObject creditBadge = UIStyleHelper.CreatePanel(headerBanner.transform, "CreditBadge", new Color(0.98f, 0.78f, 0.15f, 1f));
//            RectTransform cbRt = creditBadge.GetComponent<RectTransform>();
//            cbRt.anchorMin = new Vector2(0.68f, 0.12f);
//            cbRt.anchorMax = new Vector2(0.98f, 0.88f);
//            cbRt.offsetMin = Vector2.zero;
//            cbRt.offsetMax = Vector2.zero;

//            _creditText = UIStyleHelper.CreateText(creditBadge.transform, "CreditText", $"CREDITS: {m_playerCredits:N0}", 13, new Color(0.12f, 0.08f, 0.20f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform ctRt = _creditText.rectTransform;
//            ctRt.anchorMin = Vector2.zero;
//            ctRt.anchorMax = Vector2.one;
//            ctRt.offsetMin = Vector2.zero;
//            ctRt.offsetMax = Vector2.zero;

//            GameObject shopkeeperPanel = UIStyleHelper.CreatePanel(transform, "ShopkeeperPanel", new Color(0.16f, 0.12f, 0.25f, 0.96f));
//            RectTransform spRt = shopkeeperPanel.GetComponent<RectTransform>();
//            spRt.anchorMin = new Vector2(0.02f, 0.11f);
//            spRt.anchorMax = new Vector2(0.30f, 0.89f);
//            spRt.offsetMin = Vector2.zero;
//            spRt.offsetMax = Vector2.zero;

//            GameObject avatarBox = UIStyleHelper.CreatePanel(shopkeeperPanel.transform, "AvatarBox", new Color(0.24f, 0.18f, 0.36f, 0.95f));
//            RectTransform abRt = avatarBox.GetComponent<RectTransform>();
//            abRt.anchorMin = new Vector2(0.08f, 0.52f);
//            abRt.anchorMax = new Vector2(0.92f, 0.95f);
//            abRt.offsetMin = Vector2.zero;
//            abRt.offsetMax = Vector2.zero;

//            _shopkeeperAvatarRing = UIStyleHelper.CreatePanel(avatarBox.transform, "AvatarRing", new Color(0.18f, 0.85f, 0.65f, 1f), UIStyleHelper.CircleSprite);
//            RectTransform arRt = _shopkeeperAvatarRing.GetComponent<RectTransform>();
//            arRt.anchorMin = new Vector2(0.20f, 0.20f);
//            arRt.anchorMax = new Vector2(0.80f, 0.85f);
//            arRt.offsetMin = Vector2.zero;
//            arRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(avatarBox.transform, "AvatarIcon", "[JOJO]", 18, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject nameBadge = UIStyleHelper.CreatePanel(avatarBox.transform, "NameBadge", new Color(0.85f, 0.18f, 0.55f, 1f));
//            RectTransform nbRt = nameBadge.GetComponent<RectTransform>();
//            nbRt.anchorMin = new Vector2(0.05f, 0.04f);
//            nbRt.anchorMax = new Vector2(0.95f, 0.20f);
//            nbRt.offsetMin = Vector2.zero;
//            nbRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(nameBadge.transform, "Name", "JOJO LE BARJO [ALCHEMIST]", 9, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject speechBubble = UIStyleHelper.CreatePanel(shopkeeperPanel.transform, "SpeechBubble", new Color(0.12f, 0.09f, 0.20f, 0.98f));
//            RectTransform sbRt = speechBubble.GetComponent<RectTransform>();
//            sbRt.anchorMin = new Vector2(0.06f, 0.06f);
//            sbRt.anchorMax = new Vector2(0.94f, 0.48f);
//            sbRt.offsetMin = Vector2.zero;
//            sbRt.offsetMax = Vector2.zero;

//            _speechText = UIStyleHelper.CreateText(speechBubble.transform, "Speech", "Yo Barista! Welcome to Jojo le Barjo's Apothecary!", 16, new Color(0.95f, 0.92f, 1f, 1f), TextAlignmentOptions.TopLeft, FontStyles.Normal);
//            RectTransform stRt = _speechText.rectTransform;
//            stRt.anchorMin = new Vector2(0.06f, 0.06f);
//            stRt.anchorMax = new Vector2(0.94f, 0.94f);
//            stRt.offsetMin = Vector2.zero;
//            stRt.offsetMax = Vector2.zero;

//            GameObject catalogPanel = UIStyleHelper.CreatePanel(transform, "CatalogPanel", new Color(0.14f, 0.11f, 0.22f, 0.96f));
//            RectTransform cpRt = catalogPanel.GetComponent<RectTransform>();
//            cpRt.anchorMin = new Vector2(0.32f, 0.11f);
//            cpRt.anchorMax = new Vector2(0.98f, 0.89f);
//            cpRt.offsetMin = Vector2.zero;
//            cpRt.offsetMax = Vector2.zero;

//            GameObject titleObj = UIStyleHelper.CreateText(catalogPanel.transform, "CatalogTitle", "AVAILABLE REAGENT SUPPLIES • SELECT QUANTITY & BUY", 13, new Color(0.18f, 0.85f, 0.65f, 1f), TextAlignmentOptions.Left, FontStyles.Bold).gameObject;
//            RectTransform ttlRt = titleObj.GetComponent<RectTransform>();
//            ttlRt.anchorMin = new Vector2(0.02f, 0.94f);
//            ttlRt.anchorMax = new Vector2(0.98f, 0.99f);
//            ttlRt.offsetMin = Vector2.zero;
//            ttlRt.offsetMax = Vector2.zero;

//            GameObject gridObj = new GameObject("CatalogGrid", typeof(RectTransform), typeof(GridLayoutGroup));
//            gridObj.transform.SetParent(catalogPanel.transform, false);
//            _catalogContainer = gridObj.transform;
//            RectTransform cgRt = gridObj.GetComponent<RectTransform>();
//            cgRt.anchorMin = new Vector2(0.02f, 0.02f);
//            cgRt.anchorMax = new Vector2(0.98f, 0.93f);
//            cgRt.offsetMin = Vector2.zero;
//            cgRt.offsetMax = Vector2.zero;

//            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
//            glg.cellSize = new Vector2(585, 210);
//            glg.spacing = new Vector2(16, 16);
//            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
//            glg.constraintCount = 2;

//            Button mapBtn = UIStyleHelper.CreateButton(transform, "MapBtn", "< CITY MAP", new Color(0.25f, 0.20f, 0.35f, 1f), Color.white, () =>
//            {
//                gameObject.SetActive(false);
//                OnOpenMapRequested?.Invoke();
//            });
//            RectTransform mbRt = mapBtn.GetComponent<RectTransform>();
//            mbRt.anchorMin = new Vector2(0.02f, 0.03f);
//            mbRt.anchorMax = new Vector2(0.18f, 0.09f);
//            mbRt.offsetMin = Vector2.zero;
//            mbRt.offsetMax = Vector2.zero;

//            Button returnBtn = UIStyleHelper.CreateButton(transform, "ReturnBtn", "CHECKOUT & RETURN TO WORKBENCH >", new Color(0.15f, 0.75f, 0.50f, 1f), Color.white, () =>
//            {
//                gameObject.SetActive(false);
//                OnReturnToWorkbenchRequested?.Invoke();
//            });
//            RectTransform rbRt = returnBtn.GetComponent<RectTransform>();
//            rbRt.anchorMin = new Vector2(0.64f, 0.03f);
//            rbRt.anchorMax = new Vector2(0.98f, 0.09f);
//            rbRt.offsetMin = Vector2.zero;
//            rbRt.offsetMax = Vector2.zero;
//        }

//        private void PopulateCatalog()
//        {
//            foreach (Transform child in _catalogContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            for (int i = 0; i < _shopCatalog.Count; i++)
//            {
//                var item = _shopCatalog[i];
//                CreateShopItemCard(item);
//            }
//        }

//        private void CreateShopItemCard(ShopItemData item)
//        {
//            GameObject card = UIStyleHelper.CreatePanel(_catalogContainer, $"Card_{item.m_itemId}", new Color(0.20f, 0.16f, 0.30f, 0.95f));

//            // Left icon pip
//            GameObject pip = UIStyleHelper.CreatePanel(card.transform, "Pip", item.m_itemColor, UIStyleHelper.CircleSprite);
//            RectTransform pRt = pip.GetComponent<RectTransform>();
//            pRt.anchorMin = new Vector2(0.03f, 0.28f);
//            pRt.anchorMax = new Vector2(0.16f, 0.72f);
//            pRt.offsetMin = Vector2.zero;
//            pRt.offsetMax = Vector2.zero;

//            // Category tag
//            TextMeshProUGUI tagTxt = UIStyleHelper.CreateText(card.transform, "Tag", $"[{item.m_categoryTag}]", 12, new Color(0.18f, 0.85f, 0.65f, 1f), TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform tgRt = tagTxt.rectTransform;
//            tgRt.anchorMin = new Vector2(0.18f, 0.78f);
//            tgRt.anchorMax = new Vector2(0.68f, 0.94f);
//            tgRt.offsetMin = Vector2.zero;
//            tgRt.offsetMax = Vector2.zero;

//            // Item Name
//            TextMeshProUGUI titleTxt = UIStyleHelper.CreateText(card.transform, "Title", item.m_displayName, 17, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform tRt = titleTxt.rectTransform;
//            tRt.anchorMin = new Vector2(0.18f, 0.52f);
//            tRt.anchorMax = new Vector2(0.68f, 0.78f);
//            tRt.offsetMin = Vector2.zero;
//            tRt.offsetMax = Vector2.zero;

//            // Description
//            TextMeshProUGUI descTxt = UIStyleHelper.CreateText(card.transform, "Desc", item.m_description, 13, new Color(0.78f, 0.75f, 0.88f, 0.9f), TextAlignmentOptions.Left, FontStyles.Normal);
//            RectTransform dRt = descTxt.rectTransform;
//            dRt.anchorMin = new Vector2(0.18f, 0.08f);
//            dRt.anchorMax = new Vector2(0.68f, 0.50f);
//            dRt.offsetMin = Vector2.zero;
//            dRt.offsetMax = Vector2.zero;

//            // Price & Buy Button Box (Right side)
//            GameObject buyBox = UIStyleHelper.CreatePanel(card.transform, "BuyBox", new Color(0.14f, 0.10f, 0.22f, 0.95f));
//            RectTransform bbRt = buyBox.GetComponent<RectTransform>();
//            bbRt.anchorMin = new Vector2(0.70f, 0.08f);
//            bbRt.anchorMax = new Vector2(0.97f, 0.92f);
//            bbRt.offsetMin = Vector2.zero;
//            bbRt.offsetMax = Vector2.zero;

//            TextMeshProUGUI priceTxt = UIStyleHelper.CreateText(buyBox.transform, "Price", $"{item.m_price} Credits", 16, new Color(0.98f, 0.78f, 0.15f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform prRt = priceTxt.rectTransform;
//            prRt.anchorMin = new Vector2(0f, 0.54f);
//            prRt.anchorMax = new Vector2(1f, 0.95f);
//            prRt.offsetMin = Vector2.zero;
//            prRt.offsetMax = Vector2.zero;

//            Button buyBtn = UIStyleHelper.CreateButton(buyBox.transform, "BuyBtn", "+ BUY 1", new Color(0.85f, 0.18f, 0.55f, 1f), Color.white, () => AttemptPurchase(item, 1));
//            RectTransform bRt = buyBtn.GetComponent<RectTransform>();
//            bRt.anchorMin = new Vector2(0.06f, 0.08f);
//            bRt.anchorMax = new Vector2(0.94f, 0.50f);
//            bRt.offsetMin = Vector2.zero;
//            bRt.offsetMax = Vector2.zero;
//            buyBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;
//        }

//        private void AttemptPurchase(ShopItemData item, int qty)
//        {
//            int totalCost = item.m_price * qty;
//            if (m_playerCredits >= totalCost)
//            {
//                m_playerCredits -= totalCost;
//                UpdateCreditDisplay();

//                SetShopkeeperSpeech($"<color=#10B981>+ Purchased {qty}x {item.m_displayName}!</color>\n Sent straight to your Lab workbench!");
//                OnItemPurchased?.Invoke(item, qty);
//            }
//            else
//            {
//                SetShopkeeperSpeech($"<color=#EF4444>Hé ho, Barista! Not enough credits for {item.m_displayName}!</color>\nYou need {totalCost - m_playerCredits} more Credits.");
//            }
//        }

//        private void UpdateCreditDisplay()
//        {
//            if (_creditText != null)
//            {
//                _creditText.text = $"CREDITS: {m_playerCredits:N0}";
//            }
//        }

//        private void SetShopkeeperSpeech(string text)
//        {
//            if (_speechText != null)
//            {
//                _speechText.text = text;
//            }
//        }

//        private bool HasSerializedBindings()
//        {
//            return _catalogContainer != null && _creditText != null;
//        }

//        private void BindExistingHierarchy()
//        {
//            if (_openMapBtn != null)
//            {
//                _openMapBtn.onClick.AddListener(() =>
//                {
//                    gameObject.SetActive(false);
//                    OnOpenMapRequested?.Invoke();
//                });
//            }

//            if (_returnToWorkbenchBtn != null)
//            {
//                _returnToWorkbenchBtn.onClick.AddListener(() =>
//                {
//                    gameObject.SetActive(false);
//                    OnReturnToWorkbenchRequested?.Invoke();
//                });
//            }
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private TextMeshProUGUI _creditText;
//        [SerializeField] private TextMeshProUGUI _speechText;
//        [SerializeField] private GameObject _shopkeeperAvatarRing;
//        [SerializeField] private Transform _catalogContainer;
//        [SerializeField] private Button _openMapBtn;
//        [SerializeField] private Button _returnToWorkbenchBtn;

//        private List<ShopItemData> _shopCatalog = new List<ShopItemData>();

//        #endregion
//    }
//}

//using System;
//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MolecularBrewing.Runtime
//{
//    public class UIPersonaCityMapScreen : MonoBehaviour
//    {
//        #region Publics

//        public event Action OnEnterShopRequested;
//        public event Action OnReturnToWorkbenchRequested;

//        #endregion


//        #region Unity API

//        private void Update()
//        {
//            if (gameObject.activeSelf)
//            {
//                float labPulse = 1f + Mathf.Sin(Time.time * 4f) * 0.12f;
//                if (_labGlowRing != null)
//                {
//                    _labGlowRing.transform.localScale = new Vector3(labPulse, labPulse, 1f);
//                }

//                float jojoPulse = 1f + Mathf.Sin(Time.time * 5f + 1f) * 0.15f;
//                if (_jojoGlowRing != null)
//                {
//                    _jojoGlowRing.transform.localScale = new Vector3(jojoPulse, jojoPulse, 1f);
//                }

//                float skyPulse = 1f + Mathf.Sin(Time.time * 2f) * 0.05f;
//                if (_skyGlowRing != null)
//                {
//                    _skyGlowRing.transform.localScale = new Vector3(skyPulse, skyPulse, 1f);
//                }
//            }
//        }

//        #endregion


//        #region Main API

//        public void Initialize()
//        {
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

//        public void OpenMap()
//        {
//            gameObject.SetActive(true);
//            transform.SetAsLastSibling();

//            Canvas.ForceUpdateCanvases();
//            ResetTransitRoute();
//            StartCoroutine(DeferredInitGeometry());
//        }

//        #endregion


//        #region Tools and Utilities

//        private IEnumerator DeferredInitGeometry()
//        {
//            yield return null;
//            Canvas.ForceUpdateCanvases();
//            SetupTrackGeometry(0f);
//        }

//        private void BuildUI()
//        {
//            var t = UIStyleHelper.Theme;

//            RectTransform rootRt = GetComponent<RectTransform>();
//            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
//            rootRt.anchorMin = Vector2.zero;
//            rootRt.anchorMax = Vector2.one;
//            rootRt.offsetMin = Vector2.zero;
//            rootRt.offsetMax = Vector2.zero;

//            GameObject bgObj = UIStyleHelper.CreatePanel(transform, "PersonaBg", new Color(0.06f, 0.07f, 0.09f, 1f));
//            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
//            bgRt.anchorMin = Vector2.zero;
//            bgRt.anchorMax = Vector2.one;
//            bgRt.offsetMin = Vector2.zero;
//            bgRt.offsetMax = Vector2.zero;

//            GameObject redRibbon = UIStyleHelper.CreatePanel(transform, "P5RedRibbon", new Color(0.85f, 0.15f, 0.22f, 0.95f));
//            RectTransform rrRt = redRibbon.GetComponent<RectTransform>();
//            rrRt.anchorMin = new Vector2(0.03f, 0.89f);
//            rrRt.anchorMax = new Vector2(0.97f, 0.98f);
//            rrRt.offsetMin = Vector2.zero;
//            rrRt.offsetMax = Vector2.zero;

//            GameObject blackRibbon = UIStyleHelper.CreatePanel(redRibbon.transform, "P5BlackRibbon", new Color(0.08f, 0.09f, 0.12f, 0.98f));
//            RectTransform brRt = blackRibbon.GetComponent<RectTransform>();
//            brRt.anchorMin = new Vector2(0.004f, 0.08f);
//            brRt.anchorMax = new Vector2(0.996f, 0.92f);
//            brRt.offsetMin = Vector2.zero;
//            brRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(blackRibbon.transform, "P5Title", "///  NEO-FREMIO CITY MAP  ///  NIGHT METRO", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform ptRt = blackRibbon.transform.Find("P5Title").GetComponent<RectTransform>();
//            ptRt.anchorMin = new Vector2(0.02f, 0.1f);
//            ptRt.anchorMax = new Vector2(0.65f, 0.9f);
//            ptRt.offsetMin = Vector2.zero;
//            ptRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(blackRibbon.transform, "TimeTicker", "DAY 1 • 22:30 • ", 12, new Color(1f, 0.85f, 0.2f, 1f), TextAlignmentOptions.Right, FontStyles.Bold);
//            RectTransform ttRt = blackRibbon.transform.Find("TimeTicker").GetComponent<RectTransform>();
//            ttRt.anchorMin = new Vector2(0.65f, 0.1f);
//            ttRt.anchorMax = new Vector2(0.98f, 0.9f);
//            ttRt.offsetMin = Vector2.zero;
//            ttRt.offsetMax = Vector2.zero;

//            GameObject mapContainer = UIStyleHelper.CreatePanel(transform, "MapContainer", new Color(0.04f, 0.06f, 0.09f, 1f));
//            _mapCanvasRt = mapContainer.GetComponent<RectTransform>();
//            _mapCanvasRt.anchorMin = new Vector2(0.03f, 0.10f);
//            _mapCanvasRt.anchorMax = new Vector2(0.97f, 0.87f);
//            _mapCanvasRt.offsetMin = Vector2.zero;
//            _mapCanvasRt.offsetMax = Vector2.zero;

//            // Map Background Texture Sprite
//            Sprite mapBgSprite = LoadMapBackgroundSprite();
//            if (mapBgSprite != null)
//            {
//                GameObject mapArtObj = UIStyleHelper.CreatePanel(mapContainer.transform, "MapArtBg", Color.white, mapBgSprite);
//                RectTransform maRt = mapArtObj.GetComponent<RectTransform>();
//                maRt.anchorMin = Vector2.zero;
//                maRt.anchorMax = Vector2.one;
//                maRt.offsetMin = Vector2.zero;
//                maRt.offsetMax = Vector2.zero;
//                Image img = mapArtObj.GetComponent<Image>();
//                img.preserveAspect = false;
//            }
//            else
//            {
//                CreateMapGridLines(mapContainer.transform);
//            }

//            GameObject trackLayer = new GameObject("TransitTrackLayer", typeof(RectTransform));
//            trackLayer.transform.SetParent(mapContainer.transform, false);
//            _trackLayerRt = trackLayer.GetComponent<RectTransform>();
//            _trackLayerRt.anchorMin = Vector2.zero;
//            _trackLayerRt.anchorMax = Vector2.one;
//            _trackLayerRt.offsetMin = Vector2.zero;
//            _trackLayerRt.offsetMax = Vector2.zero;

//            _trackBackgroundObj = UIStyleHelper.CreatePanel(trackLayer.transform, "TrackBackground", new Color(0.20f, 0.25f, 0.35f, 0.60f));
//            _trackBgRt = _trackBackgroundObj.GetComponent<RectTransform>();
//            _trackBgRt.pivot = new Vector2(0f, 0.5f);

//            _trackFillObj = UIStyleHelper.CreatePanel(trackLayer.transform, "TrackFill", new Color(0.85f, 0.15f, 0.22f, 0.95f));
//            _trackFillRt = _trackFillObj.GetComponent<RectTransform>();
//            _trackFillRt.pivot = new Vector2(0f, 0.5f);

//            _transitBeaconObj = UIStyleHelper.CreatePanel(trackLayer.transform, "TransitBeacon", new Color(1f, 0.90f, 0.25f, 1f), UIStyleHelper.CircleSprite);
//            _transitBeaconRt = _transitBeaconObj.GetComponent<RectTransform>();
//            _transitBeaconRt.sizeDelta = new Vector2(20, 20);
//            _transitBeaconObj.SetActive(false);

//            _labNodeObj = CreateMapNode(
//                mapContainer.transform,
//                "Node_CoffeeLab",
//                new Vector2(0.18f, 0.22f),
//                "COFFEE LAB WORKBENCH",
//                "District 07 • Sector Base",
//                "[CURRENT LOCATION]",
//                new Color(0.15f, 0.75f, 0.85f, 1f),
//                out _labGlowRing,
//                null
//            );

//            _jojoNodeObj = CreateMapNode(
//                mapContainer.transform,
//                "Node_JojoShop",
//                new Vector2(0.80f, 0.76f),
//                "JOJO LE BARJO'S SHOP",
//                "District 01 • Suspicious Alley",
//                "[CLICK TO TRAVEL & RESTOCK]",
//                new Color(0.85f, 0.15f, 0.22f, 1f),
//                out _jojoGlowRing,
//                OnTravelToJojoClicked
//            );

//            CreateMapNode(
//                mapContainer.transform,
//                "Node_SkyLounge",
//                new Vector2(0.48f, 0.82f),
//                "SKY-LOUNGE PLAZA",
//                "District 03 • VIP Corporate Rooftops",
//                "[LOCKED - OPENS DAY 2]",
//                new Color(0.35f, 0.40f, 0.50f, 0.8f),
//                out _skyGlowRing,
//                null
//            );

//            //GameObject dummyGlow;
//            //CreateMapNode(
//            //    mapContainer.transform,
//            //    "Node_SubwaySlums",
//            //    new Vector2(0.84f, 0.24f),
//            //    "SUBWAY SLUMS TECH BAZAAR",
//            //    "District 09 • Modder Workshops",
//            //    "[LOCKED - OPENS DAY 3]",
//            //    new Color(0.35f, 0.40f, 0.50f, 0.8f),
//            //    out dummyGlow,
//            //    null
//            //);

//            Button backBtn = UIStyleHelper.CreateButton(transform, "BackToLabBtn", "< RETURN TO WORKBENCH", t.m_btnSecondary, t.m_textPrimary, () =>
//            {
//                if (_isTraveling) return;
//                gameObject.SetActive(false);
//                OnReturnToWorkbenchRequested?.Invoke();
//            });
//            RectTransform bbRt = backBtn.GetComponent<RectTransform>();
//            bbRt.anchorMin = new Vector2(0.03f, 0.025f);
//            bbRt.anchorMax = new Vector2(0.24f, 0.085f);
//            bbRt.offsetMin = Vector2.zero;
//            bbRt.offsetMax = Vector2.zero;

//            _travelButton = UIStyleHelper.CreateButton(transform, "TravelBtn", "TRANSIT TO JOJO LE BARJO'S SHOP >", new Color(0.85f, 0.15f, 0.22f, 1f), Color.white, OnTravelToJojoClicked);
//            RectTransform tbRt = _travelButton.GetComponent<RectTransform>();
//            tbRt.anchorMin = new Vector2(0.68f, 0.025f);
//            tbRt.anchorMax = new Vector2(0.97f, 0.085f);
//            tbRt.offsetMin = Vector2.zero;
//            tbRt.offsetMax = Vector2.zero;

//            _statusReadoutText = UIStyleHelper.CreateText(transform, "StatusReadout", "SELECT DESTINATION: JOJO LE BARJO'S SHOP (DISTRICT 01)", 12, new Color(0.90f, 0.94f, 0.98f, 1f), TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform srRt = _statusReadoutText.rectTransform;
//            srRt.anchorMin = new Vector2(0.26f, 0.025f);
//            srRt.anchorMax = new Vector2(0.66f, 0.085f);
//            srRt.offsetMin = Vector2.zero;
//            srRt.offsetMax = Vector2.zero;
//        }

//        private Sprite LoadMapBackgroundSprite()
//        {
//            Sprite mapSprite = Resources.Load<Sprite>("NeoFremioMapBg");
//            if (mapSprite != null) return mapSprite;

//            try
//            {
//                string bgPath = System.IO.Path.Combine(Application.dataPath, "Resources/NeoFremioMapBg.png");
//                if (System.IO.File.Exists(bgPath))
//                {
//                    byte[] bytes = System.IO.File.ReadAllBytes(bgPath);
//                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
//                    if (tex.LoadImage(bytes))
//                    {
//                        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.LogWarning("[UIPersonaCityMapScreen] Failed loading map background image: " + e.Message);
//            }

//            return null;
//        }

//        private void CreateMapGridLines(Transform parent)
//        {
//            for (int i = 1; i <= 4; i++)
//            {
//                float normY = i * 0.20f;
//                GameObject hLine = UIStyleHelper.CreatePanel(parent, $"GridH_{i}", new Color(0.14f, 0.18f, 0.25f, 0.40f));
//                RectTransform hRt = hLine.GetComponent<RectTransform>();
//                hRt.anchorMin = new Vector2(0.02f, normY);
//                hRt.anchorMax = new Vector2(0.98f, normY + 0.002f);
//                hRt.offsetMin = Vector2.zero;
//                hRt.offsetMax = Vector2.zero;
//            }

//            for (int i = 1; i <= 4; i++)
//            {
//                float normX = i * 0.20f;
//                GameObject vLine = UIStyleHelper.CreatePanel(parent, $"GridV_{i}", new Color(0.14f, 0.18f, 0.25f, 0.40f));
//                RectTransform vRt = vLine.GetComponent<RectTransform>();
//                vRt.anchorMin = new Vector2(normX, 0.02f);
//                vRt.anchorMax = new Vector2(normX + 0.002f, 0.98f);
//                vRt.offsetMin = Vector2.zero;
//                vRt.offsetMax = Vector2.zero;
//            }
//        }

//        private GameObject CreateMapNode(
//            Transform parent,
//            string name,
//            Vector2 anchorPos,
//            string title,
//            string subtitle,
//            string status,
//            Color accentColor,
//            out GameObject glowRing,
//            Action onClick)
//        {
//            GameObject nodeContainer = new GameObject(name, typeof(RectTransform));
//            nodeContainer.transform.SetParent(parent, false);
//            RectTransform ncRt = nodeContainer.GetComponent<RectTransform>();
//            ncRt.anchorMin = anchorPos;
//            ncRt.anchorMax = anchorPos;
//            ncRt.sizeDelta = new Vector2(240, 75);
//            ncRt.anchoredPosition = Vector2.zero;

//            glowRing = UIStyleHelper.CreatePanel(nodeContainer.transform, "GlowRing", new Color(accentColor.r, accentColor.g, accentColor.b, 0.35f), UIStyleHelper.CircleSprite);
//            RectTransform grRt = glowRing.GetComponent<RectTransform>();
//            grRt.anchorMin = new Vector2(0f, 0.5f);
//            grRt.anchorMax = new Vector2(0f, 0.5f);
//            grRt.anchoredPosition = new Vector2(22, 0);
//            grRt.sizeDelta = new Vector2(52, 52);

//            GameObject pinObj = UIStyleHelper.CreatePanel(nodeContainer.transform, "Pin", accentColor, UIStyleHelper.CircleSprite);
//            RectTransform pRt = pinObj.GetComponent<RectTransform>();
//            pRt.anchorMin = new Vector2(0f, 0.5f);
//            pRt.anchorMax = new Vector2(0f, 0.5f);
//            pRt.anchoredPosition = new Vector2(22, 0);
//            pRt.sizeDelta = new Vector2(24, 24);

//            GameObject cardObj = UIStyleHelper.CreatePanel(nodeContainer.transform, "Card", new Color(0.12f, 0.15f, 0.22f, 0.95f));
//            RectTransform cRt = cardObj.GetComponent<RectTransform>();
//            cRt.anchorMin = new Vector2(0f, 0f);
//            cRt.anchorMax = new Vector2(1f, 1f);
//            cRt.offsetMin = new Vector2(46, 0);
//            cRt.offsetMax = Vector2.zero;

//            GameObject stripe = UIStyleHelper.CreatePanel(cardObj.transform, "Stripe", accentColor);
//            RectTransform sRt = stripe.GetComponent<RectTransform>();
//            sRt.anchorMin = Vector2.zero;
//            sRt.anchorMax = new Vector2(0.04f, 1f);
//            sRt.offsetMin = Vector2.zero;
//            sRt.offsetMax = Vector2.zero;

//            TextMeshProUGUI tTxt = UIStyleHelper.CreateText(cardObj.transform, "Title", title, 10, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform ttRt = tTxt.rectTransform;
//            ttRt.anchorMin = new Vector2(0.08f, 0.60f);
//            ttRt.anchorMax = new Vector2(0.96f, 0.95f);
//            ttRt.offsetMin = Vector2.zero;
//            ttRt.offsetMax = Vector2.zero;

//            TextMeshProUGUI subTxt = UIStyleHelper.CreateText(cardObj.transform, "Sub", subtitle, 8, new Color(0.70f, 0.78f, 0.86f, 0.85f), TextAlignmentOptions.Left, FontStyles.Normal);
//            RectTransform stRt = subTxt.rectTransform;
//            stRt.anchorMin = new Vector2(0.08f, 0.32f);
//            stRt.anchorMax = new Vector2(0.96f, 0.60f);
//            stRt.offsetMin = Vector2.zero;
//            stRt.offsetMax = Vector2.zero;

//            TextMeshProUGUI statTxt = UIStyleHelper.CreateText(cardObj.transform, "Status", status, 8, accentColor, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform sstRt = statTxt.rectTransform;
//            sstRt.anchorMin = new Vector2(0.08f, 0.05f);
//            sstRt.anchorMax = new Vector2(0.96f, 0.32f);
//            sstRt.offsetMin = Vector2.zero;
//            sstRt.offsetMax = Vector2.zero;

//            if (onClick != null)
//            {
//                Button cardBtn = cardObj.AddComponent<Button>();
//                cardBtn.onClick.AddListener(() => onClick());
//            }

//            return nodeContainer;
//        }

//        private void ResetTransitRoute()
//        {
//            _isTraveling = false;

//            if (_travelCoroutine != null)
//            {
//                StopCoroutine(_travelCoroutine);
//                _travelCoroutine = null;
//            }

//            if (_travelButton != null)
//            {
//                _travelButton.interactable = true;
//                _travelButton.GetComponentInChildren<TextMeshProUGUI>().text = "TRANSIT TO JOJO LE BARJO'S SHOP >";
//            }

//            if (_statusReadoutText != null)
//            {
//                _statusReadoutText.text = "SELECT DESTINATION: JOJO LE BARJO'S UNDERGROUND SHOP (DISTRICT 01)";
//            }

//            SetupTrackGeometry(0f);
//            if (_transitBeaconObj != null) _transitBeaconObj.SetActive(false);
//        }

//        private void SetupTrackGeometry(float fillProgress)
//        {
//            if (_trackLayerRt == null || _labNodeObj == null || _jojoNodeObj == null) return;

//            Transform labPin = _labNodeObj.transform.Find("Pin");
//            Transform jojoPin = _jojoNodeObj.transform.Find("Pin");
//            if (labPin == null || jojoPin == null) return;

//            Vector2 labLocal = _trackLayerRt.InverseTransformPoint(labPin.position);
//            Vector2 jojoLocal = _trackLayerRt.InverseTransformPoint(jojoPin.position);

//            Vector2 dir = jojoLocal - labLocal;
//            float totalDist = dir.magnitude;
//            if (totalDist < 1f) return;

//            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

//            if (_trackBgRt != null)
//            {
//                _trackBgRt.anchoredPosition = labLocal;
//                _trackBgRt.sizeDelta = new Vector2(totalDist, 6);
//                _trackBgRt.localRotation = Quaternion.Euler(0, 0, angle);
//            }

//            if (_trackFillRt != null)
//            {
//                _trackFillRt.anchoredPosition = labLocal;
//                _trackFillRt.sizeDelta = new Vector2(totalDist * Mathf.Clamp01(fillProgress), 8);
//                _trackFillRt.localRotation = Quaternion.Euler(0, 0, angle);
//            }

//            if (_transitBeaconRt != null)
//            {
//                Vector2 beaconPos = Vector2.Lerp(labLocal, jojoLocal, Mathf.Clamp01(fillProgress));
//                _transitBeaconRt.anchoredPosition = beaconPos;
//            }
//        }

//        private void OnTravelToJojoClicked()
//        {
//            if (_isTraveling) return;
//            _travelCoroutine = StartCoroutine(TravelToJojoSequenceRoutine());
//        }

//        private IEnumerator TravelToJojoSequenceRoutine()
//        {
//            _isTraveling = true;
//            if (_travelButton != null) _travelButton.interactable = false;

//            AudioManager.PlayMetro(1.0f);

//            if (_statusReadoutText != null)
//            {
//                _statusReadoutText.text = "<color=#FBBF24>TRANSIT IN PROGRESS: DEPARTING COFFEE LAB (DISTRICT 07)...</color>";
//            }

//            if (_transitBeaconObj != null) _transitBeaconObj.SetActive(true);

//            float duration = 1.4f;
//            float elapsed = 0f;

//            while (elapsed < duration)
//            {
//                elapsed += Time.deltaTime;
//                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);

//                SetupTrackGeometry(progress);

//                if (progress > 0.5f && _statusReadoutText != null)
//                {
//                    _statusReadoutText.text = "<color=#EF4444>APPROACHING JOJO LE BARJO'S SHOP (DISTRICT 01)...</color>";
//                }

//                yield return null;
//            }

//            SetupTrackGeometry(1f);

//            if (_statusReadoutText != null)
//            {
//                _statusReadoutText.text = "<color=#10B981>ARRIVAL CONFIRMED • WELCOME TO JOJO LE BARJO'S SHOP!</color>";
//            }

//            yield return new WaitForSecondsRealtime(0.35f);

//            gameObject.SetActive(false);
//            _isTraveling = false;
//            OnEnterShopRequested?.Invoke();
//        }

//        private bool HasSerializedBindings()
//        {
//            return _travelButton != null && _mapCanvasRt != null;
//        }

//        private void BindExistingHierarchy()
//        {
//            if (_travelButton != null)
//            {
//                _travelButton.onClick.AddListener(OnTravelToJojoClicked);
//            }

//            if (_returnToWorkbenchButton != null)
//            {
//                _returnToWorkbenchButton.onClick.AddListener(() =>
//                {
//                    gameObject.SetActive(false);
//                    OnReturnToWorkbenchRequested?.Invoke();
//                });
//            }
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private RectTransform _mapCanvasRt;
//        [SerializeField] private RectTransform _trackLayerRt;
//        [SerializeField] private GameObject _labNodeObj;
//        [SerializeField] private GameObject _jojoNodeObj;
//        [SerializeField] private GameObject _labGlowRing;
//        [SerializeField] private GameObject _jojoGlowRing;
//        [SerializeField] private GameObject _skyGlowRing;

//        [SerializeField] private GameObject _trackBackgroundObj;
//        [SerializeField] private RectTransform _trackBgRt;
//        [SerializeField] private GameObject _trackFillObj;
//        [SerializeField] private RectTransform _trackFillRt;
//        [SerializeField] private GameObject _transitBeaconObj;
//        [SerializeField] private RectTransform _transitBeaconRt;

//        [SerializeField] private Button _travelButton;
//        [SerializeField] private Button _returnToWorkbenchButton;
//        [SerializeField] private TextMeshProUGUI _statusReadoutText;
//        [SerializeField] private TextMeshProUGUI _sidebarDistrictTitle;
//        [SerializeField] private TextMeshProUGUI _sidebarDescriptionText;

//        private bool _isTraveling;
//        private Coroutine _travelCoroutine;

//        #endregion
//    }
//}

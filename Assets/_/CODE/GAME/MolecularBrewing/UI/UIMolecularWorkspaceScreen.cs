//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    public class UIMolecularWorkspaceScreen : MonoBehaviour
//    {
//        #region Publics

//        public event Action OnBackToPantryRequested;
//        public event Action<List<MoleculeType>, List<(MoleculeType a, MoleculeType b)>> OnValidateRequested;

//        #endregion


//        #region Unity API

//        private void Update()
//        {
//            if (_isDrawingLink && _linkOriginNode != null && _tempLinkRenderer != null)
//            {
//                Vector2 localMouse;
//                RectTransformUtility.ScreenPointToLocalPointInRectangle(
//                    _linesContainer.GetComponent<RectTransform>(),
//                    Input.mousePosition,
//                    _rootCanvas != null && _rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? _rootCanvas.worldCamera : null,
//                    out localMouse);

//                _tempLinkRenderer.SetFreeLineGeometry(_linkOriginNode.m_rectTransform.anchoredPosition, localMouse, new Color(0.3f, 0.9f, 1f, 0.9f));
//            }
//            if (_activeLinks != null && _activeLinks.Count > 0)
//            {
//                for (int i = 0; i < _activeLinks.Count; i++)
//                {
//                    if (_activeLinks[i] != null)
//                    {
//                        _activeLinks[i].UpdateLineGeometry();
//                    }
//                }

//                if (_activeLinks.Count >= 2)
//                {
//                    CheckAndBreakIntersectingLinks();
//                }
//            }
//        }

//        #endregion


//        #region Main API

//        public void Initialize(
//            List<IngredientDefinition> ingredients,
//            List<DrinkRecipePattern> recipes,
//            List<SpecialEffectDefinition> effects,
//            Func<MoleculeType, MoleculeDefinition> defLookup,
//            Canvas rootCanvas)
//        {
//            _selectedIngredients = ingredients;
//            _recipeDatabase = recipes;
//            _effectDatabase = effects;
//            _defLookup = defLookup;
//            _rootCanvas = rootCanvas;

//            if (!HasSerializedBindings())
//            {
//                BuildUI();
//            }
//            else
//            {
//                BindExistingHierarchy();
//            }

//            SpawnScatteredMolecules();
//            UpdateWorkspaceState();
//        }

//        public void UpdateWorkspaceState()
//        {
//            HashSet<UIMoleculeNode> bondedNodes = new HashSet<UIMoleculeNode>();
//            List<(MoleculeType a, MoleculeType b)> activeBonds = new List<(MoleculeType a, MoleculeType b)>();

//            foreach (var link in _activeLinks)
//            {
//                if (link != null && link.m_nodeA != null && link.m_nodeB != null)
//                {
//                    bondedNodes.Add(link.m_nodeA);
//                    bondedNodes.Add(link.m_nodeB);
//                    activeBonds.Add((link.m_nodeA.m_moleculeType, link.m_nodeB.m_moleculeType));
//                }
//            }

//            foreach (var node in _spawnedNodes)
//            {
//                if (node != null)
//                {
//                    node.SetBondedState(bondedNodes.Contains(node));
//                }
//            }

//            foreach (var link in _activeLinks)
//            {
//                if (link != null) link.UpdateLineGeometry();
//            }

//            List<MoleculeType> bondedMolecules = new List<MoleculeType>();
//            foreach (var node in bondedNodes)
//            {
//                if (node != null) bondedMolecules.Add(node.m_moleculeType);
//            }

//            TasteStats stats = TasteStats.Calculate(bondedMolecules, activeBonds, _defLookup);
//            _meterSweetness.SetValue(stats.m_sweetness);
//            _meterBitterness.SetValue(stats.m_bitterness);
//            _meterNeuralCalm.SetValue(stats.m_neuralCalm);
//            _meterSedation.SetValue(stats.m_sedation);
//            _meterFocus.SetValue(stats.m_focus);
//            _meterEuphoria.SetValue(stats.m_euphoria);

//            if (bondedMolecules.Count == 0)
//            {
//                _liveEffectBannerText.text = "Crucible Idle\n(Right-click drag to link molecules together)";
//                _liveEffectBannerPanel.SetActive(true);
//                _liveRecipeMatchText.text = "Recognized Base: [Unbonded Crucible]\nDraw links to assemble a drink";
//            }
//            else
//            {
//                var eval = DrinkEvaluator.Evaluate(bondedMolecules, activeBonds, _recipeDatabase, _effectDatabase, _defLookup);

//                if (eval.m_unlockedEffects != null && eval.m_unlockedEffects.Count > 0)
//                {
//                    var eff = eval.m_unlockedEffects[0];
//                    _liveEffectBannerText.text = $"SPECIAL EFFECT ACTIVE: <color=#{ColorUtility.ToHtmlStringRGB(eff.m_badgeColor)}>[{eff.m_displayName}]</color>\n{eff.m_tagline}";
//                    _liveEffectBannerPanel.SetActive(true);
//                }
//                else
//                {
//                    _liveEffectBannerText.text = "No Special Medicinal Effect Active\n(Link bio-active plants to coffee/water/fats)";
//                    _liveEffectBannerPanel.SetActive(true);
//                }

//                _liveRecipeMatchText.text = $"Recognized Base: <color=#{ColorUtility.ToHtmlStringRGB(eval.m_primaryColor)}>{eval.m_drinkName}</color>\nRating: Rank {eval.m_rank} ({eval.m_matchScore}% Synthesis Match)";
//            }

//            _activeLinkCountText.text = $"Active Chemical Bonds: {_activeLinks.Count} ({bondedMolecules.Count} Molecules Linked)";
//        }

//        #endregion


//        #region Tools and Utilities

//        private void BuildUI()
//        {
//            var t = UIStyleHelper.Theme;

//            RectTransform rootRt = GetComponent<RectTransform>();
//            if (rootRt == null) rootRt = gameObject.AddComponent<RectTransform>();
//            rootRt.anchorMin = Vector2.zero;
//            rootRt.anchorMax = Vector2.one;
//            rootRt.offsetMin = Vector2.zero;
//            rootRt.offsetMax = Vector2.zero;

//            GameObject bgObj = UIStyleHelper.CreatePanel(transform, "Bg", t.m_background);
//            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
//            bgRt.anchorMin = Vector2.zero;
//            bgRt.anchorMax = Vector2.one;
//            bgRt.offsetMin = Vector2.zero;
//            bgRt.offsetMax = Vector2.zero;

//            GameObject topBar = UIStyleHelper.CreatePanel(transform, "TopBar", t.m_headerBg);
//            RectTransform tbRt = topBar.GetComponent<RectTransform>();
//            tbRt.anchorMin = new Vector2(0f, 0.92f);
//            tbRt.anchorMax = new Vector2(1f, 1f);
//            tbRt.offsetMin = new Vector2(15, 5);
//            tbRt.offsetMax = new Vector2(-15, -5);

//            UIStyleHelper.CreateButton(topBar.transform, "BackBtn", "< HOPPER", t.m_btnSecondary, t.m_textPrimary, () => OnBackToPantryRequested?.Invoke());
//            RectTransform bRt = topBar.transform.Find("BackBtn").GetComponent<RectTransform>();
//            bRt.anchorMin = new Vector2(0.01f, 0.15f);
//            bRt.anchorMax = new Vector2(0.10f, 0.85f);
//            bRt.offsetMin = Vector2.zero;
//            bRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(topBar.transform, "Title", "MICROSCOPIC SYNTHESIS CHAMBER - MOLECULAR BONDING", 16, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);

//            Button recipeBookBtn = UIStyleHelper.CreateButton(topBar.transform, "BlueprintsBtn", "BLUEPRINTS", t.m_btnPrimary, Color.white, ToggleRecipeBook);
//            RectTransform rbRt = recipeBookBtn.GetComponent<RectTransform>();
//            rbRt.anchorMin = new Vector2(0.89f, 0.15f);
//            rbRt.anchorMax = new Vector2(0.99f, 0.85f);
//            rbRt.offsetMin = Vector2.zero;
//            rbRt.offsetMax = Vector2.zero;

//            GameObject chamberPanel = UIStyleHelper.CreatePanel(transform, "ChamberFrame", t.m_panelBg);
//            RectTransform cvRt = chamberPanel.GetComponent<RectTransform>();
//            cvRt.anchorMin = new Vector2(0.02f, 0.04f);
//            cvRt.anchorMax = new Vector2(0.70f, 0.90f);
//            cvRt.offsetMin = Vector2.zero;
//            cvRt.offsetMax = Vector2.zero;

//            GameObject chHeader = UIStyleHelper.CreatePanel(chamberPanel.transform, "Header", t.m_headerBg);
//            RectTransform chhRt = chHeader.GetComponent<RectTransform>();
//            chhRt.anchorMin = new Vector2(0.01f, 0.92f);
//            chhRt.anchorMax = new Vector2(0.99f, 0.99f);
//            chhRt.offsetMin = Vector2.zero;
//            chhRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(chHeader.transform, "Inst", "MICROSCOPE OCULAR [Left-Click Drag: Move | Right-Click Drag: Link Molecules]", 11, new Color(0.8f, 0.95f, 1f, 0.95f), TextAlignmentOptions.Center, FontStyles.Bold);

//            GameObject bezelObj = UIStyleHelper.CreatePanel(chamberPanel.transform, "BezelGlow", t.m_lensBezel, UIStyleHelper.CircleSprite);
//            RectTransform bzRt = bezelObj.GetComponent<RectTransform>();
//            bzRt.anchorMin = new Vector2(0.04f, 0.09f);
//            bzRt.anchorMax = new Vector2(0.96f, 0.91f);
//            bzRt.offsetMin = new Vector2(-6, -6);
//            bzRt.offsetMax = new Vector2(6, 6);
//            AspectRatioFitter arfBz = bezelObj.AddComponent<AspectRatioFitter>();
//            arfBz.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
//            arfBz.aspectRatio = 1.0f;

//            GameObject lensAperture = UIStyleHelper.CreatePanel(chamberPanel.transform, "LensAperture", t.m_lensGlass, UIStyleHelper.CircleSprite);
//            Mask lensMask = lensAperture.AddComponent<Mask>();
//            lensMask.showMaskGraphic = true;
//            _chamberViewport = lensAperture.transform;

//            RectTransform laRt = lensAperture.GetComponent<RectTransform>();
//            laRt.anchorMin = new Vector2(0.04f, 0.09f);
//            laRt.anchorMax = new Vector2(0.96f, 0.91f);
//            laRt.offsetMin = Vector2.zero;
//            laRt.offsetMax = Vector2.zero;
//            AspectRatioFitter arfLa = lensAperture.AddComponent<AspectRatioFitter>();
//            arfLa.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
//            arfLa.aspectRatio = 1.0f;

//            GameObject reticleRing1 = UIStyleHelper.CreatePanel(lensAperture.transform, "ReticleRing1", t.m_lensReticle, UIStyleHelper.CircleSprite);
//            RectTransform rr1 = reticleRing1.GetComponent<RectTransform>();
//            rr1.anchorMin = new Vector2(0.20f, 0.20f);
//            rr1.anchorMax = new Vector2(0.80f, 0.80f);
//            rr1.offsetMin = Vector2.zero;
//            rr1.offsetMax = Vector2.zero;

//            GameObject reticleRing2 = UIStyleHelper.CreatePanel(lensAperture.transform, "ReticleRing2", t.m_lensReticle, UIStyleHelper.CircleSprite);
//            RectTransform rr2 = reticleRing2.GetComponent<RectTransform>();
//            rr2.anchorMin = new Vector2(0.38f, 0.38f);
//            rr2.anchorMax = new Vector2(0.62f, 0.62f);
//            rr2.offsetMin = Vector2.zero;
//            rr2.offsetMax = Vector2.zero;

//            GameObject crossH = UIStyleHelper.CreatePanel(lensAperture.transform, "CrossH", t.m_lensReticle);
//            RectTransform chRt = crossH.GetComponent<RectTransform>();
//            chRt.anchorMin = new Vector2(0.05f, 0.495f);
//            chRt.anchorMax = new Vector2(0.95f, 0.505f);
//            chRt.offsetMin = Vector2.zero;
//            chRt.offsetMax = Vector2.zero;

//            GameObject crossV = UIStyleHelper.CreatePanel(lensAperture.transform, "CrossV", t.m_lensReticle);
//            RectTransform cvhRt = crossV.GetComponent<RectTransform>();
//            cvhRt.anchorMin = new Vector2(0.497f, 0.05f);
//            cvhRt.anchorMax = new Vector2(0.503f, 0.95f);
//            cvhRt.offsetMin = Vector2.zero;
//            cvhRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(lensAperture.transform, "ZoomTag", "MAG: 450,000X [NANOSCALE OCULAR]", 9, t.m_accentCyan, TextAlignmentOptions.TopLeft, FontStyles.Bold);
//            RectTransform ztRt = lensAperture.transform.Find("ZoomTag").GetComponent<RectTransform>();
//            ztRt.anchorMin = new Vector2(0.08f, 0.88f);
//            ztRt.anchorMax = new Vector2(0.50f, 0.95f);
//            ztRt.offsetMin = Vector2.zero;
//            ztRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(lensAperture.transform, "ApertureTag", "FOV: 14.8 nm CIRCULAR", 9, t.m_accentCyan, TextAlignmentOptions.TopRight, FontStyles.Bold);
//            RectTransform atRt = lensAperture.transform.Find("ApertureTag").GetComponent<RectTransform>();
//            atRt.anchorMin = new Vector2(0.50f, 0.88f);
//            atRt.anchorMax = new Vector2(0.92f, 0.95f);
//            atRt.offsetMin = Vector2.zero;
//            atRt.offsetMax = Vector2.zero;

//            GameObject linesObj = new GameObject("LinesContainer", typeof(RectTransform));
//            linesObj.transform.SetParent(lensAperture.transform, false);
//            _linesContainer = linesObj.transform;
//            RectTransform lcRt = linesObj.GetComponent<RectTransform>();
//            lcRt.anchorMin = Vector2.zero;
//            lcRt.anchorMax = Vector2.one;
//            lcRt.offsetMin = Vector2.zero;
//            lcRt.offsetMax = Vector2.zero;

//            GameObject tempLineObj = new GameObject("TempDragLine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//            tempLineObj.transform.SetParent(_linesContainer, false);
//            _tempLinkRenderer = tempLineObj.AddComponent<UILinkRenderer>();
//            tempLineObj.SetActive(false);

//            GameObject nodesObj = new GameObject("NodesContainer", typeof(RectTransform));
//            nodesObj.transform.SetParent(lensAperture.transform, false);
//            _nodesContainer = nodesObj.transform;
//            RectTransform ncRt = nodesObj.GetComponent<RectTransform>();
//            ncRt.anchorMin = Vector2.zero;
//            ncRt.anchorMax = Vector2.one;
//            ncRt.offsetMin = Vector2.zero;
//            ncRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateButton(chamberPanel.transform, "RescatterBtn", "RE-SCATTER", t.m_btnSecondary, t.m_textPrimary, SpawnScatteredMolecules);
//            RectTransform rsRt = chamberPanel.transform.Find("RescatterBtn").GetComponent<RectTransform>();
//            rsRt.anchorMin = new Vector2(0.02f, 0.02f);
//            rsRt.anchorMax = new Vector2(0.16f, 0.08f);
//            rsRt.offsetMin = Vector2.zero;
//            rsRt.offsetMax = Vector2.zero;

//            _freezeToggleBtn = UIStyleHelper.CreateButton(chamberPanel.transform, "FreezeToggleBtn", "FREEZE MOLECULE: OFF", t.m_btnPrimary, Color.white, ToggleFreezeMolecules);
//            RectTransform dtRt = _freezeToggleBtn.GetComponent<RectTransform>();
//            dtRt.anchorMin = new Vector2(0.18f, 0.02f);
//            dtRt.anchorMax = new Vector2(0.35f, 0.08f);
//            dtRt.offsetMin = Vector2.zero;
//            dtRt.offsetMax = Vector2.zero;
//            _freezeToggleText = _freezeToggleBtn.GetComponentInChildren<TextMeshProUGUI>();
//            _freezeToggleText.fontSize = 11;

//            UIStyleHelper.CreateButton(chamberPanel.transform, "ClearLinksBtn", "CLEAR ALL BONDS", t.m_btnDanger, Color.white, ClearAllBonds);
//            RectTransform clRt = chamberPanel.transform.Find("ClearLinksBtn").GetComponent<RectTransform>();
//            clRt.anchorMin = new Vector2(0.36f, 0.02f);
//            clRt.anchorMax = new Vector2(0.52f, 0.08f);
//            clRt.offsetMin = Vector2.zero;
//            clRt.offsetMax = Vector2.zero;

//            _activeLinkCountText = UIStyleHelper.CreateText(chamberPanel.transform, "LinkCounter", "Active Chemical Bonds: 0", 11, t.m_textSecondary, TextAlignmentOptions.Right, FontStyles.Normal);
//            RectTransform lkRt = _activeLinkCountText.rectTransform;
//            lkRt.anchorMin = new Vector2(0.54f, 0.02f);
//            lkRt.anchorMax = new Vector2(0.98f, 0.08f);
//            lkRt.offsetMin = Vector2.zero;
//            lkRt.offsetMax = Vector2.zero;

//            GameObject sidePanel = UIStyleHelper.CreatePanel(transform, "ScannerSidebar", t.m_panelBg);
//            RectTransform spRt = sidePanel.GetComponent<RectTransform>();
//            spRt.anchorMin = new Vector2(0.72f, 0.04f);
//            spRt.anchorMax = new Vector2(0.98f, 0.90f);
//            spRt.offsetMin = Vector2.zero;
//            spRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(sidePanel.transform, "SideTitle", "DRINK SCANNER & BIO-STATS", 14, t.m_textPrimary, TextAlignmentOptions.TopLeft, FontStyles.Bold);

//            _liveEffectBannerPanel = UIStyleHelper.CreatePanel(sidePanel.transform, "EffectBanner", t.m_cardBg);
//            RectTransform ebRt = _liveEffectBannerPanel.GetComponent<RectTransform>();
//            ebRt.anchorMin = new Vector2(0.04f, 0.70f);
//            ebRt.anchorMax = new Vector2(0.96f, 0.91f);
//            ebRt.offsetMin = Vector2.zero;
//            ebRt.offsetMax = Vector2.zero;

//            _liveEffectBannerText = UIStyleHelper.CreateText(_liveEffectBannerPanel.transform, "EffectText", "Effect...", 10, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform etRt = _liveEffectBannerText.rectTransform;
//            etRt.anchorMin = Vector2.zero;
//            etRt.anchorMax = Vector2.one;
//            etRt.offsetMin = new Vector2(6, 6);
//            etRt.offsetMax = new Vector2(-6, -6);

//            GameObject matchBanner = UIStyleHelper.CreatePanel(sidePanel.transform, "MatchBanner", t.m_cardBg);
//            RectTransform mbRt = matchBanner.GetComponent<RectTransform>();
//            mbRt.anchorMin = new Vector2(0.04f, 0.52f);
//            mbRt.anchorMax = new Vector2(0.96f, 0.68f);
//            mbRt.offsetMin = Vector2.zero;
//            mbRt.offsetMax = Vector2.zero;

//            _liveRecipeMatchText = UIStyleHelper.CreateText(matchBanner.transform, "MatchText", "Recognized Base: ...", 10, t.m_textPrimary, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform mbtRt = _liveRecipeMatchText.rectTransform;
//            mbtRt.anchorMin = Vector2.zero;
//            mbtRt.anchorMax = Vector2.one;
//            mbtRt.offsetMin = new Vector2(6, 6);
//            mbtRt.offsetMax = new Vector2(-6, -6);

//            GameObject metersObj = new GameObject("Meters", typeof(RectTransform), typeof(VerticalLayoutGroup));
//            metersObj.transform.SetParent(sidePanel.transform, false);
//            RectTransform mRt = metersObj.GetComponent<RectTransform>();
//            mRt.anchorMin = new Vector2(0.04f, 0.14f);
//            mRt.anchorMax = new Vector2(0.96f, 0.50f);
//            mRt.offsetMin = Vector2.zero;
//            mRt.offsetMax = Vector2.zero;

//            VerticalLayoutGroup vlg = metersObj.GetComponent<VerticalLayoutGroup>();
//            vlg.spacing = 4;
//            vlg.childControlWidth = true;
//            vlg.childControlHeight = false;

//            _meterNeuralCalm = CreateMeter(metersObj.transform, "Neural Calm (Anxiety)", new Color(0.05f, 0.70f, 0.60f, 1f));
//            _meterSedation = CreateMeter(metersObj.transform, "Sedation (Deep Sleep)", new Color(0.35f, 0.25f, 0.80f, 1f));
//            _meterFocus = CreateMeter(metersObj.transform, "Focus (Overclock)", new Color(0.85f, 0.25f, 0.30f, 1f));
//            _meterEuphoria = CreateMeter(metersObj.transform, "Euphoria (Glitch-Joy)", new Color(0.85f, 0.30f, 0.75f, 1f));
//            _meterSweetness = CreateMeter(metersObj.transform, "Sweetness", new Color(0.85f, 0.65f, 0.20f, 1f));
//            _meterBitterness = CreateMeter(metersObj.transform, "Bitterness", new Color(0.40f, 0.25f, 0.15f, 1f));

//            UIStyleHelper.CreateButton(sidePanel.transform, "SynthesizeBtn", "SYNTHESIZE & SERVE >", t.m_accentGreen, Color.white, OnValidateClicked);
//            RectTransform syRt = sidePanel.transform.Find("SynthesizeBtn").GetComponent<RectTransform>();
//            syRt.anchorMin = new Vector2(0.04f, 0.03f);
//            syRt.anchorMax = new Vector2(0.96f, 0.11f);
//            syRt.offsetMin = Vector2.zero;
//            syRt.offsetMax = Vector2.zero;

//            BuildRecipeBookModal();
//        }

//        private UIStatMeter CreateMeter(Transform parent, string name, Color color)
//        {
//            GameObject meterObj = new GameObject($"Meter_{name}", typeof(RectTransform));
//            meterObj.transform.SetParent(parent, false);
//            UIStatMeter meter = meterObj.AddComponent<UIStatMeter>();
//            meter.Initialize(name, color);
//            return meter;
//        }

//        private void SpawnScatteredMolecules()
//        {
//            ClearAllBonds();

//            foreach (var n in _spawnedNodes)
//            {
//                if (n != null) Destroy(n.gameObject);
//            }
//            _spawnedNodes.Clear();

//            List<MoleculeType> allYields = new List<MoleculeType>();
//            foreach (var ing in _selectedIngredients)
//            {
//                foreach (var y in ing.m_moleculeYields)
//                {
//                    for (int i = 0; i < y.m_count; i++)
//                    {
//                        allYields.Add(y.m_type);
//                    }
//                }
//            }

//            RectTransform cvRt = _chamberViewport.GetComponent<RectTransform>();
//            float lensRadius = Mathf.Min(cvRt.rect.width, cvRt.rect.height) * 0.5f;
//            if (lensRadius < 100f) lensRadius = 320f;
//            float maxRadius = Mathf.Max(60f, lensRadius - 48f);

//            int count = allYields.Count;
//            List<Vector2> placedPositions = new List<Vector2>();

//            for (int i = 0; i < count; i++)
//            {
//                var molType = allYields[i];
//                var def = _defLookup(molType);

//                Vector2 candidatePos = Vector2.zero;
//                float bestSeparation = -1f;

//                for (int attempt = 0; attempt < 30; attempt++)
//                {
//                    float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
//                    float radiusNorm = Mathf.Sqrt(UnityEngine.Random.Range(0.04f, 0.78f));
//                    Vector2 testPos = new Vector2(Mathf.Cos(angle) * maxRadius * radiusNorm, Mathf.Sin(angle) * maxRadius * radiusNorm);

//                    float minNeighbor = float.MaxValue;
//                    foreach (var p in placedPositions)
//                    {
//                        float d = Vector2.Distance(testPos, p);
//                        if (d < minNeighbor) minNeighbor = d;
//                    }

//                    if (minNeighbor > bestSeparation)
//                    {
//                        bestSeparation = minNeighbor;
//                        candidatePos = testPos;
//                        if (minNeighbor > 85f) break;
//                    }
//                }

//                placedPositions.Add(candidatePos);

//                GameObject nodeObj = new GameObject($"Node_{molType}", typeof(RectTransform), typeof(CanvasGroup));
//                nodeObj.transform.SetParent(_nodesContainer, false);
//                UIMoleculeNode node = nodeObj.AddComponent<UIMoleculeNode>();
//                node.Initialize(def, _rootCanvas, candidatePos);
//                node.SetFrozenState(_isMoleculesFrozen);

//                node.OnNodeMoved += (n) => UpdateWorkspaceState();
//                node.OnRightClickDragStart += HandleRightClickDragStart;
//                node.OnRightClickDragUpdate += HandleRightClickDragUpdate;
//                node.OnRightClickDragEnd += HandleRightClickDragEnd;

//                _spawnedNodes.Add(node);
//            }

//            UpdateWorkspaceState();
//        }

//        private void ToggleFreezeMolecules()
//        {
//            _isMoleculesFrozen = !_isMoleculesFrozen;
//            if (_freezeToggleText != null)
//            {
//                _freezeToggleText.text = _isMoleculesFrozen ? "FREEZE MOLECULE: ON" : "FREEZE MOLECULE: OFF";
//            }
//            if (_freezeToggleBtn != null)
//            {
//                _freezeToggleBtn.GetComponent<Image>().color = _isMoleculesFrozen ?
//                    new Color(0.12f, 0.65f, 0.85f, 1f) : UIStyleHelper.Theme.m_btnPrimary;
//            }
//            foreach (var node in _spawnedNodes)
//            {
//                if (node != null) node.SetFrozenState(_isMoleculesFrozen);
//            }
//        }

//        private void HandleRightClickDragStart(UIMoleculeNode node, Vector2 mousePos)
//        {
//            _isDrawingLink = true;
//            _linkOriginNode = node;
//            if (_tempLinkRenderer != null)
//            {
//                _tempLinkRenderer.gameObject.SetActive(true);
//            }
//        }

//        private void HandleRightClickDragUpdate(UIMoleculeNode node, Vector2 mousePos)
//        {
//        }

//        private void HandleRightClickDragEnd(UIMoleculeNode originNode, UIMoleculeNode targetNode)
//        {
//            _isDrawingLink = false;
//            _linkOriginNode = null;
//            if (_tempLinkRenderer != null)
//            {
//                _tempLinkRenderer.gameObject.SetActive(false);
//            }

//            if (originNode != null && targetNode != null && originNode != targetNode)
//            {
//                ToggleLink(originNode, targetNode);
//            }
//        }

//        private void ToggleLink(UIMoleculeNode nodeA, UIMoleculeNode nodeB)
//        {
//            UILinkRenderer existing = _activeLinks.Find(l =>
//                (l.m_nodeA == nodeA && l.m_nodeB == nodeB) ||
//                (l.m_nodeA == nodeB && l.m_nodeB == nodeA));

//            if (existing != null)
//            {
//                AudioManager.PlayBreak();
//                _activeLinks.Remove(existing);
//                Destroy(existing.gameObject);
//                UpdateWorkspaceState();
//                return;
//            }

//            // Check if this new link crosses any existing link (>50% line intersection check)
//            Vector2 p1 = nodeA.m_rectTransform.anchoredPosition;
//            Vector2 p2 = nodeB.m_rectTransform.anchoredPosition;

//            UILinkRenderer crossedExisting = null;
//            foreach (var l in _activeLinks)
//            {
//                if (l == null || l.m_nodeA == null || l.m_nodeB == null) continue;

//                // Endpoints shared = valid multi-node junction
//                if (l.m_nodeA == nodeA || l.m_nodeA == nodeB || l.m_nodeB == nodeA || l.m_nodeB == nodeB)
//                    continue;

//                Vector2 p3 = l.m_nodeA.m_rectTransform.anchoredPosition;
//                Vector2 p4 = l.m_nodeB.m_rectTransform.anchoredPosition;

//                if (CheckLineIntersection(p1, p2, p3, p4, out float t, out float u))
//                {
//                    crossedExisting = l;
//                    break;
//                }
//            }

//            if (crossedExisting != null)
//            {
//                AudioManager.PlayBreak();
//                ShowLinkBreakAlert("BOND INTERFERENCE: Link crossed existing covalent bond! Bond severed.");
//                return;
//            }

//            GameObject lineObj = new GameObject("LinkLine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//            lineObj.transform.SetParent(_linesContainer, false);
//            UILinkRenderer link = lineObj.AddComponent<UILinkRenderer>();
//            link.Initialize(nodeA, nodeB);
//            link.OnLinkSeverRequested += (l) =>
//            {
//                AudioManager.PlayBreak();
//                _activeLinks.Remove(l);
//                Destroy(l.gameObject);
//                UpdateWorkspaceState();
//            };

//            AudioManager.PlayBond();
//            _activeLinks.Add(link);
//            UpdateWorkspaceState();
//        }

//        private void CheckAndBreakIntersectingLinks()
//        {
//            if (_activeLinks == null || _activeLinks.Count < 2) return;

//            List<UILinkRenderer> linksToSever = new List<UILinkRenderer>();

//            for (int i = 0; i < _activeLinks.Count; i++)
//            {
//                var linkA = _activeLinks[i];
//                if (linkA == null || linkA.m_nodeA == null || linkA.m_nodeB == null) continue;

//                Vector2 a1 = linkA.m_nodeA.m_rectTransform.anchoredPosition;
//                Vector2 a2 = linkA.m_nodeB.m_rectTransform.anchoredPosition;

//                for (int j = i + 1; j < _activeLinks.Count; j++)
//                {
//                    var linkB = _activeLinks[j];
//                    if (linkB == null || linkB.m_nodeA == null || linkB.m_nodeB == null) continue;

//                    if (linkA.m_nodeA == linkB.m_nodeA || linkA.m_nodeA == linkB.m_nodeB ||
//                        linkA.m_nodeB == linkB.m_nodeA || linkA.m_nodeB == linkB.m_nodeB)
//                    {
//                        continue;
//                    }

//                    Vector2 b1 = linkB.m_nodeA.m_rectTransform.anchoredPosition;
//                    Vector2 b2 = linkB.m_nodeB.m_rectTransform.anchoredPosition;

//                    if (CheckLineIntersection(a1, a2, b1, b2, out float t, out float u))
//                    {
//                        if (!linksToSever.Contains(linkB)) linksToSever.Add(linkB);
//                    }
//                }
//            }

//            if (linksToSever.Count > 0)
//            {
//                AudioManager.PlayBreak();
//                foreach (var link in linksToSever)
//                {
//                    _activeLinks.Remove(link);
//                    if (link != null) Destroy(link.gameObject);
//                }

//                UpdateWorkspaceState();
//                ShowLinkBreakAlert("BOND COLLISION: Covalent links crossed during drift! Bond severed.");
//            }
//        }

//        private bool CheckLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out float t, out float u)
//        {
//            t = 0f;
//            u = 0f;

//            Vector2 d1 = p2 - p1;
//            Vector2 d2 = p4 - p3;
//            float cross = d1.x * d2.y - d1.y * d2.x;

//            if (Mathf.Abs(cross) < 0.0001f) return false;

//            Vector2 d3 = p3 - p1;
//            t = (d3.x * d2.y - d3.y * d2.x) / cross;
//            u = (d3.x * d1.y - d3.y * d1.x) / cross;

//            // Crosses within the active segments (> 5% from endpoints)
//            return (t >= 0.05f && t <= 0.95f && u >= 0.05f && u <= 0.95f);
//        }

//        private void ShowLinkBreakAlert(string message)
//        {
//            if (_liveEffectBannerText != null)
//            {
//                _liveEffectBannerText.text = $"<color=#EF4444>[!] {message}</color>";
//            }
//        }

//        private void ClearAllBonds()
//        {
//            if (_activeLinks.Count > 0)
//            {
//                AudioManager.PlayBreak();
//            }

//            foreach (var link in _activeLinks)
//            {
//                if (link != null) Destroy(link.gameObject);
//            }
//            _activeLinks.Clear();
//            UpdateWorkspaceState();
//        }

//        private void OnValidateClicked()
//        {
//            HashSet<UIMoleculeNode> bondedNodes = new HashSet<UIMoleculeNode>();
//            List<(MoleculeType a, MoleculeType b)> activeBonds = new List<(MoleculeType a, MoleculeType b)>();

//            foreach (var link in _activeLinks)
//            {
//                if (link != null && link.m_nodeA != null && link.m_nodeB != null)
//                {
//                    bondedNodes.Add(link.m_nodeA);
//                    bondedNodes.Add(link.m_nodeB);
//                    activeBonds.Add((link.m_nodeA.m_moleculeType, link.m_nodeB.m_moleculeType));
//                }
//            }

//            List<MoleculeType> bondedMolecules = new List<MoleculeType>();
//            foreach (var node in bondedNodes)
//            {
//                if (node != null) bondedMolecules.Add(node.m_moleculeType);
//            }

//            OnValidateRequested?.Invoke(bondedMolecules, activeBonds);
//        }

//        private void BuildRecipeBookModal()
//        {
//            var t = UIStyleHelper.Theme;

//            _recipeBookPanel = UIStyleHelper.CreatePanel(transform, "RecipeBookModal", t.m_panelBg);
//            RectTransform rbRt = _recipeBookPanel.GetComponent<RectTransform>();
//            rbRt.anchorMin = new Vector2(0.12f, 0.06f);
//            rbRt.anchorMax = new Vector2(0.88f, 0.94f);
//            rbRt.offsetMin = Vector2.zero;
//            rbRt.offsetMax = Vector2.zero;

//            GameObject headerObj = UIStyleHelper.CreatePanel(_recipeBookPanel.transform, "Header", t.m_headerBg);
//            RectTransform hRt = headerObj.GetComponent<RectTransform>();
//            hRt.anchorMin = new Vector2(0.01f, 0.90f);
//            hRt.anchorMax = new Vector2(0.99f, 0.98f);
//            hRt.offsetMin = Vector2.zero;
//            hRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(headerObj.transform, "Title", "CYBER-BARISTA SYNTHESIS BLUEPRINTS", 15, Color.white, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform ttRt = headerObj.transform.Find("Title").GetComponent<RectTransform>();
//            ttRt.anchorMin = new Vector2(0.03f, 0.45f);
//            ttRt.anchorMax = new Vector2(0.70f, 0.95f);
//            ttRt.offsetMin = Vector2.zero;
//            ttRt.offsetMax = Vector2.zero;

//            UIStyleHelper.CreateText(headerObj.transform, "Subtitle", "Target Molecular Formulations & Special Effect Protocols", 9, t.m_textSecondary, TextAlignmentOptions.Left, FontStyles.Normal);
//            RectTransform stRt = headerObj.transform.Find("Subtitle").GetComponent<RectTransform>();
//            stRt.anchorMin = new Vector2(0.03f, 0.05f);
//            stRt.anchorMax = new Vector2(0.70f, 0.45f);
//            stRt.offsetMin = Vector2.zero;
//            stRt.offsetMax = Vector2.zero;

//            Button closeBtn = UIStyleHelper.CreateButton(headerObj.transform, "CloseBtn", "CLOSE [X]", t.m_btnDanger, Color.white, () => _recipeBookPanel.SetActive(false));
//            RectTransform cRt = closeBtn.GetComponent<RectTransform>();
//            cRt.anchorMin = new Vector2(0.84f, 0.15f);
//            cRt.anchorMax = new Vector2(0.98f, 0.85f);
//            cRt.offsetMin = Vector2.zero;
//            cRt.offsetMax = Vector2.zero;

//            // Blueprint Cards Scrollable Container
//            GameObject scrollObj = new GameObject("RecipeScroll", typeof(RectTransform), typeof(ScrollRect));
//            scrollObj.transform.SetParent(_recipeBookPanel.transform, false);
//            RectTransform sRt = scrollObj.GetComponent<RectTransform>();
//            sRt.anchorMin = new Vector2(0.02f, 0.03f);
//            sRt.anchorMax = new Vector2(0.98f, 0.88f);
//            sRt.offsetMin = Vector2.zero;
//            sRt.offsetMax = Vector2.zero;

//            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
//            viewport.transform.SetParent(scrollObj.transform, false);
//            RectTransform vpRt = viewport.GetComponent<RectTransform>();
//            vpRt.anchorMin = Vector2.zero;
//            vpRt.anchorMax = Vector2.one;
//            vpRt.offsetMin = Vector2.zero;
//            vpRt.offsetMax = Vector2.zero;

//            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
//            content.transform.SetParent(viewport.transform, false);
//            _recipeCardsContainer = content.transform;
//            RectTransform cntRt = content.GetComponent<RectTransform>();
//            cntRt.anchorMin = new Vector2(0f, 1f);
//            cntRt.anchorMax = new Vector2(1f, 1f);
//            cntRt.pivot = new Vector2(0.5f, 1f);
//            cntRt.sizeDelta = Vector2.zero;

//            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
//            vlg.spacing = 12;
//            vlg.padding = new RectOffset(6, 6, 8, 8);
//            vlg.childControlWidth = true;
//            vlg.childControlHeight = false;
//            vlg.childForceExpandWidth = true;
//            vlg.childForceExpandHeight = false;

//            ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
//            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

//            ScrollRect sr = scrollObj.GetComponent<ScrollRect>();
//            sr.viewport = vpRt;
//            sr.content = cntRt;
//            sr.horizontal = false;
//            sr.vertical = true;
//            sr.scrollSensitivity = 25f;

//            _recipeBookPanel.SetActive(false);
//        }

//        private void ToggleRecipeBook()
//        {
//            if (_recipeBookPanel != null)
//            {
//                bool willBeActive = !_recipeBookPanel.activeSelf;
//                _recipeBookPanel.SetActive(willBeActive);
//                if (willBeActive)
//                {
//                    _recipeBookPanel.transform.SetAsLastSibling();
//                    PopulateRecipeBook();
//                }
//            }
//        }

//        private void PopulateRecipeBook()
//        {
//            if (_recipeCardsContainer == null) return;

//            foreach (Transform child in _recipeCardsContainer)
//            {
//                Destroy(child.gameObject);
//            }

//            var recipes = _recipeDatabase != null && _recipeDatabase.Count > 0 ?
//                _recipeDatabase : (MolecularBrewingManager.Instance != null ? MolecularBrewingManager.Instance.GetAllRecipes() : DrinkRecipePattern.CreateDefaultRecipes());

//            var t = UIStyleHelper.Theme;
//            int idx = 1;

//            foreach (var recipe in recipes)
//            {
//                bool isDiscovered = MolecularBrewingManager.Instance != null ?
//                    MolecularBrewingManager.Instance.IsRecipeDiscovered(recipe.m_id) : recipe.m_isDiscoveredByDefault;

//                CreateBlueprintCard(_recipeCardsContainer, recipe, idx++, isDiscovered, t);
//            }
//        }

//        private void CreateBlueprintCard(Transform parent, DrinkRecipePattern recipe, int index, bool isDiscovered, UIThemePalette t)
//        {
//            GameObject card = UIStyleHelper.CreatePanel(parent, $"Card_{recipe.m_id}", isDiscovered ? t.m_cardBg : new Color(0.10f, 0.12f, 0.16f, 0.95f));
//            RectTransform cRt = card.GetComponent<RectTransform>();
//            cRt.sizeDelta = new Vector2(0, isDiscovered ? 150 : 135);

//            GameObject tagBar = UIStyleHelper.CreatePanel(card.transform, "TagBar", isDiscovered ? recipe.m_themeColor : new Color(0.85f, 0.20f, 0.25f, 0.9f));
//            RectTransform tbRt = tagBar.GetComponent<RectTransform>();
//            tbRt.anchorMin = new Vector2(0f, 0f);
//            tbRt.anchorMax = new Vector2(0.015f, 1f);
//            tbRt.offsetMin = Vector2.zero;
//            tbRt.offsetMax = Vector2.zero;

//            if (isDiscovered)
//            {
//                UIStyleHelper.CreateText(card.transform, "Title", $"[BLUEPRINT {index:D2}] {recipe.m_displayName.ToUpper()}  <color=#94A3B8>•  \"{recipe.m_tagline}\"</color>", 12, recipe.m_themeColor, TextAlignmentOptions.Left, FontStyles.Bold);
//                RectTransform ttRt = card.transform.Find("Title").GetComponent<RectTransform>();
//                ttRt.anchorMin = new Vector2(0.03f, 0.76f);
//                ttRt.anchorMax = new Vector2(0.97f, 0.96f);
//                ttRt.offsetMin = Vector2.zero;
//                ttRt.offsetMax = Vector2.zero;

//                UIStyleHelper.CreateText(card.transform, "Desc", recipe.m_description, 10, t.m_textSecondary, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform dRt = card.transform.Find("Desc").GetComponent<RectTransform>();
//                dRt.anchorMin = new Vector2(0.03f, 0.52f);
//                dRt.anchorMax = new Vector2(0.97f, 0.74f);
//                dRt.offsetMin = Vector2.zero;
//                dRt.offsetMax = Vector2.zero;

//                GameObject formBox = UIStyleHelper.CreatePanel(card.transform, "FormBox", t.m_wellBg);
//                RectTransform fbRt = formBox.GetComponent<RectTransform>();
//                fbRt.anchorMin = new Vector2(0.03f, 0.08f);
//                fbRt.anchorMax = new Vector2(0.97f, 0.48f);
//                fbRt.offsetMin = Vector2.zero;
//                fbRt.offsetMax = Vector2.zero;

//                List<string> molReqs = new List<string>();
//                foreach (var mr in recipe.m_requiredMolecules)
//                {
//                    molReqs.Add($"{mr.m_minCount}x {mr.m_type}");
//                }
//                string molStr = string.Join("  |  ", molReqs);

//                List<string> bondReqs = new List<string>();
//                foreach (var br in recipe.m_requiredBonds)
//                {
//                    bondReqs.Add($"[{br.m_typeA}] <──(Covalent)──> [{br.m_typeB}]");
//                }
//                string bondStr = bondReqs.Count > 0 ? string.Join("   AND   ", bondReqs) : "None (Free Solution)";

//                UIStyleHelper.CreateText(formBox.transform, "Reqs", $"<color=#F59E0B>• Target Molecules:</color> {molStr}\n<color=#38BDF8>• Chemical Bonds Required:</color> {bondStr}", 10, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform rqRt = formBox.transform.Find("Reqs").GetComponent<RectTransform>();
//                rqRt.anchorMin = new Vector2(0.02f, 0.05f);
//                rqRt.anchorMax = new Vector2(0.98f, 0.95f);
//                rqRt.offsetMin = Vector2.zero;
//                rqRt.offsetMax = Vector2.zero;
//            }
//            else
//            {
//                UIStyleHelper.CreateText(card.transform, "Title", $"[BLUEPRINT {index:D2}] ???  <color=#EF4444>[UNDISCOVERED MOLECULAR BLUEPRINT]</color>", 12, new Color(0.95f, 0.35f, 0.40f, 1f), TextAlignmentOptions.Left, FontStyles.Bold);
//                RectTransform ttRt = card.transform.Find("Title").GetComponent<RectTransform>();
//                ttRt.anchorMin = new Vector2(0.03f, 0.76f);
//                ttRt.anchorMax = new Vector2(0.97f, 0.96f);
//                ttRt.offsetMin = Vector2.zero;
//                ttRt.offsetMax = Vector2.zero;

//                string hintText = string.IsNullOrEmpty(recipe.m_hint) ?
//                    "Formulation classified. Synthesize and link matching reagents in the chamber to decrypt." :
//                    $"<color=#F59E0B>Codex Hint:</color> {recipe.m_hint}";

//                UIStyleHelper.CreateText(card.transform, "Desc", hintText, 10, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform dRt = card.transform.Find("Desc").GetComponent<RectTransform>();
//                dRt.anchorMin = new Vector2(0.03f, 0.52f);
//                dRt.anchorMax = new Vector2(0.97f, 0.74f);
//                dRt.offsetMin = Vector2.zero;
//                dRt.offsetMax = Vector2.zero;

//                GameObject formBox = UIStyleHelper.CreatePanel(card.transform, "FormBox", t.m_wellBg);
//                RectTransform fbRt = formBox.GetComponent<RectTransform>();
//                fbRt.anchorMin = new Vector2(0.03f, 0.08f);
//                fbRt.anchorMax = new Vector2(0.97f, 0.48f);
//                fbRt.offsetMin = Vector2.zero;
//                fbRt.offsetMax = Vector2.zero;

//                UIStyleHelper.CreateText(formBox.transform, "Reqs", "<color=#64748B>• Target Formulation:</color> <color=#94A3B8>[ ??? REDACTED REAGENT RATIOS ??? ]</color>\n<color=#64748B>• Molecular Links:</color> <color=#94A3B8>[ ??? UNRESOLVED COVALENT BONDS - SYNTHESIZE TO DECRYPT ??? ]</color>", 10, t.m_textMuted, TextAlignmentOptions.Left, FontStyles.Normal);
//                RectTransform rqRt = formBox.transform.Find("Reqs").GetComponent<RectTransform>();
//                rqRt.anchorMin = new Vector2(0.02f, 0.05f);
//                rqRt.anchorMax = new Vector2(0.98f, 0.95f);
//                rqRt.offsetMin = Vector2.zero;
//                rqRt.offsetMax = Vector2.zero;
//            }
//        }

//        private bool HasSerializedBindings()
//        {
//            return _nodesContainer != null && _linesContainer != null;
//        }

//        private void BindExistingHierarchy()
//        {
//            if (_freezeToggleBtn != null)
//            {
//                _freezeToggleBtn.onClick.AddListener(ToggleFreezeMolecules);
//            }
//            if (_rescatterBtn != null)
//            {
//                _rescatterBtn.onClick.AddListener(SpawnScatteredMolecules);
//            }
//            if (_clearLinksBtn != null)
//            {
//                _clearLinksBtn.onClick.AddListener(ClearAllBonds);
//            }
//            if (_validateBtn != null)
//            {
//                _validateBtn.onClick.AddListener(OnValidateClicked);
//            }
//            if (_backBtn != null)
//            {
//                _backBtn.onClick.AddListener(() => OnBackToPantryRequested?.Invoke());
//            }
//            if (_blueprintsBtn != null)
//            {
//                _blueprintsBtn.onClick.AddListener(ToggleRecipeBook);
//            }

//            if (_tempLinkRenderer == null && _linesContainer != null)
//            {
//                GameObject tempLineObj = new GameObject("TempLine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//                tempLineObj.transform.SetParent(_linesContainer, false);
//                _tempLinkRenderer = tempLineObj.AddComponent<UILinkRenderer>();
//                _tempLinkRenderer.gameObject.SetActive(false);
//            }

//            if (_recipeBookPanel != null)
//            {
//                _recipeBookPanel.SetActive(false);
//            }

//            if (_meterNeuralCalm != null) _meterNeuralCalm.Initialize("Neural Calm (Anxiety)", new Color(0.05f, 0.70f, 0.60f, 1f));
//            if (_meterSedation != null) _meterSedation.Initialize("Sedation (Deep Sleep)", new Color(0.35f, 0.25f, 0.80f, 1f));
//            if (_meterFocus != null) _meterFocus.Initialize("Cognitive Focus (Energy)", new Color(0.95f, 0.55f, 0.15f, 1f));
//            if (_meterEuphoria != null) _meterEuphoria.Initialize("Euphoria (Mood Lift)", new Color(0.90f, 0.20f, 0.45f, 1f));
//            if (_meterSweetness != null) _meterSweetness.Initialize("Sweetness Balance", new Color(0.95f, 0.75f, 0.20f, 1f));
//            if (_meterBitterness != null) _meterBitterness.Initialize("Bitterness / Astringency", new Color(0.40f, 0.30f, 0.20f, 1f));
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private Transform _chamberViewport;
//        [SerializeField] private Transform _linesContainer;
//        [SerializeField] private Transform _nodesContainer;
//        [SerializeField] private Button _freezeToggleBtn;
//        [SerializeField] private TextMeshProUGUI _freezeToggleText;
//        [SerializeField] private Button _rescatterBtn;
//        [SerializeField] private Button _clearLinksBtn;
//        [SerializeField] private Button _validateBtn;
//        [SerializeField] private Button _backBtn;
//        [SerializeField] private Button _blueprintsBtn;
//        [SerializeField] private TextMeshProUGUI _activeLinkCountText;
//        [SerializeField] private GameObject _liveEffectBannerPanel;
//        [SerializeField] private TextMeshProUGUI _liveEffectBannerText;
//        [SerializeField] private TextMeshProUGUI _liveRecipeMatchText;
//        [SerializeField] private GameObject _recipeBookPanel;
//        [SerializeField] private Transform _recipeCardsContainer;

//        [SerializeField] private UIStatMeter _meterNeuralCalm;
//        [SerializeField] private UIStatMeter _meterSedation;
//        [SerializeField] private UIStatMeter _meterFocus;
//        [SerializeField] private UIStatMeter _meterEuphoria;
//        [SerializeField] private UIStatMeter _meterSweetness;
//        [SerializeField] private UIStatMeter _meterBitterness;

//        private List<IngredientDefinition> _selectedIngredients;
//        private List<DrinkRecipePattern> _recipeDatabase;
//        private List<SpecialEffectDefinition> _effectDatabase;
//        private Func<MoleculeType, MoleculeDefinition> _defLookup;
//        private Canvas _rootCanvas;

//        private List<UIMoleculeNode> _spawnedNodes = new List<UIMoleculeNode>();
//        private List<UILinkRenderer> _activeLinks = new List<UILinkRenderer>();

//        private bool _isDrawingLink;
//        private UIMoleculeNode _linkOriginNode;
//        private UILinkRenderer _tempLinkRenderer;
//        private bool _isMoleculesFrozen = false;

//        #endregion
//    }
//}

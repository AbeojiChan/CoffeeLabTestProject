using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    [Serializable]
    public class MoleculeBond
    {
        public FloatingMoleculeNode m_nodeA;
        public FloatingMoleculeNode m_nodeB;
        public GameObject m_lineObject;
    }

    public class MolecularSynthesisScreen : MonoBehaviour
    {
        #region Publics

        [Header("Reaction Crucible (Circular Confinement)")]
        public RectTransform m_crucibleContainer;
        public Image m_crucibleCircleBorder;
        public Image m_liquidLevelImage;
        public float m_crucibleRadius = 320f;
        public GameObject m_moleculePrefab;

        [Header("Codex Drawer (Right Panel)")]
        public RetractableSidePanel m_recipeCodexDrawer;
        public RecipeCodexPanel m_recipeCodexPanel;

        [Header("Apparatus Controls (Right Side)")]
        public Image m_injectorApparatusImage;
        public Button m_synthesizeButton;
        public Button m_clearBondsButton;
        public Button m_backToPrepButton;
        public Text m_statusPromptText;
        public Text m_bondCounterText;

        [Header("Result Modal")]
        public BrewResultModal m_brewResultModal;

        [Header("Screen Control")]
        public CanvasGroup m_screenCanvasGroup;

        public event Action OnBackToPrepMachine;
        public event Action<RecipeData> OnSynthesisSuccess;
        public event Action OnSessionFinished;

        public bool IsScreenOpen => _isOpen;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_screenCanvasGroup == null)
            {
                m_screenCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (m_synthesizeButton != null)
            {
                m_synthesizeButton.onClick.AddListener(ExecuteSynthesis);
            }

            if (m_clearBondsButton != null)
            {
                m_clearBondsButton.onClick.AddListener(ClearAllBonds);
            }

            if (m_backToPrepButton != null)
            {
                m_backToPrepButton.onClick.AddListener(HandleBackToPrep);
            }

            if (m_brewResultModal == null)
            {
                m_brewResultModal = GetComponentInChildren<BrewResultModal>(true) ?? FindFirstObjectByType<BrewResultModal>();
            }

            if (m_brewResultModal != null)
            {
                m_brewResultModal.OnDrinkServed -= HandleDrinkServed;
                m_brewResultModal.OnDrinkServed += HandleDrinkServed;
                m_brewResultModal.OnDrinkDiscarded -= HandleDrinkDiscarded;
                m_brewResultModal.OnDrinkDiscarded += HandleDrinkDiscarded;
            }
        }

        private void Update()
        {
            if (!_isOpen) return;

            // Keep bond lines updated with moving molecule positions
            UpdateBondLinePositions();
        }

        #endregion


        #region Main API

        public void OpenWithPreparedIngredients(List<RawIngredientItemData> preparedIngredients)
        {
            _activeIngredients = preparedIngredients != null
                ? new List<RawIngredientItemData>(preparedIngredients)
                : new List<RawIngredientItemData>();

            ClearAllBonds();
            SpawnMoleculesFromIngredients(_activeIngredients);
            UpdateUI();
            SetScreenVisible(true);
        }

        public void CloseScreen()
        {
            SetScreenVisible(false);
        }

        public void ClearAllBonds()
        {
            foreach (var bond in _activeBonds)
            {
                if (bond.m_lineObject != null) Destroy(bond.m_lineObject);
            }
            _activeBonds.Clear();

            DestroyPreviewLine();

            if (_selectedNode != null)
            {
                _selectedNode.SetSelected(false);
                _selectedNode = null;
            }

            UpdateUI();
        }

        public void ExecuteSynthesis()
        {
            Debug.Log($"<color=cyan><b>[MolecularSynthesis]</b> Synthesizing with {_activeBonds.Count} active bonds...</color>");

            // 1. Separate base ingredients from additives
            List<RawIngredientItemData> baseIngredients = _activeIngredients.FindAll(i => i != null && i.m_ingredientType == RawIngredientType.Base);
            List<RawIngredientItemData> additiveIngredients = _activeIngredients.FindAll(i => i != null && i.m_ingredientType == RawIngredientType.Additive);

            // 2. Evaluate Base Recipe match
            RecipeData matchedRecipe = null;
            if (m_recipeCodexPanel != null && m_recipeCodexPanel.m_recipes != null)
            {
                foreach (var r in m_recipeCodexPanel.m_recipes)
                {
                    if (r != null && DoesIngredientsMatch(r.m_requiredRawIngredients, baseIngredients))
                    {
                        matchedRecipe = r;
                        matchedRecipe.Unlock();
                        break;
                    }
                }
            }

            // 3. Evaluate Additive Synergies match
            List<AdditiveEffectData> matchedAdditiveEffects = new List<AdditiveEffectData>();
            if (m_recipeCodexPanel != null && m_recipeCodexPanel.m_additiveEffects != null && additiveIngredients.Count > 0)
            {
                foreach (var eff in m_recipeCodexPanel.m_additiveEffects)
                {
                    if (eff != null && eff.MatchesAdditives(additiveIngredients))
                    {
                        eff.Unlock();
                        matchedAdditiveEffects.Add(eff);
                    }
                }
            }

            // 4. Update prompt & trigger success event
            if (m_statusPromptText != null)
            {
                if (matchedRecipe != null)
                {
                    m_statusPromptText.text = $"<color=#20DF80><b>SYNTHÈSE RÉUSSIE !</b> {matchedRecipe.m_recipeName} formulé avec succès.</color>";
                }
                else
                {
                    m_statusPromptText.text = "<color=#FF8844><b>MÉLANGE INSTABLE :</b> Aucun blueprint répertorié pour cette combinaison.</color>";
                }
            }

            OnSynthesisSuccess?.Invoke(matchedRecipe);

            // 5. Open Brew Result Modal (Always opens: whether known recipe or failed Barista Trial!)
            if (m_brewResultModal == null)
            {
                m_brewResultModal = GetComponentInChildren<BrewResultModal>(true) ?? FindFirstObjectByType<BrewResultModal>();
            }

            if (m_brewResultModal != null)
            {
                m_brewResultModal.DisplayResult(matchedRecipe, matchedAdditiveEffects, _activeIngredients);
            }
            else
            {
                Debug.LogWarning("[MolecularSynthesisScreen] No BrewResultModal found in hierarchy! Please run 'Tools > Preparation 0.2 > Build Molecular Synthesis Screen'.");
            }
        }

        #endregion


        #region Tools and Utilities

        private void HandleBackToPrep()
        {
            CloseScreen();
            OnBackToPrepMachine?.Invoke();
        }

        private void SetScreenVisible(bool visible)
        {
            _isOpen = visible;
            if (m_screenCanvasGroup != null)
            {
                m_screenCanvasGroup.alpha = visible ? 1f : 0f;
                m_screenCanvasGroup.interactable = visible;
                m_screenCanvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        private void SpawnMoleculesFromIngredients(List<RawIngredientItemData> ingredients)
        {
            foreach (var node in _spawnedNodes)
            {
                if (node != null) Destroy(node.gameObject);
            }
            _spawnedNodes.Clear();

            if (m_crucibleContainer == null || ingredients == null) return;

            Vector2 center = Vector2.zero;

            foreach (var ing in ingredients)
            {
                if (ing == null || ing.m_yieldMoleculeNames == null) continue;

                foreach (var molName in ing.m_yieldMoleculeNames)
                {
                    if (string.IsNullOrEmpty(molName)) continue;
                    CreateMoleculeNode(molName, ing.m_themeColor, center);
                }
            }
        }

        private void CreateMoleculeNode(string moleculeName, Color themeColor, Vector2 center)
        {
            GameObject obj = null;
            if (m_moleculePrefab != null)
            {
                obj = Instantiate(m_moleculePrefab, m_crucibleContainer);
            }
            else
            {
                obj = new GameObject($"Mol_{moleculeName}", typeof(RectTransform), typeof(Image));
                obj.transform.SetParent(m_crucibleContainer, false);
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(56, 56);
            }

            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float dist = UnityEngine.Random.Range(20f, m_crucibleRadius * 0.75f);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            FloatingMoleculeNode node = obj.GetComponent<FloatingMoleculeNode>() ?? obj.AddComponent<FloatingMoleculeNode>();
            node.Setup(moleculeName, themeColor, m_crucibleRadius, center);

            node.OnMoleculeClicked += HandleNodeClicked;

            node.OnBondDrawStart += HandleBondDrawStart;
            node.OnBondDrawUpdate += HandleBondDrawUpdate;
            node.OnBondDrawEnd += HandleBondDrawEnd;

            _spawnedNodes.Add(node);
        }

        private void HandleNodeClicked(FloatingMoleculeNode clickedNode)
        {
            if (clickedNode == null) return;

            if (_selectedNode == null)
            {
                _selectedNode = clickedNode;
                _selectedNode.SetSelected(true);
            }
            else if (_selectedNode == clickedNode)
            {
                _selectedNode.SetSelected(false);
                _selectedNode = null;
            }
            else
            {
                ToggleBond(_selectedNode, clickedNode);
                _selectedNode.SetSelected(false);
                _selectedNode = null;
            }
        }

        private void HandleBondDrawStart(FloatingMoleculeNode startNode, Vector2 mousePos)
        {
            if (startNode == null) return;
            startNode.SetSelected(true);
            EnsurePreviewLine(startNode.RectTransform.anchoredPosition, mousePos);
        }

        private void HandleBondDrawUpdate(FloatingMoleculeNode startNode, Vector2 mousePos)
        {
            if (startNode == null) return;
            UpdatePreviewLine(startNode.RectTransform.anchoredPosition, mousePos);
        }

        private void HandleBondDrawEnd(FloatingMoleculeNode startNode, FloatingMoleculeNode targetNode)
        {
            DestroyPreviewLine();

            if (startNode != null)
            {
                startNode.SetSelected(false);
            }

            if (startNode != null && targetNode != null && startNode != targetNode)
            {
                ToggleBond(startNode, targetNode);
            }
        }

        private void EnsurePreviewLine(Vector2 startPos, Vector2 currentPos)
        {
            if (_previewBondLine == null)
            {
                _previewBondLine = new GameObject("PreviewBondLine", typeof(RectTransform), typeof(Image));
                _previewBondLine.transform.SetParent(m_crucibleContainer, false);
                _previewBondLine.transform.SetAsFirstSibling();

                Image img = _previewBondLine.GetComponent<Image>();
                img.color = new Color(0.95f, 0.85f, 0.25f, 0.85f);
                img.raycastTarget = false;
            }
            UpdatePreviewLine(startPos, currentPos);
        }

        private void UpdatePreviewLine(Vector2 startPos, Vector2 currentPos)
        {
            if (_previewBondLine == null) return;

            Vector2 dir = currentPos - startPos;
            float length = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            RectTransform rect = _previewBondLine.GetComponent<RectTransform>();
            rect.anchoredPosition = (startPos + currentPos) * 0.5f;
            rect.sizeDelta = new Vector2(length, 3f);
            rect.localEulerAngles = new Vector3(0, 0, angle);
        }

        private void DestroyPreviewLine()
        {
            if (_previewBondLine != null)
            {
                Destroy(_previewBondLine);
                _previewBondLine = null;
            }
        }

        private void ToggleBond(FloatingMoleculeNode a, FloatingMoleculeNode b)
        {
            var existing = _activeBonds.Find(bond =>
                (bond.m_nodeA == a && bond.m_nodeB == b) ||
                (bond.m_nodeA == b && bond.m_nodeB == a));

            if (existing != null)
            {
                if (existing.m_lineObject != null) Destroy(existing.m_lineObject);
                _activeBonds.Remove(existing);
            }
            else
            {
                GameObject lineObj = new GameObject("BondLine", typeof(RectTransform), typeof(Image));
                lineObj.transform.SetParent(m_crucibleContainer, false);
                lineObj.transform.SetAsFirstSibling();

                Image img = lineObj.GetComponent<Image>();
                img.color = new Color(0.20f, 0.85f, 0.95f, 0.85f);
                img.raycastTarget = false;

                _activeBonds.Add(new MoleculeBond
                {
                    m_nodeA = a,
                    m_nodeB = b,
                    m_lineObject = lineObj
                });
            }

            UpdateUI();
        }

        private void UpdateBondLinePositions()
        {
            foreach (var bond in _activeBonds)
            {
                if (bond.m_lineObject == null || bond.m_nodeA == null || bond.m_nodeB == null) continue;

                Vector2 posA = bond.m_nodeA.RectTransform.anchoredPosition;
                Vector2 posB = bond.m_nodeB.RectTransform.anchoredPosition;

                Vector2 dir = posB - posA;
                float length = dir.magnitude;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                RectTransform lineRect = bond.m_lineObject.GetComponent<RectTransform>();
                lineRect.anchoredPosition = (posA + posB) * 0.5f;
                lineRect.sizeDelta = new Vector2(length, 4f);
                lineRect.localEulerAngles = new Vector3(0, 0, angle);
            }
        }

        private void UpdateUI()
        {
            if (m_bondCounterText != null)
            {
                m_bondCounterText.text = $"Liaisons: {_activeBonds.Count} / {_spawnedNodes.Count}";
            }

            if (m_statusPromptText != null && !_isOpen)
            {
                m_statusPromptText.text = "Cliquez-droit & glissez entre 2 molécules pour créer des liaisons chimiques.";
            }
        }

        private bool DoesIngredientsMatch(List<RawIngredientItemData> recipeReqs, List<RawIngredientItemData> currentItems)
        {
            if (recipeReqs == null || currentItems == null) return false;
            if (recipeReqs.Count != currentItems.Count) return false;

            List<string> reqIds = new List<string>();
            foreach (var r in recipeReqs) if (r != null) reqIds.Add(r.m_id);

            List<string> curIds = new List<string>();
            foreach (var c in currentItems) if (c != null) curIds.Add(c.m_id);

            reqIds.Sort();
            curIds.Sort();

            for (int i = 0; i < reqIds.Count; i++)
            {
                if (reqIds[i] != curIds[i]) return false;
            }

            return true;
        }

        private void HandleDrinkServed(RecipeData recipe, List<AdditiveEffectData> additiveEffects)
        {
            Debug.Log($"<color=green><b>[MolecularSynthesis]</b> Drink successfully served to customer!</color>");
            CloseScreen();

            // Return all the way to tablet screen and reset
            var machine = FindFirstObjectByType<PreparationMachineScreen>();
            if (machine != null) machine.CloseScreen();

            var tablet = FindFirstObjectByType<PreparationTabletScreen>();
            if (tablet != null && tablet.m_trayController != null)
            {
                tablet.m_trayController.ResetTray();
            }

            OnSessionFinished?.Invoke();
        }

        private void HandleDrinkDiscarded()
        {
            Debug.Log("<color=yellow><b>[MolecularSynthesis]</b> Drink discarded. Returning to preparation.</color>");
            CloseScreen();

            var machine = FindFirstObjectByType<PreparationMachineScreen>();
            if (machine != null) machine.CloseScreen();

            var tablet = FindFirstObjectByType<PreparationTabletScreen>();
            if (tablet != null && tablet.m_trayController != null)
            {
                tablet.m_trayController.ResetTray();
            }

            OnSessionFinished?.Invoke();
        }

        #endregion


        #region Private and Protected

        private bool _isOpen = false;
        private FloatingMoleculeNode _selectedNode;
        private GameObject _previewBondLine;
        private List<RawIngredientItemData> _activeIngredients = new List<RawIngredientItemData>();
        private readonly List<FloatingMoleculeNode> _spawnedNodes = new List<FloatingMoleculeNode>();
        private readonly List<MoleculeBond> _activeBonds = new List<MoleculeBond>();

        #endregion
    }
}

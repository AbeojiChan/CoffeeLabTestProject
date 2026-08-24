using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace MolecularBrewing.Runtime
{
    [DefaultExecutionOrder(-100)]
    public class MolecularBrewingBootstrap : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            EnsureSceneSetup();
        }

        #endregion


        #region Main API

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoInitializeOnPlay()
        {
            if (FindFirstObjectByType<MolecularBrewingManager>() == null && FindFirstObjectByType<MolecularBrewingBootstrap>() == null)
            {
                GameObject bootObj = new GameObject("MolecularBrewingBootstrap", typeof(MolecularBrewingBootstrap));
                var boot = bootObj.GetComponent<MolecularBrewingBootstrap>();
                boot.EnsureSceneSetup();
            }
        }

        public void EnsureSceneSetup()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                camObj.tag = "MainCamera";
                mainCam = camObj.GetComponent<Camera>();
                camObj.transform.position = new Vector3(0, 0, -10);
            }
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.06f, 0.08f, 0.10f, 1f);

            EventSystem es = FindFirstObjectByType<EventSystem>();
            if (es == null)
            {
                GameObject esObj = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
                esObj.AddComponent<InputSystemUIInputModule>();
#else
                esObj.AddComponent<StandaloneInputModule>();
#endif
            }

            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("CoffeeChemistryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            MolecularBrewingManager manager = FindFirstObjectByType<MolecularBrewingManager>();
            if (manager == null)
            {
                GameObject mgrObj = new GameObject("MolecularBrewingManager", typeof(MolecularBrewingManager));
                manager = mgrObj.GetComponent<MolecularBrewingManager>();
            }

            UISelectionScreen selectionScreen = canvas.GetComponentInChildren<UISelectionScreen>(true);
            if (selectionScreen == null)
            {
                GameObject selObj = new GameObject("UISelectionScreen", typeof(RectTransform));
                selObj.transform.SetParent(canvas.transform, false);
                selectionScreen = selObj.AddComponent<UISelectionScreen>();
                selectionScreen.Initialize(manager.GetAllIngredients());
            }

            UIMolecularWorkspaceScreen workspaceScreen = canvas.GetComponentInChildren<UIMolecularWorkspaceScreen>(true);
            if (workspaceScreen == null)
            {
                GameObject wsObj = new GameObject("UIMolecularWorkspaceScreen", typeof(RectTransform));
                wsObj.transform.SetParent(canvas.transform, false);
                workspaceScreen = wsObj.AddComponent<UIMolecularWorkspaceScreen>();
            }

            UIBrewResultModal resultModal = canvas.GetComponentInChildren<UIBrewResultModal>(true);
            if (resultModal == null)
            {
                GameObject resObj = new GameObject("UIBrewResultModal", typeof(RectTransform));
                resObj.transform.SetParent(canvas.transform, false);
                resultModal = resObj.AddComponent<UIBrewResultModal>();
                resultModal.Initialize();
            }

            UISynthesisLoadingOverlay loadingOverlay = canvas.GetComponentInChildren<UISynthesisLoadingOverlay>(true);
            if (loadingOverlay == null)
            {
                GameObject loadObj = new GameObject("UISynthesisLoadingOverlay", typeof(RectTransform));
                loadObj.transform.SetParent(canvas.transform, false);
                loadingOverlay = loadObj.AddComponent<UISynthesisLoadingOverlay>();
                loadingOverlay.Initialize();
            }

            UIEndOfShiftDialogue dialogueScreen = canvas.GetComponentInChildren<UIEndOfShiftDialogue>(true);
            if (dialogueScreen == null)
            {
                GameObject dlgObj = new GameObject("UIEndOfShiftDialogue", typeof(RectTransform));
                dlgObj.transform.SetParent(canvas.transform, false);
                dialogueScreen = dlgObj.AddComponent<UIEndOfShiftDialogue>();
                dialogueScreen.Initialize();
            }

            UIPersonaCityMapScreen cityMapScreen = canvas.GetComponentInChildren<UIPersonaCityMapScreen>(true);
            if (cityMapScreen == null)
            {
                GameObject mapObj = new GameObject("UIPersonaCityMapScreen", typeof(RectTransform));
                mapObj.transform.SetParent(canvas.transform, false);
                cityMapScreen = mapObj.AddComponent<UIPersonaCityMapScreen>();
                cityMapScreen.Initialize();
            }

            UIRCGShopScreen shopScreen = canvas.GetComponentInChildren<UIRCGShopScreen>(true);
            if (shopScreen == null)
            {
                GameObject shopObj = new GameObject("UIRCGShopScreen", typeof(RectTransform));
                shopObj.transform.SetParent(canvas.transform, false);
                shopScreen = shopObj.AddComponent<UIRCGShopScreen>();
                shopScreen.Initialize(manager.m_playerCredits);
            }

            manager.SetupUI(canvas, selectionScreen, workspaceScreen, resultModal, loadingOverlay, dialogueScreen, cityMapScreen, shopScreen);
        }

        #endregion


        #region Tools and Utilities
        // Bootstrap configuration helpers
        #endregion


        #region Private and Protected
        // Initialization state
        #endregion
    }
}

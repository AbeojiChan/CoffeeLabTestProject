//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;
//using MolecularBrewing.Runtime;

//namespace MolecularBrewing.Editor
//{
//    public static class MolecularBrewingEditorMenu
//    {
//        #region Main API

//        [MenuItem("Coffee Lab/Setup Molecular Brewing Scene in Editor", false, 10)]
//        public static void SetupSceneInEditor()
//        {
//            MolecularBrewingBootstrap bootstrap = Object.FindFirstObjectByType<MolecularBrewingBootstrap>();
//            if (bootstrap == null)
//            {
//                GameObject bootObj = new GameObject("MolecularBrewingBootstrap", typeof(MolecularBrewingBootstrap));
//                bootstrap = bootObj.GetComponent<MolecularBrewingBootstrap>();
//                Undo.RegisterCreatedObjectUndo(bootObj, "Create MolecularBrewingBootstrap");
//            }

//            bootstrap.EnsureSceneSetup();
//            EditorUtility.DisplayDialog("Coffee Lab", "Molecular Brewing Scene Setup completed successfully! You can now press Play.", "OK");
//        }

//        #endregion
//    }
//}
//#endif

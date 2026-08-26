using UnityEngine;
using UnityEngine.UI;

namespace MolecularBrewing.Preparation
{
    public class PreparationTabletScreen : MonoBehaviour
    {
        #region Publics

        [Header("Sub-Panels")]
        public IngredientPantryPanel m_pantryPanel;
        public PreparationTrayController m_trayController;
        public RecipeCodexPanel m_recipeCodexPanel;

        [Header("Side Drawers")]
        public RetractableSidePanel m_leftDrawer;
        public RetractableSidePanel m_rightDrawer;

        [Header("Background & Frame")]
        public Image m_tabletBackground;
        public Image m_tabletBezel;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (m_trayController != null)
            {
                m_trayController.OnProceedToMachine += HandleProceedToMachine;
            }
        }

        #endregion


        #region Main API

        public void HandleProceedToMachine(System.Collections.Generic.List<RawIngredientItemData> chosenIngredients)
        {
            Debug.Log($"[PreparationTabletScreen] Proceeding to Physical 3-Chamber Machine with {chosenIngredients.Count} ingredients loaded on tray!");
            
            if (m_recipeCodexPanel != null)
            {
                m_recipeCodexPanel.CheckAndUnlockRecipeAndEffects(chosenIngredients);
            }
        }

        #endregion


        #region Tools and Utilities
        #endregion


        #region Private and Protected
        #endregion
    }
}

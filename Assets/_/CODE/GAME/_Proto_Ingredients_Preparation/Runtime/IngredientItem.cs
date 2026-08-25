using UnityEngine;

    public class IngredientItem : MonoBehaviour
    {
    #region Publics

    public IngredientsData Data
    {
        get { return m_data; }
    }

    #endregion

    #region Unity API
    #endregion

    #region Main API
    #endregion

    #region Tools and Utilities
    #endregion

    #region Private and Protected

    [SerializeField] private IngredientsData m_data;

	#endregion

    }

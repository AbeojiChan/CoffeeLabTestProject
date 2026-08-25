using UnityEngine;

[CreateAssetMenu(fileName = "IngredientsData", menuName = "Scriptable Objects/IngredientsData", order =1)]
public class IngredientsData : ScriptableObject
{
    [SerializeField] private GameObject m_processedPrefab;
    [SerializeField] private float m_grindDuration = 3f;

    public GameObject ProcessedPrefab => m_processedPrefab;
    public float GrindDuration => m_grindDuration;
}

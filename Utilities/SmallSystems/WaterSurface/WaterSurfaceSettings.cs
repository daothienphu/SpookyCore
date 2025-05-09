using UnityEngine;

namespace SpookyCore.Utilities
{
    [CreateAssetMenu(menuName = "SpookyCore/Misc/Water Surface Settings", fileName = "Water_Surface_Settings")]
    public class WaterSurfaceSettings : ScriptableObject
    {
        [Header("Visual")] 
        [field: SerializeField] public int ColumnCount = 100;
        [field: SerializeField] [Range(0.5f, 2f)] public float ColumnWidth = 1f;
        [field: SerializeField] [Range(1f, 100f)] public float ColumnHeight = 2f;

        [Header("Mechanic")] 
        [field: SerializeField] [Range(0f, 0.5f)] public float Dampening = 0.025f;
        [field: SerializeField] [Range(0f, 0.5f)] public float Tension = 0.025f;
        [field: SerializeField] [Range(0f, 0.5f)] public float Spread = 0.25f;
        [field: SerializeField] [Range(1f,10f)] public int RipplePasses = 8;
    }
}
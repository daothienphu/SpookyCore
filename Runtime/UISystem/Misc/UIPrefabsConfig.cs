using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.UI
{
    [CreateAssetMenu(menuName = "SpookyCore/UI Configs/UI Layer Config", fileName = "UI_Layer_Config")]
    public class UIPrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public List<GameObject> HudLayer;
        [field: SerializeField] public List<GameObject> PopupLayer;
        [field: SerializeField] public List<GameObject> OverlayLayer;
        [field: SerializeField] public List<GameObject> WorldSpaceLayer;
    }
}
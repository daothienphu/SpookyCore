using System.IO;
using SpookyCore.EntitySystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SpookyCore.Editor.EntitySystem
{
    [CustomEditor(typeof(EntityHealth), true)]
    public class EntityHealthEditor : EntityComponentEditor
    {
        private EntityHealth _health;
        private string _requiredPath = "Canvas(Clone)/Slider";
        private static string _defaultCanvasPath;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _health = (EntityHealth)target;
            
            if (_defaultCanvasPath == null)
            {
                _defaultCanvasPath = Path.Combine(SpookyPathResolver.GetSpookyCorePath(), "Runtime/EntitySystem/Utils/Health/Canvas.prefab");
            }

            var sliderTransform = _entity.transform.Find(_requiredPath); 
            if (sliderTransform)
            {
                AssignReferences(sliderTransform);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_health.HealthBar)
            {
                EditorGUILayout.HelpBox("You should assign a world space UI health bar slider.", MessageType.Warning);
            }
            
            if (!_entity.transform.Find(_requiredPath))
            {
                if (GUILayout.Button("Build Health Hierarchy"))
                {
                    var sliderTransform = CreateSlider(_entity.transform);
                    AssignReferences(sliderTransform);
                }
            }
        }

        private Transform CreateSlider(Transform downMostTransform)
        {
            var canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_defaultCanvasPath);
            var canvas = Instantiate(canvasPrefab, downMostTransform, true);
            canvas.transform.localPosition = Vector3.zero;
            
            return canvas.transform.Find("Slider").transform;
        }
        
        protected override void AssignReferences(Transform downMostTransform)
        {
            var slider = downMostTransform.GetComponent<Slider>();
            if (slider)
            {
                _health.HealthBar = slider;
            }
            
            EditorUtility.SetDirty(_entity.gameObject);
            AssetDatabase.SaveAssets();
        }
    }
}
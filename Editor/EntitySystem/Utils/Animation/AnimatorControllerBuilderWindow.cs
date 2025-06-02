using System.IO;
using SpookyCore.Runtime.EntitySystem;
using SpookyCore.Runtime.Utilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    public class AnimatorControllerBuilderWindow : EditorWindow
    {
        private EntityAnimation _targetEntityAnimation;
        private EntityAnimationConfig _config;
        private DefaultAsset _saveFolder;
        private string _animatorName = "Entity_AnimatorController";
        private string _bindingPath = "Visual/MainVisual";

        public static void OpenWindow(EntityAnimation target = null)
        {
            var window = GetWindow<AnimatorControllerBuilderWindow>("Animator Controller Builder");
            window._targetEntityAnimation = target;
            window._config = target?.AnimationConfig;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animator Controller Builder", EditorStyles.boldLabel);
            _config = (EntityAnimationConfig)EditorGUILayout.ObjectField("Animation Config", _config, typeof(EntityAnimationConfig), false);
            _saveFolder = (DefaultAsset)EditorGUILayout.ObjectField("Save Folder", _saveFolder, typeof(DefaultAsset), false);
            _animatorName = EditorGUILayout.TextField("Animator Controller Name", _animatorName);
            _bindingPath = EditorGUILayout.TextField("Target sprite rebinding path", _bindingPath);

            EditorGUILayout.Space();

            if (!_config || !_saveFolder)
            {
                EditorGUILayout.HelpBox("Please assign both the Animation Config and the Save Folder.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Build Animator Controller"))
            {
                BuildAnimatorController();
            }
        }

        private void BuildAnimatorController()
        {
            if (_config.AnimationClips == null || _config.AnimationClips.Count == 0)
            {
                Debug.LogError("No animation clips found in config.");
                return;
            }

            var folderPath = AssetDatabase.GetAssetPath(_saveFolder);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Invalid save folder.");
                return;
            }

            var controllerPath = Path.Combine(folderPath, _animatorName + ".controller").Replace("\\", "/");
            
            if (AssetDatabase.AssetPathExists(controllerPath) && 
                !EditorUtility.DisplayDialog("Overwrite Animator Controller File", $"An animator controller with the same name exists at {controllerPath}.\n Overwrite?", "Yes", "Cancel"))
            {
                Debug.Log("Animator Controller creation cancelled.");
                return;
            }
            
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            var stateMachine = controller.layers[0].stateMachine;

            foreach (var clip in _config.AnimationClips)
            {
                if (!clip) continue;
                RebindSpritePath(clip, _bindingPath);
                var state = stateMachine.AddState(clip.name);
                state.motion = clip;
            }

            if (stateMachine.states.Length > 0)
            {
                stateMachine.defaultState = stateMachine.states[0].state;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Animator Controller".Color("cyan") + " created at: " + controllerPath.Color("cyan"));

            if (_targetEntityAnimation)
            {
                _targetEntityAnimation._animatorController = controller;
                var sprite = GetASpriteFromConfig();
                _targetEntityAnimation.InitAnimatorController(controller, sprite);
                EditorUtility.SetDirty(_targetEntityAnimation.gameObject);
                AssetDatabase.SaveAssets();
            }

            Close();
        }

        private Sprite GetASpriteFromConfig()
        {
            var firstClip = _config.AnimationClips[0];
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(firstClip);

            foreach (var binding in bindings)
            {
                if (binding.propertyName != "m_Sprite") continue;

                var keyframes = AnimationUtility.GetObjectReferenceCurve(firstClip, binding);
                if (keyframes.Length > 0 && keyframes[0].value is Sprite sprite)
                {
                    return sprite;
                }
            }

            return null;
        }
        
        private void RebindSpritePath(AnimationClip clip, string newPath)
        {
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            foreach (var binding in bindings)
            {
                if (binding.propertyName != "m_Sprite") continue;
                
                if (binding.path == newPath) continue;
                
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                
                var newBinding = new EditorCurveBinding
                {
                    path = newPath,
                    type = typeof(SpriteRenderer),
                    propertyName = "m_Sprite"
                };
                
                AnimationUtility.SetObjectReferenceCurve(clip, newBinding, keyframes);
                AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
            }
        }
    }
}

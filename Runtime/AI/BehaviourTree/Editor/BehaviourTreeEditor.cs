using System.Collections.Generic;
using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Runtime.AI.BehaviourTree.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private EntityAIRunner _aiRunner;
        private Vector2 _scrollPosition;
        private Dictionary<AIBehavior, bool> _behaviorFoldouts = new();
        private List<NodeBase> _nodes = new List<NodeBase>();
        
        public static void ShowWindow(EntityAIRunner aiRunner)
        {
            var window = GetWindow<BehaviourTreeEditor>("Behavior Tree Editor");
            window._aiRunner = aiRunner;
            window.Show();
        }

        private void OnGUI()
        {
            if (!_aiRunner)
            {
                GUILayout.Label("No AIRunner selected");
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var behavior in _aiRunner.MainTree.GetChildren())
            {
                DrawNode(behavior);
            }
            EditorGUILayout.EndScrollView();

            if (EditorApplication.isPlaying)
            {
                Repaint();
            }
        }

        private void DrawNode(NodeBase node, int depth = 0, Color? color = null)
        {
            var originalColor = GUI.color;

            if (color.HasValue)
            {
                GUI.color = color.Value;
            }
            else
            {
                GUI.color = node.CurrentState switch
                {
                    NodeState.Running => Color.yellow,
                    NodeState.Failure => Color.red,
                    NodeState.Success => Color.green,
                    _ => Color.gray
                };
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(depth * 20);
            EditorGUILayout.LabelField($"{node.GetType().Name.Replace("Node", "")} {node.Name}", EditorStyles.helpBox);
            EditorGUILayout.EndHorizontal();

            GUI.color = originalColor;
            
            var forceNextColor = color.HasValue;
            foreach (var child in node.GetChildren())
            {
                DrawNode(child, depth + 1, forceNextColor? Color.gray : null);
                if (!forceNextColor && 
                    node is CompositeNodeBase compositeNode &&
                    (child is ExecutionNode || (child is ConditionNode && child.CurrentState == NodeState.Failure)))
                {
                    forceNextColor = true;
                }
            }
        }
    }
}
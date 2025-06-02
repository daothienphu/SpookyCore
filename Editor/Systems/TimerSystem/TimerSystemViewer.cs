using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using SpookyCore.Runtime.Systems;

namespace SpookyCore.Editor.Systems
{
    public class TimerSystemViewer : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("SpookyTools/Systems/Timer System/Timer System Viewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimerSystemViewer>("Timer System Viewer");
            window.Show();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to inspect timers.", MessageType.Info);
                return;
            }

            if (!TimerSystem.Instance)
            {
                EditorGUILayout.HelpBox("TimerSystem not initialized.", MessageType.Warning);
                return;
            }

            var timers = GetTimers();

            EditorGUILayout.LabelField($"Active Timers: {timers.Count}", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var timer in timers)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    DrawTimerInfo(timer);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private List<Timer> GetTimers()
        {
            var field = typeof(TimerSystem).GetField("_activeTimers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(TimerSystem.Instance) as List<Timer> ?? new List<Timer>();
        }

        private void DrawTimerInfo(Timer timer)
        {
            EditorGUILayout.LabelField(timer.GetType().Name, EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Is Running", timer.IsRunning.ToString());
            EditorGUILayout.LabelField("Is Finished", timer.IsFinished.ToString());
            EditorGUILayout.LabelField("Current Time", timer.CurrentTime.ToString("F2"));
            EditorGUILayout.LabelField("Duration", timer.Duration.ToString("F2"));
            EditorGUILayout.Slider("Progress", timer.CurrentTime / timer.Duration, 0f, 1f);
        }
    }
}
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SpookyCore.Editor.Systems
{
	[InitializeOnLoad]
	public static class BootstrapSceneLoader
	{
		private const string EDITOR_PREF_LOAD_BOOTSTRAP_ON_PLAY = "SceneAutoLoader.LoadBootstrapOnPlay";
		private const string EDITOR_PREF_BOOTSRTAP_SCENE = "SceneAutoLoader.BootstrapScene";
		private const string EDITOR_PREF_PREVIOUS_SCENE = "SceneAutoLoader.PreviousScene";
		private static bool LoadBootstrapOnPlay
		{
			get => EditorPrefs.GetBool(EDITOR_PREF_LOAD_BOOTSTRAP_ON_PLAY, false);
			set => EditorPrefs.SetBool(EDITOR_PREF_LOAD_BOOTSTRAP_ON_PLAY, value);
		}
		private static string BootstrapScene
		{
			get => EditorPrefs.GetString(EDITOR_PREF_BOOTSRTAP_SCENE, "Bootstrap.unity");
			set => EditorPrefs.SetString(EDITOR_PREF_BOOTSRTAP_SCENE, value);
		}
		private static string PreviousScene
		{
			get => EditorPrefs.GetString(EDITOR_PREF_PREVIOUS_SCENE, SceneManager.GetActiveScene().path);
			set => EditorPrefs.SetString(EDITOR_PREF_PREVIOUS_SCENE, value);
		}
		
		static BootstrapSceneLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}
		
		[MenuItem("SpookyTools/Systems/Bootstrap System/Select Bootstrap Scene")]
		private static void SelectMasterScene()
		{
			var bootstrapScene = EditorUtility.OpenFilePanel("Select Bootstrap Scene", Application.dataPath, "unity");
			bootstrapScene = bootstrapScene.Replace(Application.dataPath, "Assets");	//project relative instead of absolute path
			if (!string.IsNullOrEmpty(bootstrapScene))
			{
				BootstrapScene = bootstrapScene;
				LoadBootstrapOnPlay = true;
				Debug.Log($"Bootstrap scene selected: {BootstrapScene}.");
				Debug.Log("Enabled load bootstrap scene on play.");
				return;
			}
			Debug.Log("Bootstrap scene not selected.");
		}
	 
		[MenuItem("SpookyTools/Systems/Bootstrap System/Enable Load Bootstrap On Play")]
		private static void EnableLoadBootstrapOnPlay()
		{
			LoadBootstrapOnPlay = true;
			Debug.Log("Enabled load bootstrap scene on play.");
		}
		
		[MenuItem("SpookyTools/Systems/Bootstrap System/Disable Load Bootstrap On Play")]
		private static void DisableLoadMasterOnPlay()
		{
			LoadBootstrapOnPlay = false;
			Debug.Log("Disabled load bootstrap scene on play.");
		}
		
		[MenuItem("SpookyTools/Systems/Bootstrap System/Enable Load Bootstrap On Play", true)]
		private static bool ShouldHighlightEnableLoadBootstrapOnPlay()
		{
			return !LoadBootstrapOnPlay;
		}
		
		[MenuItem("SpookyTools/Systems/Bootstrap System/Disable Load Bootstrap On Play", true)]
		private static bool ShouldHighlightDisableLoadBootstrapOnPlay()
		{
			return LoadBootstrapOnPlay;
		}
		
		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (!LoadBootstrapOnPlay)
			{
				return;
			}
	 
			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				PreviousScene = SceneManager.GetActiveScene().path;
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					try
					{
						EditorSceneManager.OpenScene(BootstrapScene);
					}
					catch
					{
						Debug.LogError($"Error: Scene {BootstrapScene} not found.");
						EditorApplication.isPlaying = false;
					}
				}
				else
				{
					EditorApplication.isPlaying = false;
				}
			}
			
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				try
				{
					EditorSceneManager.OpenScene(PreviousScene);
				}
				catch
				{
					Debug.LogError($"Error: Scene {PreviousScene} not found.");
				}
			}
		}
	}
}

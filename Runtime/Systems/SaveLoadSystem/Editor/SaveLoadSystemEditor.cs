using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.SystemLoader
{
    public class SaveLoadSystemEditor : EditorWindow
    {
        private static string SAVE_FILE_PATH = $"{Application.persistentDataPath}/Abyssalis_save.json";

        [MenuItem("SpookyTools/Systems/Save Load System/Open Save Location")]
        public static void OpenSaveLocation()
        {
            Debug.Log($"<color=cyan>[Save Load]</color> Save path: {SAVE_FILE_PATH}");
            OpenSaveFileLocation();
        }

        [MenuItem("SpookyTools/Systems/Save Load System/Delete Save File")]
        public static void DeleteCurrentSaveFile()
        {
            AskDeleteSaveFile();
        }
        
        private static void OpenSaveFileLocation()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
            else
            {
                Debug.LogWarning("<color=cyan>[Save Load System]</color> Save file directory does not exist.");
            }
        }

        private static void AskDeleteSaveFile()
        {
            if (File.Exists(SAVE_FILE_PATH))
            {
                var confirmDelete = EditorUtility.DisplayDialog(
                    "Delete Save File",
                    "Are you sure you want to delete the save file? This action CANNOT be undone.",
                    "Yes",
                    "No"
                );

                if (confirmDelete)
                {
                    File.Delete(SAVE_FILE_PATH);
                    Debug.Log("Save file deleted.");
                    //EditorUtility.DisplayDialog("Save Load System", "Save file successfully deleted.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Save Load System", "No save file found to delete.", "OK");
            }
        }
    }
}
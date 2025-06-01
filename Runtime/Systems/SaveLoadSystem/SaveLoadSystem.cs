using System;
using System.IO;
using System.Text;
using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.SystemLoader
{
    public class SaveLoadSystem : PersistentMonoSingleton<SaveLoadSystem>, IGameSystem
    {
        private static string SAVE_FILE_PATH;

        protected override void OnStart()
        {
            base.OnStart();
            SAVE_FILE_PATH = $"{Application.persistentDataPath}/Abyssalis_save.json";
            Debug.Log("<color=cyan>[Save Load]</color> system ready.");
        }

        public static void Save<T>(T data)
        {
            try
            {
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SAVE_FILE_PATH, json);
                Debug.Log($"Data saved to {SAVE_FILE_PATH}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data: {e}");
            }
        }

        public static T Load<T>() where T : new ()
        {
            try
            {
                if (File.Exists(SAVE_FILE_PATH))
                {
                    var json = File.ReadAllText(SAVE_FILE_PATH);
                    return JsonUtility.FromJson<T>(json);
                }
                else
                {
                    Debug.LogWarning("Save file not found. Returning default data.");
                    return new T();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e}");
                return new T();
            }
        }
        
        public static void DeleteSaveFile()
        {
            if (File.Exists(SAVE_FILE_PATH))
            {
                File.Delete(SAVE_FILE_PATH);
                Debug.Log("Save file deleted.");
            }
        }

        private static string Encrypt(string data, string key)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            for (var i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] ^= keyBytes[i % keyBytes.Length];
            }

            return Convert.ToBase64String(dataBytes);
        }

        private static string Decrypt(string data, string key)
        {
            var dataBytes = Convert.FromBase64String(data);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            for (var i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] ^= keyBytes[i % keyBytes.Length];
            }
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets._Project.Scripts.Saving
{
    public static class SaveSystem
    {
        private static string savePath = Application.persistentDataPath + "/save.json";

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Сохранено в " + savePath);
        }

        public static SaveData Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"Загружено из " + savePath);
                return data;
            }
            else
            {
                Debug.Log("Файл сохранения не найден. Создаю новый.");
                return null;
            }
        }

        public static bool SaveExists()
        {
            return File.Exists(savePath);
        }
    }
}
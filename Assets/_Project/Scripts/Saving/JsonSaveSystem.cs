using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Saving
{
    public class JsonSaveSystem : ISaveSystem
    {
        private readonly string saveDirectory;

        public JsonSaveSystem(string directory = null)
        {
            saveDirectory = directory ?? Application.persistentDataPath;
        }

        public void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(saveDirectory, fileName);
            File.WriteAllText(path, json);
        }

        public T Load<T>(string fileName) where T : new()
        {
            string path = Path.Combine(saveDirectory, fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.LogWarning($"File not found: {path}");
                return new T();
            }
        }

        public bool Exists(string fileName)
        {
            return File.Exists(Path.Combine(saveDirectory, fileName));
        }
    }
}

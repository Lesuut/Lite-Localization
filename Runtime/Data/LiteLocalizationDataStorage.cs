using System.IO;
using UnityEngine;

namespace LiteLocalization.Runtime.Data
{
    public class LiteLocalizationDataStorage
    {
        public LiteLocalizationData Data { get; private set; }
        
        private readonly string _folderPath;
        private readonly string _filePath;
        private readonly string _defaultLanguageCode;

        public LiteLocalizationDataStorage(string folderPath, string defaultLanguageCode, string fileName = "LiteLocalizationData.json")
        {
            _folderPath = folderPath;
            _filePath = Path.Combine(_folderPath, fileName);
            _defaultLanguageCode = defaultLanguageCode;
            
            Data = Load();
        }

        public void Save(LiteLocalizationData data = null)
        {
            if (data != null)
                Data = data;

            if (Data == null)
            {
                Debug.LogError("[LiteLocalization] Cannot save null data.");
                return;
            }

            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(_filePath, json);
        }

        private LiteLocalizationData Load()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning($"[LiteLocalization] No saved data found. Creating new file at {_filePath}.");
                var newData = new LiteLocalizationData()
                {
                    LanguageCode = _defaultLanguageCode
                };
                Save(newData);
                return newData;
            }

            string json = File.ReadAllText(_filePath);
            var loadedData = JsonUtility.FromJson<LiteLocalizationData>(json);

            return loadedData;
        }
    }
}

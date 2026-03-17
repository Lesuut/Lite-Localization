using UnityEngine;
using System.Collections.Generic;

namespace LiteLocalization.Runtime.Data
{
    public class LocalizationTable
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data;
        private readonly string _separator;

        public LocalizationTable(TextAsset csvFile, string separator)
        {
            _data = new Dictionary<string, Dictionary<string, string>>();
            _separator = separator;
            ParseCSV(csvFile.text);
        }

        private void ParseCSV(string csvText)
        {
            string[] lines = csvText.Split('\n');

            if (lines.Length < 2)
            {
                return;
            }

            string[] headers = SplitCSVLine(lines[0]);

            if (headers.Length < 2)
            {
                Debug.LogError("CSV must have at least 2 columns");
                return;
            }

            string[] languageCodes = new string[headers.Length - 1];
            for (int i = 1; i < headers.Length; i++)
            {
                languageCodes[i - 1] = headers[i].Trim();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                string[] cells = SplitCSVLine(line);

                if (cells.Length < 2)
                    continue;

                string key = cells[0].Trim();

                if (!_data.ContainsKey(key))
                {
                    _data[key] = new Dictionary<string, string>();
                }

                for (int j = 1; j < cells.Length && j <= languageCodes.Length; j++)
                {
                    string langCode = languageCodes[j - 1];
                    string value = cells[j].Trim();
                    _data[key][langCode] = value;
                }
            }
        }

        private string[] SplitCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == _separator[0] && !inQuotes)
                {
                    result.Add(line.Substring(startIndex, i - startIndex).Trim('"', ' ', '\r'));
                    startIndex = i + 1;
                }
            }

            result.Add(line.Substring(startIndex).Trim('"', ' ', '\r'));
            return result.ToArray();
        }

        public string GetCell(string key, string languageCode)
        {
            if (!_data.ContainsKey(key))
            {
                return $"{key} <i><color=red>{languageCode}</color></i>";
            }

            var translations = _data[key];

            if (translations.ContainsKey(languageCode))
            {
                if (string.IsNullOrEmpty(translations[languageCode]))
                    return $"{key} <i><color=red>{languageCode}</color></i>";
                
                return translations[languageCode];
            }

            Debug.LogWarning($"Translation for '{key}' in '{languageCode}' not found");
            return $"{key} <i><color=red>{languageCode}</color></i>";
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
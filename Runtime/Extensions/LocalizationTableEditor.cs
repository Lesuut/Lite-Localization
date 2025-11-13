using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LiteLocalization.Runtime
{
    public class LocalizationTableEditor
    {
        private readonly TextAsset _textAsset;
        private readonly string _separator;
        private List<List<string>> _table;
        private List<string> _locales;
        private Dictionary<string, int> _localeIndexMap;
        private Dictionary<string, int> _keyIndexMap;

        public LocalizationTableEditor(TextAsset asset, string separator)
        {
            _textAsset = asset;
            _separator = separator;
            ParseTable();
        }

        private void ParseTable()
        {
            _table = new List<List<string>>();
            _localeIndexMap = new Dictionary<string, int>();
            _keyIndexMap = new Dictionary<string, int>();

            string text = _textAsset.text;
            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] cells = lines[i].Split(_separator);
                List<string> row = new List<string>();

                foreach (string cell in cells)
                {
                    row.Add(cell.Trim());
                }

                _table.Add(row);

                if (i == 0)
                {
                    _locales = new List<string>(row);
                    for (int j = 1; j < _locales.Count; j++)
                    {
                        _localeIndexMap[_locales[j]] = j;
                    }
                }
                else
                {
                    if (row.Count > 0)
                    {
                        _keyIndexMap[row[0]] = i;
                    }
                }
            }
        }

        public bool HasLocale(string locale)
        {
            return _localeIndexMap.ContainsKey(locale);
        }

        public void AddLocale(string locale)
        {
            if (HasLocale(locale))
            {
                Debug.LogWarning($"Locale '{locale}' already exists!");
                return;
            }

            _table[0].Add(locale);
            _locales.Add(locale);
            _localeIndexMap[locale] = _table[0].Count - 1;

            for (int i = 1; i < _table.Count; i++)
            {
                _table[i].Add("");
            }

            SaveTable();
        }

        public bool HasKey(string key)
        {
            return _keyIndexMap.ContainsKey(key);
        }

        public void AddKey(string key, string sourceLocale)
        {
            if (HasKey(key))
            {
                return;
            }

            if (!HasLocale(sourceLocale))
            {
                Debug.LogError($"Locale '{sourceLocale}' not found!");
                return;
            }

            List<string> newRow = new List<string>();
            newRow.Add(key);

            for (int i = 1; i < _table[0].Count; i++)
            {
                if (_table[0][i] == sourceLocale)
                    newRow.Add(key);
                else
                    newRow.Add("");
            }

            _table.Add(newRow);
            _keyIndexMap[key] = _table.Count - 1;

            SaveTable();
        }

        public string GetValue(string key, string locale)
        {
            if (!HasKey(key))
            {
                Debug.LogWarning($"Key '{key}' not found!");
                return key;
            }

            if (!HasLocale(locale))
            {
                Debug.LogWarning($"Locale '{locale}' not found!");
                return key;
            }

            int rowIndex = _keyIndexMap[key];
            int colIndex = _localeIndexMap[locale];

            return _table[rowIndex][colIndex];
        }

        public void SetValue(string key, string locale, string value)
        {
            if (!HasKey(key) || !HasLocale(locale))
            {
                Debug.LogError("Key or locale not found!");
                return;
            }

            int rowIndex = _keyIndexMap[key];
            int colIndex = _localeIndexMap[locale];

            _table[rowIndex][colIndex] = value;
            SaveTable();
        }

        private void SaveTable()
        {
            string result = "";

            for (int i = 0; i < _table.Count; i++)
            {
                result += string.Join(_separator, _table[i]);
                if (i < _table.Count - 1)
                    result += "\n";
            }

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(_textAsset);

            if (!string.IsNullOrEmpty(assetPath))
            {
                File.WriteAllText(assetPath, result);
                UnityEditor.AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Failed to find file path!");
            }
        }

        public List<string> GetAllLocales()
        {
            return new List<string>(_locales.Skip(1));
        }

        public List<string> GetAllKeys()
        {
            return new List<string>(_keyIndexMap.Keys);
        }
    }
}

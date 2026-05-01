using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LiteLocalization.Runtime.Data;
using UnityEngine;

namespace LiteLocalization.Runtime
{
    public class LiteLocalizationManager
    {
        private static LiteLocalizationManager _instance;
        public static LiteLocalizationManager Instance 
        {
            get
            {
                if (_instance == null)
                    _instance = new LiteLocalizationManager();
                return _instance;
            }
        }
        
        public static Action OnLanguageChanged;
        
        private readonly LiteLocalizationDataStorage _dataStorage;
        private readonly LocalizationTable _localizationTable;
        private readonly LocalizationTableEditor _localizationTableEditor;
        
        public LiteLocalizationManager()
        {
            _dataStorage = new LiteLocalizationDataStorage(
                Path.Combine(Application.persistentDataPath, "Localization"),
                LiteLocalizationSettings.Instance.DefaultLanguage);

            if (_dataStorage.IsFirstRun)
            {
                var detected = DetectDeviceLanguage();
                if (!string.IsNullOrEmpty(detected) &&
                    LiteLocalizationSettings.Instance.Languages.Any(x => x.languageCode == detected))
                {
                    _dataStorage.Data.LanguageCode = detected;
                    _dataStorage.Save();
                    Debug.Log($"[LiteLocalization] Device language detected: {detected}");
                }
            }

            _localizationTable = new LocalizationTable(
                LiteLocalizationSettings.Instance.LanguagesTextAsset,
                LiteLocalizationSettings.Instance.Separator);
            
            _localizationTableEditor = new LocalizationTableEditor(
                LiteLocalizationSettings.Instance.LanguagesTextAsset,
                LiteLocalizationSettings.Instance.Separator);
            
# if UNITY_EDITOR
            foreach (var languages in LiteLocalizationSettings.Instance.Languages)
            {
                if (!_localizationTableEditor.HasLocale(languages.languageCode))
                {
                    _localizationTableEditor.AddLocale(languages.languageCode);
                }
            }
#endif
        }
        
        public LanguageCodeItem GetCurrentLanguage() => LiteLocalizationSettings.Instance.Languages.First(x => x.languageCode == _dataStorage.Data.LanguageCode);
        
        public LanguageCodeItem[] GetLanguages() => LiteLocalizationSettings.Instance.Languages;
        
        public void SetLocalization(string languageCode)
        {
            _dataStorage.Data.LanguageCode = languageCode;
            _dataStorage.Save();
            OnLanguageChanged?.Invoke();
        }
        
        public static string Translate(string key)
        {
# if UNITY_EDITOR
            if (!Instance._localizationTable.HasKey(key))
                Instance._localizationTableEditor.AddKey(key, LiteLocalizationSettings.Instance.SourceLanguage);
#endif

            return Instance._localizationTable.GetCell(key, Instance._dataStorage.Data.LanguageCode);
        }

        private static string DetectDeviceLanguage()
        {
            try
            {
                return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }
            catch
            {
                return SystemLanguageToCode(Application.systemLanguage);
            }
        }

        private static string SystemLanguageToCode(SystemLanguage language)
        {
            var map = new Dictionary<SystemLanguage, string>
            {
                { SystemLanguage.Afrikaans,        "af" },
                { SystemLanguage.Arabic,           "ar" },
                { SystemLanguage.Basque,           "eu" },
                { SystemLanguage.Belarusian,       "be" },
                { SystemLanguage.Bulgarian,        "bg" },
                { SystemLanguage.Catalan,          "ca" },
                { SystemLanguage.Chinese,          "zh" },
                { SystemLanguage.ChineseSimplified,"zh" },
                { SystemLanguage.ChineseTraditional,"zh" },
                { SystemLanguage.Czech,            "cs" },
                { SystemLanguage.Danish,           "da" },
                { SystemLanguage.Dutch,            "nl" },
                { SystemLanguage.English,          "en" },
                { SystemLanguage.Estonian,         "et" },
                { SystemLanguage.Faroese,          "fo" },
                { SystemLanguage.Finnish,          "fi" },
                { SystemLanguage.French,           "fr" },
                { SystemLanguage.German,           "de" },
                { SystemLanguage.Greek,            "el" },
                { SystemLanguage.Hebrew,           "he" },
                { SystemLanguage.Hungarian,        "hu" },
                { SystemLanguage.Icelandic,        "is" },
                { SystemLanguage.Indonesian,       "id" },
                { SystemLanguage.Italian,          "it" },
                { SystemLanguage.Japanese,         "ja" },
                { SystemLanguage.Korean,           "ko" },
                { SystemLanguage.Latvian,          "lv" },
                { SystemLanguage.Lithuanian,       "lt" },
                { SystemLanguage.Norwegian,        "no" },
                { SystemLanguage.Polish,           "pl" },
                { SystemLanguage.Portuguese,       "pt" },
                { SystemLanguage.Romanian,         "ro" },
                { SystemLanguage.Russian,          "ru" },
                { SystemLanguage.SerboCroatian,    "sr" },
                { SystemLanguage.Slovak,           "sk" },
                { SystemLanguage.Slovenian,        "sl" },
                { SystemLanguage.Spanish,          "es" },
                { SystemLanguage.Swedish,          "sv" },
                { SystemLanguage.Thai,             "th" },
                { SystemLanguage.Turkish,          "tr" },
                { SystemLanguage.Ukrainian,        "uk" },
                { SystemLanguage.Vietnamese,       "vi" },
            };

            return map.TryGetValue(language, out var code) ? code : string.Empty;
        }
    }
}
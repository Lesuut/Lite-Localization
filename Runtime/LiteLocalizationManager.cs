using System;
using System.Linq;
using LiteLocalization.Runtime.Data;

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
            _dataStorage  = new LiteLocalizationDataStorage(
                "Localization", 
                LiteLocalizationSettings.Instance.DefaultLanguage);
            
            _localizationTable = new LocalizationTable(
                LiteLocalizationSettings.Instance.LanguagesTextAsset,
                LiteLocalizationSettings.Instance.Separator);
            
            _localizationTableEditor = new LocalizationTableEditor(
                LiteLocalizationSettings.Instance.LanguagesTextAsset,
                LiteLocalizationSettings.Instance.Separator);
            
            foreach (var languages in LiteLocalizationSettings.Instance.Languages)
            {
                if (!_localizationTableEditor.HasLocale(languages.languageCode))
                {
                    _localizationTableEditor.AddLocale(languages.languageCode);
                }
            }
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
            if (!Instance._localizationTable.HasKey(key))
                Instance._localizationTableEditor.AddKey(key, LiteLocalizationSettings.Instance.SourceLanguage);
            
            return Instance._localizationTable.GetCell(key, Instance._dataStorage.Data.LanguageCode);
        }
    }
}
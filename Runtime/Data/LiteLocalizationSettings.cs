using UnityEngine;

namespace LiteLocalization.Runtime.Data
{
    public class LiteLocalizationSettings : ScriptableObject
    {
        private const string Path = "Assets/Resources/LiteLocalizationSettings.asset";
        
        public string DefaultLanguage = "en";
        public string SourceLanguage = "en";
        public LanguageCodeItem[] Languages = new LanguageCodeItem[]
        {
            new LanguageCodeItem { languageCode = "en", languageFullName = "English"},
        };
        public TextAsset LanguagesTextAsset;
        public string Separator = ";";

        #region Singleton

        private static LiteLocalizationSettings instance;

        public static LiteLocalizationSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LiteLocalizationSettings>("LiteLocalizationSettings");

#if UNITY_EDITOR
                    if (instance == null)
                    {
                        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

                        instance = CreateInstance<LiteLocalizationSettings>();
                        UnityEditor.AssetDatabase.CreateAsset(instance, Path);
                        UnityEditor.AssetDatabase.SaveAssets();
                    }
#endif
                }

                return instance;
            }
        }

        #endregion
    }
}
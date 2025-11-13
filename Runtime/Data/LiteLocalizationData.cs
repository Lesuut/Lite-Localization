using System;

namespace LiteLocalization.Runtime.Data
{
    [Serializable]
    public class LiteLocalizationData
    {
        public string LanguageCode;

        public LiteLocalizationData() { }

        public LiteLocalizationData(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }
}
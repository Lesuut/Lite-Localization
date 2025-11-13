using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace LiteLocalization.Runtime.UI
{
    [RequireComponent(typeof(Dropdown))]
    public class ChangeLanguageDropdown : Dropdown
    {
        protected override void Awake()
        {
            base.Awake();

            options.Clear();

            var languages = LiteLocalizationManager.Instance.GetLanguages();
            var languageNames = languages.Select(l => l.languageFullName).ToList();

            foreach (var name in languageNames)
            {
                options.Add(new OptionData(name));
            }
            
            string currentLanguageName = LiteLocalizationManager.Instance.GetCurrentLanguage().languageFullName;
            int index = languageNames.IndexOf(currentLanguageName);

            if (index >= 0)
                value = index;

            onValueChanged.AddListener(OnLanguageChanged);
        }

        private void OnLanguageChanged(int selectedIndex)
        {
            var selectedLanguage = LiteLocalizationManager.Instance.GetLanguages()[selectedIndex];
            LiteLocalizationManager.Instance.SetLocalization(selectedLanguage.languageCode);

            Debug.Log($"Language changed to: <color=green>{selectedLanguage.languageFullName}</color>");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onValueChanged.RemoveListener(OnLanguageChanged);
        }
    }
}
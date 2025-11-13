using UnityEngine;
using TMPro;
using System.Linq;

namespace LiteLocalization.Runtime.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ChangeLanguageTMPDropdown : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.ClearOptions();

            var languages = LiteLocalizationManager.Instance.GetLanguages();
            var languageNames = languages.Select(l => l.languageFullName).ToList();

            foreach (var name in languageNames)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(name));
            }

            string currentLanguageName = LiteLocalizationManager.Instance.GetCurrentLanguage().languageFullName;
            int index = languageNames.IndexOf(currentLanguageName);

            if (index >= 0)
                _dropdown.value = index;

            _dropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void OnLanguageChanged(int selectedIndex)
        {
            var selectedLanguage = LiteLocalizationManager.Instance.GetLanguages()[selectedIndex];
            LiteLocalizationManager.Instance.SetLocalization(selectedLanguage.languageCode);

            Debug.Log($"Language changed to: <color=green>{selectedLanguage.languageFullName}</color>");
        }

        private void OnDestroy()
        {
            if (_dropdown != null)
                _dropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }
    }
}
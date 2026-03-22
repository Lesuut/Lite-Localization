using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LiteLocalization.Runtime.UI
{
    public class TranslateText : MonoBehaviour
    {
        [SerializeField] private string _key;

        private Text _text;
        private TMP_Text _textTMP;

        private void Awake()
        {
            TryGetComponent(out _text);
            TryGetComponent(out _textTMP);
        }

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(_key)) return;

            if (TryGetComponent(out Text text) && !string.IsNullOrEmpty(text.text))
                _key = text.text;
            else if (TryGetComponent(out TMP_Text tmp) && !string.IsNullOrEmpty(tmp.text))
                _key = tmp.text;
        }
        
        private void Start()
        {
            UpdateText();
            LiteLocalizationManager.OnLanguageChanged += UpdateText;
        }

        private void OnDestroy()
        {
            if (LiteLocalizationManager.Instance != null)
                LiteLocalizationManager.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(_key))
                return;

            var translated = LiteLocalizationManager.Translate(_key);

            if (_text != null)
                _text.text = translated;

            if (_textTMP != null)
                _textTMP.text = translated;
        }
    }
}
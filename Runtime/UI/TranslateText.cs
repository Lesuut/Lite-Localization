using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LiteLocalization.Runtime.UI
{
    public class TranslateText : MonoBehaviour
    {
        [SerializeField] private string _customCashText;
        [SerializeField] private string _sourceLanguage;

        private Text _text;
        private TMP_Text _textTMP;
        private string _originalText;

        private void Start()
        {
            if (TryGetComponent<Text>(out _text))
                _originalText = _text.text;
            else if (TryGetComponent<TMP_Text>(out _textTMP))
                _originalText = _textTMP.text;

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
            var key = string.IsNullOrEmpty(_customCashText) ? _originalText : _customCashText;
            var src = string.IsNullOrEmpty(_sourceLanguage) ? null : _sourceLanguage;

            if (_text != null)
                _text.text = LiteLocalizationManager.Translate(key, src);

            if (_textTMP != null)
                _textTMP.text = LiteLocalizationManager.Translate(key, src);
        }
    }
}
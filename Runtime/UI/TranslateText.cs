using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LiteLocalization.Runtime.UI
{
    public class TranslateText : MonoBehaviour
    {
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
            if (_text != null)
                _text.text = LiteLocalizationManager.Translate(_originalText);

            if (_textTMP != null)
                _textTMP.text = LiteLocalizationManager.Translate(_originalText);
        }
    }
}
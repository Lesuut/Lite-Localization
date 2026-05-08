#if UNITY_EDITOR
using System.Collections.Generic;
using LiteLocalization.Runtime.Data;
using LiteLocalization.Runtime.UI;
using UnityEditor;
using UnityEngine;

namespace LiteLocalization.Runtime.Editor
{
    [CustomEditor(typeof(TranslateText))]
    public class TranslateTextEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_customCashText"));

            SerializedProperty sourceLangProp = serializedObject.FindProperty("_sourceLanguage");

            var settings = LiteLocalizationSettings.Instance;
            if (settings != null && settings.Languages != null && settings.Languages.Length > 0)
            {
                var codes = new List<string>();
                foreach (var lang in settings.Languages)
                {
                    if (!string.IsNullOrEmpty(lang.languageCode))
                        codes.Add(lang.languageCode);
                }

                var displayOptions = new List<string> { $"Default ({settings.SourceLanguage})" };
                displayOptions.AddRange(codes);

                int currentIndex = string.IsNullOrEmpty(sourceLangProp.stringValue)
                    ? 0
                    : codes.IndexOf(sourceLangProp.stringValue) + 1;
                if (currentIndex < 0) currentIndex = 0;

                int newIndex = EditorGUILayout.Popup("Source Language", currentIndex, displayOptions.ToArray());
                sourceLangProp.stringValue = newIndex == 0 ? "" : codes[newIndex - 1];
            }
            else
            {
                EditorGUILayout.PropertyField(sourceLangProp, new GUIContent("Source Language"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

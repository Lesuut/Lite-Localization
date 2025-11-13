#if UNITY_EDITOR
using LiteLocalization.Runtime.Data;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LiteLocalization.Runtime.Editor
{
    public class LiteLocalizationSettingsProvider : SettingsProvider
    {
        private SerializedObject serializedSettings;

        public LiteLocalizationSettingsProvider(string path, SettingsScope scopes)
            : base(path, scopes)
        {
        }

        public override void OnGUI(string searchContext)
        {
            if (serializedSettings == null)
                serializedSettings = new SerializedObject(LiteLocalizationSettings.Instance);

            serializedSettings.Update();
            
            # region DefaultLanguageDropDown
            EditorGUILayout.Space(10);

            SerializedProperty prop = serializedSettings.GetIterator();
            SerializedProperty defaultLangProp = serializedSettings.FindProperty("DefaultLanguage");
            SerializedProperty languagesProp = serializedSettings.FindProperty("Languages");
            
            if (languagesProp != null && languagesProp.isArray)
            {
                List<string> codes = new List<string>();
                for (int i = 0; i < languagesProp.arraySize; i++)
                {
                    var el = languagesProp.GetArrayElementAtIndex(i);
                    var codeProp = el.FindPropertyRelative("languageCode");
                    if (!string.IsNullOrEmpty(codeProp.stringValue))
                        codes.Add(codeProp.stringValue);
                }

                if (codes.Count == 0)
                {
                    EditorGUILayout.HelpBox("Add at least one language to select a default.", MessageType.Info);
                }
                else
                {
                    int currentIndex = Mathf.Max(0, codes.IndexOf(defaultLangProp.stringValue));
                    int newIndex = EditorGUILayout.Popup("Default Language", currentIndex, codes.ToArray());
                    defaultLangProp.stringValue = codes[newIndex];
                }
            }
            #endregion
            
            # region SourceLanguageDropDown
            SerializedProperty sourceDefaultLangProp = serializedSettings.FindProperty("SourceLanguage");
            SerializedProperty sourceLanguagesProp = serializedSettings.FindProperty("Languages");

            if (sourceLanguagesProp != null && sourceLanguagesProp.isArray)
            {
                List<string> languageCodes = new List<string>();
                for (int i = 0; i < sourceLanguagesProp.arraySize; i++)
                {
                    var languageElement = sourceLanguagesProp.GetArrayElementAtIndex(i);
                    var languageCodeProp = languageElement.FindPropertyRelative("languageCode");
                    if (!string.IsNullOrEmpty(languageCodeProp.stringValue))
                        languageCodes.Add(languageCodeProp.stringValue);
                }
    
                if (languageCodes.Count == 0)
                {
                    EditorGUILayout.HelpBox("Add at least one language to select a default.", MessageType.Info);
                }
                else
                {
                    int currentIndex = Mathf.Max(0, languageCodes.IndexOf(sourceDefaultLangProp.stringValue));
                    int newIndex = EditorGUILayout.Popup("Source Language", currentIndex, languageCodes.ToArray());
                    sourceDefaultLangProp.stringValue = languageCodes[newIndex];
                }
            }
            #endregion
            
            #region LanguagesTextAsset
            SerializedProperty languagesTextAsset = serializedSettings.FindProperty("LanguagesTextAsset");

            TextAsset previousAsset = languagesTextAsset.objectReferenceValue as TextAsset;

            EditorGUILayout.PropertyField(
                languagesTextAsset,
                new GUIContent("Languages Text Asset", "Select a CSV or TextAsset containing your language definitions.")
            );

            TextAsset newAsset = languagesTextAsset.objectReferenceValue as TextAsset;
            
            if (newAsset != null && newAsset != previousAsset)
            {
                string path = AssetDatabase.GetAssetPath(newAsset);
                string text = System.IO.File.ReadAllText(path);
                
                if (string.IsNullOrWhiteSpace(text))
                {
                    const string baseContent = "Keys;en";

                    System.IO.File.WriteAllText(path, baseContent);
                    AssetDatabase.Refresh();
                    Debug.Log($"LanguagesTextAsset initialized with base content: {baseContent}");
                }
            }

            bool isEmpty = languagesTextAsset.objectReferenceValue == null;

            Color defaultColor = GUI.backgroundColor;

            if (isEmpty)
                GUI.backgroundColor = new Color(1f, 0.18f, 0.2f);

            GUI.backgroundColor = defaultColor;

            if (isEmpty)
            {
                EditorGUILayout.HelpBox(
                    "You need to assign a TextAsset containing language data (e.g., a CSV file).\nBase file content: 'Keys;en'",
                    MessageType.Warning
                );
            }
            #endregion
            
            #region Separator
            SerializedProperty separatorProp = serializedSettings.FindProperty("Separator");
            EditorGUILayout.PropertyField(separatorProp, new GUIContent("Separator", "Char for split CSV"));
            EditorGUILayout.Space(8);
            #endregion
            
            #region Languages
            EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var codeSet = new HashSet<string>();
            var nameSet = new HashSet<string>();

            GUIStyle placeholderStyle = new GUIStyle(EditorStyles.label);
            placeholderStyle.normal.textColor = new Color(0.65f, 0.65f, 0.65f);

            for (int i = 0; i < languagesProp.arraySize; i++)
            {
                SerializedProperty element = languagesProp.GetArrayElementAtIndex(i);
                var codeProp = element.FindPropertyRelative("languageCode");
                var nameProp = element.FindPropertyRelative("languageFullName");

                bool duplicate = !string.IsNullOrEmpty(codeProp.stringValue) && codeSet.Contains(codeProp.stringValue);

                if (!string.IsNullOrEmpty(nameProp.stringValue) && nameSet.Contains(nameProp.stringValue))
                    duplicate = true;

                if (!string.IsNullOrEmpty(codeProp.stringValue))
                    codeSet.Add(codeProp.stringValue);
                if (!string.IsNullOrEmpty(nameProp.stringValue))
                    nameSet.Add(nameProp.stringValue);

                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 2);
                if (duplicate)
                    EditorGUI.DrawRect(rect, new Color(1f, 0.07f, 0.13f, 0.8f));

                float buttonWidth = 20f;
                float fieldWidth = (rect.width - buttonWidth - 4f) / 2f;

                Rect codeRect = new Rect(rect.x, rect.y, fieldWidth, rect.height);
                Rect nameRect = new Rect(rect.x + fieldWidth + 2f, rect.y, fieldWidth, rect.height);
                Rect deleteRect = new Rect(rect.x + fieldWidth * 2 + 4f, rect.y, buttonWidth, rect.height);

                EditorGUI.PropertyField(codeRect, codeProp, GUIContent.none);
                if (string.IsNullOrEmpty(codeProp.stringValue))
                    EditorGUI.LabelField(codeRect, " code (en)", placeholderStyle);
                
                EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
                if (string.IsNullOrEmpty(nameProp.stringValue))
                    EditorGUI.LabelField(nameRect, " Language name (English)", placeholderStyle);
                
                if (GUI.Button(deleteRect, "-"))
                    languagesProp.DeleteArrayElementAtIndex(i);
            }

            if (GUILayout.Button("+ Add Language"))
            {
                languagesProp.InsertArrayElementAtIndex(languagesProp.arraySize);
            }
            #endregion
            
            EditorGUI.indentLevel--;
            
            serializedSettings.ApplyModifiedProperties();
        }

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new LiteLocalizationSettingsProvider("Project/Game/Lite Localization", SettingsScope.Project)
            {
                label = "Lite Localization"
            };
        }
    }
}
#endif

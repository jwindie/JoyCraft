// Developed by Tom Kail at Inkle
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT

// Must be placed within a folder named "Editor"

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
///     Extends how ScriptableObject object references are displayed in the inspector
///     Shows you all values under the object reference
///     Also provides a button to create a new ScriptableObject if property is null.
/// </summary>
[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ExtendedScriptableObjectDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var totalHeight = EditorGUIUtility.singleLineHeight;
        if (property.objectReferenceValue == null || !(property.objectReferenceValue is ScriptableObject) || !AreAnySubPropertiesVisible(property)) return totalHeight;
        if (property.isExpanded) {
            var data = property.objectReferenceValue as ScriptableObject;
            if (data == null) return EditorGUIUtility.singleLineHeight;
            using var serializedObject = new SerializedObject(data);
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
                do {
                    if (prop.name == "m_Script") continue;
                    var subProp = serializedObject.FindProperty(prop.name);
                    var height = EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
                    totalHeight += height;
                } while (prop.NextVisible(false));

            // Add a tiny bit of height if open for the background
            totalHeight += EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }

    const int buttonWidth = 66;

    static readonly List<string> ignoreClassFullNames = new() {"TMPro.TMP_FontAsset"};

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        var type = GetFieldType();

        if (type == null || ignoreClassFullNames.Contains(type.FullName)) {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
            return;
        }

        ScriptableObject propertySO = null;
        if (!property.hasMultipleDifferentValues && property.serializedObject.targetObject != null && property.serializedObject.targetObject is ScriptableObject) propertySO = (ScriptableObject) property.serializedObject.targetObject;

        var propertyRect = Rect.zero;
        var guiContent = new GUIContent(property.displayName);
        var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        if (property.objectReferenceValue != null && AreAnySubPropertiesVisible(property)) {
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true);
        } else {
            // So yeah having a foldout look like a label is a weird hack 
            // but both code paths seem to need to be a foldout or 
            // the object field control goes weird when the codepath changes.
            // I guess because foldout is an interactable control of its own and throws off the controlID?
            foldoutRect.x += 12;
            EditorGUI.Foldout(foldoutRect, property.isExpanded, guiContent, true, EditorStyles.label);
        }

        var indentedPosition = EditorGUI.IndentedRect(position);
        var indentOffset = indentedPosition.x - position.x;
        propertyRect = new Rect(position.x + (EditorGUIUtility.labelWidth - indentOffset), position.y, position.width - (EditorGUIUtility.labelWidth - indentOffset), EditorGUIUtility.singleLineHeight);

        if (propertySO != null || property.objectReferenceValue == null) propertyRect.width -= buttonWidth;

        EditorGUI.ObjectField(propertyRect, property, type, GUIContent.none);
        if (GUI.changed) property.serializedObject.ApplyModifiedProperties();

        var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null) {
            var data = (ScriptableObject) property.objectReferenceValue;

            if (property.isExpanded) {
                // Draw a background that shows us clearly which fields are part of the ScriptableObject
                GUI.Box(new Rect(0, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing - 1, Screen.width, position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing), "");

                EditorGUI.indentLevel++;
                using var serializedObject = new SerializedObject(data);

                // Iterate over all the values and draw them
                var prop = serializedObject.GetIterator();
                var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (prop.NextVisible(true))
                    do {
                        // Don't bother drawing the class file
                        if (prop.name == "m_Script") continue;
                        var height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width - buttonWidth, height), prop, true);
                        y += height + EditorGUIUtility.standardVerticalSpacing;
                    } while (prop.NextVisible(false));

                if (GUI.changed)
                    serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
        } else {
            if (GUI.Button(buttonRect, "Create")) {
                var selectedAssetPath = "Assets";
                if (property.serializedObject.targetObject is MonoBehaviour) {
                    var ms = MonoScript.FromMonoBehaviour((MonoBehaviour) property.serializedObject.targetObject);
                    selectedAssetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
                }

                property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
            }
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    // Allows calling this drawer from GUILayout rather than as a property drawer, which can be useful for custom inspectors
    public static T DrawScriptableObjectField<T>(GUIContent label, T objectReferenceValue, ref bool isExpanded) where T : ScriptableObject {
        var position = EditorGUILayout.BeginVertical();

        var guiContent = label;
        var foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        if (objectReferenceValue != null) {
            isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true);
        } else {
            // So yeah having a foldout look like a label is a weird hack 
            // but both code paths seem to need to be a foldout or 
            // the object field control goes weird when the codepath changes.
            // I guess because foldout is an interactable control of its own and throws off the controlID?
            foldoutRect.x += 12;
            EditorGUI.Foldout(foldoutRect, isExpanded, guiContent, true, EditorStyles.label);
        }

        EditorGUILayout.BeginHorizontal();
        objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" "), objectReferenceValue, typeof(T), false) as T;

        if (objectReferenceValue != null) {
            EditorGUILayout.EndHorizontal();
            if (isExpanded) DrawScriptableObjectChildFields(objectReferenceValue);
        } else {
            if (GUILayout.Button("Create", GUILayout.Width(buttonWidth))) {
                var selectedAssetPath = "Assets";
                var newAsset = CreateAssetWithSavePrompt(typeof(T), selectedAssetPath);
                if (newAsset != null) objectReferenceValue = (T) newAsset;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        return objectReferenceValue;
    }
    
    static void DrawScriptableObjectChildFields<T>(T objectReferenceValue) where T : ScriptableObject {
        // Draw a background that shows us clearly which fields are part of the ScriptableObject
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical(GUI.skin.box);

        using var serializedObject = new SerializedObject(objectReferenceValue);
        // Iterate over all the values and draw them
        var prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
            do {
                // Don't bother drawing the class file
                if (prop.name == "m_Script") continue;
                EditorGUILayout.PropertyField(prop, true);
            } while (prop.NextVisible(false));

        if (GUI.changed)
            serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;
    }
    
    // Creates a new ScriptableObject via the default Save File panel
    static ScriptableObject CreateAssetWithSavePrompt(Type type, string path) {
        path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
        if (path == "") return null;
        var asset = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        EditorGUIUtility.PingObject(asset);
        return asset;
    }

    Type GetFieldType() {
        if (fieldInfo == null) return null;
        var type = fieldInfo.FieldType;
        if (type.IsArray) type = type.GetElementType();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
        return type;
    }

    static bool AreAnySubPropertiesVisible(SerializedProperty property) {
        var data = (ScriptableObject) property.objectReferenceValue;

        if (null != data) {
            using var serializedObject = new SerializedObject(data);
            var prop = serializedObject.GetIterator();

            // Check for any visible property excluding m_script
            while (prop.NextVisible(true)) {
                if (prop.name == "m_Script")
                    continue;

                return true;
            }
        }

        return false;
    }
}
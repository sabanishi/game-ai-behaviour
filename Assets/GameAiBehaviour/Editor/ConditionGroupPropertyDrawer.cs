using System;
using UnityEditor;
using UnityEngine;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// ConditionGroupのエディタ拡張
    /// </summary>
    [CustomPropertyDrawer(typeof(ConditionGroup))]
    public class ConditionGroupPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// GUI拡張
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var conditions = property.FindPropertyRelative("conditions");
            if (conditions.arraySize == 0) {
                var rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(rect, "Create Condition Test")) {
                    var parentAsset = property.serializedObject.targetObject;
                    if (parentAsset != null) {
                        var condition = ScriptableObject.CreateInstance<BooleanCondition>();
                        AssetDatabase.AddObjectToAsset(condition, parentAsset);
                        Undo.RegisterCreatedObjectUndo(condition, "Created condition.");
                        conditions.InsertArrayElementAtIndex(0);
                        conditions.GetArrayElementAtIndex(0).objectReferenceValue = condition;
                        EditorUtility.SetDirty(parentAsset);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            else {
                EditorGUI.PropertyField(position, conditions, label, true);
            }
        }

        /// <summary>
        /// GUIの高さ計算
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var conditions = property.FindPropertyRelative("conditions");
            if (conditions.arraySize == 0) {
                return EditorGUIUtility.singleLineHeight;
            }
            else {
                return EditorGUI.GetPropertyHeight(conditions, label, true);
            }
        }
    }
}
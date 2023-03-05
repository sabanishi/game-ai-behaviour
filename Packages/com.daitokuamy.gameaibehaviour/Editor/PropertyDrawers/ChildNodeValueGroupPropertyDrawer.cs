using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// ChildNodeValueGroupのエディタ拡張
    /// </summary>
    [CustomPropertyDrawer(typeof(ChildNodeValueGroup<>), true)]
    public class ChildNodeValueGroupPropertyDrawer : PropertyDrawer {
        private Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

        /// <summary>
        /// GUI拡張
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var childrenProp = property.serializedObject.FindProperty("children");
            var valuesProp = property.FindPropertyRelative("_values");

            if (childrenProp == null) {
                EditorGUI.HelpBox(position, "Not found children.", MessageType.Error);
                return;
            }

            // 子ノードの情報に合わせて、配列の要素数を変更
            valuesProp.arraySize = childrenProp.arraySize;

            var padding = EditorGUIUtility.standardVerticalSpacing;
            var rect = position;

            // フォルダ描画
            rect.y += padding;
            rect.height = EditorGUIUtility.singleLineHeight;
            if (Foldout(rect, property, label)) {
                rect.y += rect.height + padding;
                rect.x += 10;

                // 中身を描画
                for (var i = 0; i < valuesProp.arraySize; i++) {
                    var element = valuesProp.GetArrayElementAtIndex(i);
                    var nodeElement = childrenProp.GetArrayElementAtIndex(i);
                    var height = EditorGUI.GetPropertyHeight(element, true);
                    var nodeName = "Empty";
                    if (nodeElement.objectReferenceValue is Node node) {
                        nodeName = BehaviourTreeEditorUtility.GetNodeDisplayTitle(node);
                    }

                    rect.y += padding;
                    rect.height = height;
                    EditorGUI.PropertyField(rect, element, new GUIContent($"[{i}]{nodeName}"), true);
                    rect.y += height + padding;
                }

                rect.x -= 10;
            }
        }

        /// <summary>
        /// GUIの高さ計算
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var childrenProp = property.serializedObject.FindProperty("children");
            if (childrenProp == null) {
                return EditorGUIUtility.singleLineHeight * 1.5f;
            }
            
            var totalHeight = 0.0f;
            var padding = EditorGUIUtility.standardVerticalSpacing;

            var foldout = GetFoldout(property);
            totalHeight = padding * 2 + EditorGUIUtility.singleLineHeight;

            if (foldout) {
                var valuesProp = property.FindPropertyRelative("_values");

                // 要素数分の高さを返却
                for (var i = 0; i < valuesProp.arraySize; i++) {
                    var element = valuesProp.GetArrayElementAtIndex(i);
                    totalHeight += padding * 2 + EditorGUI.GetPropertyHeight(element);
                }
            }

            return totalHeight;
        }

        /// <summary>
        /// Foldout状態の取得
        /// </summary>
        private bool GetFoldout(SerializedProperty property) {
            if (!_foldouts.TryGetValue(property.propertyPath, out var foldout)) {
                foldout = false;
                _foldouts[property.propertyPath] = false;
            }

            return foldout;
        }

        /// <summary>
        /// Foldout描画
        /// </summary>
        private bool Foldout(Rect rect, SerializedProperty property, GUIContent label) {
            var result = EditorGUI.Foldout(rect, GetFoldout(property), label);
            _foldouts[property.propertyPath] = result;
            return result;
        }
    }
}
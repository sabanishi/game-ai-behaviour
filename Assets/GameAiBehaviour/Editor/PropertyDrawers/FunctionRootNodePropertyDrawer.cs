using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// FunctionRootNode型用のPropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(FunctionRootNode))]
    public class FunctionRootNodePropertyDrawer : PropertyDrawer {
        /// <summary>
        /// GUI描画
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var rect = position;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            property.serializedObject.Update();
            property.objectReferenceValue = BehaviourTreeEditorGUI.NodeField(rect, label,
                property.objectReferenceValue as FunctionRootNode);
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// GUI高さ取得
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// Property型用のPropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(Property))]
    public class PropertyPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// GUI描画
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var height = EditorGUIUtility.singleLineHeight;
            var offset = height + EditorGUIUtility.standardVerticalSpacing * 2;
            var rect = position;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = height;
            
            property.serializedObject.Update();
            
            // タイプ切り替え
            var type = property.FindPropertyRelative("propertyType");
            using (var scope = new EditorGUI.ChangeCheckScope()) {
                type.enumValueIndex = EditorGUI.Popup(rect, GUIContent.none, type.enumValueIndex,
                    type.enumDisplayNames.Select(x => new GUIContent(x)).ToArray());
            }
            
            // プロパティ名/タイプ毎の値の描画
            rect.y += offset;
            var left = rect;
            var right = rect;
            left.xMax = left.xMin + position.width * 0.5f;
            right.xMin += left.width;
            left.xMax -= 2;

            var name = property.FindPropertyRelative("propertyName");
            var val = default(SerializedProperty);
            switch ((Property.Type)type.enumValueIndex) {
                case Property.Type.Integer:
                    val = property.FindPropertyRelative("integerValue");
                    break;
                case Property.Type.Float:
                    val = property.FindPropertyRelative("floatValue");
                    break;
                case Property.Type.String:
                    val = property.FindPropertyRelative("stringValue");
                    break;
                case Property.Type.Boolean:
                    val = property.FindPropertyRelative("booleanValue");
                    break;
            }
            
            EditorGUI.PropertyField(left, name, GUIContent.none);
            EditorGUI.PropertyField(right, val, GUIContent.none);
            
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// GUI高さ取得
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameAiBehaviour {
    /// <summary>
    /// 値比較用の条件基底
    /// </summary>
    public abstract class ValueCondition<T, TValueObject> : Condition
        where TValueObject : ValueObject<T> {
        [Tooltip("左辺値")]
        public TValueObject leftValue;
        [Tooltip("右辺値")]
        public TValueObject rightValue;
        
#if UNITY_EDITOR
        /// <summary>
        /// インスペクタ描画
        /// </summary>
        public override void OnInspectorGUI(Rect position, SerializedObject serializedObject) {
            var leftProp = serializedObject.FindProperty("leftValue");
            var rightProp = serializedObject.FindProperty("rightValue");
            var operatorProp = serializedObject.FindProperty("operatorType");
            var rect = position;
            
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUI.GetPropertyHeight(leftProp, true) + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, leftProp, true);
            
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            operatorProp.enumValueIndex = EditorGUI.Popup(rect, operatorProp.displayName, operatorProp.enumValueIndex, operatorProp.enumDisplayNames);
            
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUI.GetPropertyHeight(rightProp, true) + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, rightProp, true);
        }

        /// <summary>
        /// インスペクタ描画時の高さ取得
        /// </summary>
        public override float GetInspectorGUIHeight(SerializedObject serializedObject) {
            var leftProp = serializedObject.FindProperty("leftValue");
            var rightProp = serializedObject.FindProperty("rightValue");
            var height = 0.0f;
            var padding = EditorGUIUtility.standardVerticalSpacing * 2;
            height += EditorGUI.GetPropertyHeight(leftProp, true) + padding;
            height += EditorGUIUtility.singleLineHeight + padding;
            height += EditorGUI.GetPropertyHeight(rightProp, true) + padding;
            return height;
        }
#endif

        /// <summary>
        /// 判定
        /// </summary>
        public sealed override bool Check(Blackboard blackboard) {
            return CheckInternal(GetLeftValue(blackboard), GetRightValue(blackboard));
        }

        /// <summary>
        /// 値の判定
        /// </summary>
        protected abstract bool CheckInternal(T left, T right);

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected abstract T GetPropertyValue(Blackboard blackboard, string propertyName, T defaultValue);

        /// <summary>
        /// 左辺値の取得
        /// </summary>
        private T GetLeftValue(Blackboard blackboard) {
            return GetValue(blackboard, leftValue);
        }
        
        /// <summary>
        /// 右辺値の取得
        /// </summary>
        private T GetRightValue(Blackboard blackboard) {
            return GetValue(blackboard, rightValue);
        }
        
        /// <summary>
        /// 値の取得
        /// </summary>
        private T GetValue(Blackboard blackboard, TValueObject valueObject) {
            if (string.IsNullOrEmpty(valueObject.propertyName)) {
                return valueObject.constValue;
            }
            return GetPropertyValue(blackboard, valueObject.propertyName, valueObject.constValue);
        }
    }
}
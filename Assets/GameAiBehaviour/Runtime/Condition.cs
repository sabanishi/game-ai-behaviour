using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameAiBehaviour {
    /// <summary>
    /// 条件基底
    /// </summary>
    public abstract class Condition : ScriptableObject {
        // 条件タイトル表示名
        public virtual string ConditionTitle => $"{GetType().Name}";
        
        /// <summary>
        /// 判定
        /// </summary>
        public abstract bool Check(Blackboard blackboard);
        
#if UNITY_EDITOR
        /// <summary>
        /// インスペクタ描画
        /// </summary>
        public abstract void OnInspectorGUI(Rect position, SerializedObject serializedObject);
        
        /// <summary>
        /// インスペクタ描画時の高さ取得
        /// </summary>
        public abstract float GetInspectorGUIHeight(SerializedObject serializedObject);
#endif

        /// <summary>
        /// 生成時処理
        /// </summary>
        private void Awake() {
            OnValidate();
        }

        /// <summary>
        /// 表示時処理
        /// </summary>
        private void OnValidate() {
            hideFlags |= HideFlags.HideInHierarchy;
        }
    }
}
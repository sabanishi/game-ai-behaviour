#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeのエディタGUI補助クラス
    /// </summary>
    public static class BehaviourTreeEditorGUI {
        // 現在描画中のツリー
        public static BehaviourTree CurrentTree { get; set; }

        /// <summary>
        /// プロパティ名のGUI描画
        /// </summary>
        public static string PropertyNameField(Rect rect, string propertyName, Property.Type[] typeFilters) {
            // Treeがない場合はただのTextField
            if (CurrentTree == null) {
                return EditorGUI.TextField(rect, propertyName);
            }
            
            // Treeがある場合は、BlackboardのPropertyを使ったPopup
            var propertyNames = CurrentTree.properties
                .Where(x => typeFilters?.Contains(x.propertyType) ?? true)
                .Select(x => x.propertyName)
                .ToArray();
            if (propertyNames.Length <= 0) {
                propertyNames = new[] { "Empty" };
            }

            var index = Mathf.Max(0, propertyNames.ToList().IndexOf(propertyName));
            index = EditorGUI.Popup(rect, index, propertyNames);
            return propertyNames[index];
        }
    }
}
#endif
using UnityEngine;
using UnityEditor;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// BehaviourTree用のエディタウィンドウ
    /// </summary>
    [CustomEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : UnityEditor.Editor {
        /// <summary>
        /// インスペクタ拡張
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Window")) {
                BehaviourTreeEditorWindow.Open(target as BehaviourTree);
            }
        }
    }
}
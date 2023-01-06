using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// Blackboard表示用のビュー
    /// </summary>
    public class BlackboardView : VisualElement {
        public new class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> {
        }

        // GUIContainer
        private IMGUIContainer _container;
        // スクロール位置
        private Vector2 _scroll;
        // Treeを制御しているクラス
        private IBehaviourTreeController _controller;
        // TreeのProperties用プロパティ
        private SerializedProperty _property;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BlackboardView() {
            _container = new IMGUIContainer(OnGUI);
            Add(_container);
        }

        /// <summary>
        /// Treeを制御しているControllerの設定
        /// </summary>
        public void SetController(IBehaviourTreeController controller) {
            _controller = controller;
        }

        /// <summary>
        /// Blackboardに乗せるProperties用のProperty設定
        /// </summary>
        public void SetPropertiesProperty(SerializedProperty property) {
            _property = property;
        }

        /// <summary>
        /// GUI描画
        /// </summary>
        private void OnGUI() {
            if (_controller != null) {
                // Blackboardの中身を表示
                void DrawProperty<T>(string[] keys, Func<string, T> onGui, Action<string, T> onSet) {
                    foreach (var key in keys) {
                        using (var scope = new EditorGUI.ChangeCheckScope()) {
                            var result = onGui.Invoke(key);
                            if (scope.changed) {
                                onSet.Invoke(key, result);
                            }
                        }
                    }
                }

                var blackboard = _controller.Blackboard;
                DrawProperty(blackboard.IntegerPropertyNames,
                    key => EditorGUILayout.IntField(key, blackboard.GetInteger(key)),
                    (key, result) => blackboard.SetInteger(key, result));
                DrawProperty(blackboard.FloatPropertyNames,
                    key => EditorGUILayout.FloatField(key, blackboard.GetFloat(key)),
                    (key, result) => blackboard.SetFloat(key, result));
                DrawProperty(blackboard.StringPropertyNames,
                    key => EditorGUILayout.TextField(key, blackboard.GetString(key)),
                    (key, result) => blackboard.SetString(key, result));
                DrawProperty(blackboard.BooleanPropertyNames,
                    key => EditorGUILayout.Toggle(key, blackboard.GetBoolean(key)),
                    (key, result) => blackboard.SetBoolean(key, result));
            }
            else {
                if (_property != null) {
                    _property.serializedObject.Update();

                    using (var scope = new EditorGUILayout.ScrollViewScope(_scroll)) {
                        EditorGUILayout.PropertyField(_property);
                        _scroll = scope.scrollPosition;
                    }

                    _property.serializedObject.ApplyModifiedProperties();
                }
                else {
                    _scroll = Vector2.zero;
                }
            }
        }
    }
}
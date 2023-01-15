using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// インスペクタ表示用のビュー
    /// </summary>
    public class InspectorView : VisualElement {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {
        }

        // Inspector描画用エディタ
        private UnityEditor.Editor _editor;
        // スクロール位置
        private Vector2 _scroll;
        // 描画対象のBehaviourTree
        private BehaviourTree _behaviourTree;

        // 値変更時イベント
        public System.Action<Object[]> OnChangedValue;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectorView() {
        }

        /// <summary>
        /// 描画対象となるBehaviourTreeの設定
        /// </summary>
        public void SetBehaviourTree(BehaviourTree behaviourTree) {
            _behaviourTree = behaviourTree;
            ResetSelection();
        }

        /// <summary>
        /// 選択対象の更新
        /// </summary>
        public void UpdateSelection(params Object[] targets) {
            // ターゲットの型が全部同じでない場合、何もしない
            if (targets.Length <= 0 || targets.Select(x => x.GetType()).Distinct().Count() != 1) {
                return;
            }

            ResetSelection();

            _editor = UnityEditor.Editor.CreateEditor(targets);

            var container = new IMGUIContainer(() => {
                using (var scope = new EditorGUI.ChangeCheckScope()) {
                    if (_editor.targets.Count(x => x == null) > 0) {
                        Clear();
                        return;
                    }

                    BehaviourTreeEditorGUI.CurrentTree = _behaviourTree;

                    using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scroll)) {
                        _editor.OnInspectorGUI();
                        _scroll = scrollScope.scrollPosition;
                    }

                    if (scope.changed) {
                        OnChangedValue?.Invoke(_editor.targets);
                    }

                    BehaviourTreeEditorGUI.CurrentTree = null;
                }
            });
            Add(container);
        }

        /// <summary>
        /// 選択している物のリセット
        /// </summary>
        public void ResetSelection() {
            Clear();

            if (_editor != null) {
                Object.DestroyImmediate(_editor);
                _editor = null;
            }
        }
    }
}
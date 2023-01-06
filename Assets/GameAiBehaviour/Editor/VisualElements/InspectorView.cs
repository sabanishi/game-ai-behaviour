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

        // 値変更時イベント
        public System.Action<Object[]> OnChangedValue;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectorView() {
        }

        /// <summary>
        /// 選択対象の更新
        /// </summary>
        public void UpdateSelection(params Object[] targets) {
            // ターゲットの型が全部同じでない場合、何もしない
            if (targets.Length <= 0 || targets.Select(x => x.GetType()).Distinct().Count() != 1) {
                return;
            }

            void Clear() {
                this.Clear();

                if (_editor != null) {
                    Object.DestroyImmediate(_editor);
                    _editor = null;
                }
            }

            Clear();

            _editor = UnityEditor.Editor.CreateEditor(targets);

            var container = new IMGUIContainer(() => {
                using (var scope = new EditorGUI.ChangeCheckScope()) {
                    if (_editor.targets.Count(x => x == null) > 0) {
                        Clear();
                        return;
                    }

                    using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scroll)) {
                        _editor.OnInspectorGUI();
                        _scroll = scrollScope.scrollPosition;
                    }
                    if (scope.changed) {
                        OnChangedValue?.Invoke(_editor.targets);
                    }
                }
            });
            Add(container);
        }
    }
}
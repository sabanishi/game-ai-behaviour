using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// エディタ用のノードエッジ
    /// </summary>
    public class NodeEdge : Edge {
        // 実行したか
        private bool _ran;

        // 実行状態
        public bool IsRan {
            get => _ran;
            set {
                if (value == _ran) {
                    return;
                }

                _ran = value;
                UpdateEdgeControl();
            }
        }

        /// <summary>
        /// 選択時処理
        /// </summary>
        public override void OnSelected() {
            base.OnSelected();
            UpdateEdgeControlColors();
        }

        /// <summary>
        /// 非選択時処理
        /// </summary>
        public override void OnUnselected() {
            base.OnUnselected();
            UpdateEdgeControlColors();
        }

        /// <summary>
        /// EdgeControl更新時
        /// </summary>
        public override bool UpdateEdgeControl() {
            if (!base.UpdateEdgeControl()) {
                return false;
            }

            UpdateEdgeControlColors();

            return true;
        }

        /// <summary>
        /// カスタムスタイル適用時
        /// </summary>
        protected override void OnCustomStyleResolved(ICustomStyle styles) {
            base.OnCustomStyleResolved(styles);
            UpdateEdgeControlColors();
        }

        /// <summary>
        /// エッジコントロールの色更新
        /// </summary>
        private void UpdateEdgeControlColors() {
            if (IsRan) {
                edgeControl.inputColor = new Color(1, 1, 0, 1);
                edgeControl.outputColor = new Color(1, 1, 0, 1);
            }
        }
    }
}
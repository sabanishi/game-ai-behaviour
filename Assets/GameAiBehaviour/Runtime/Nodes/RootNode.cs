using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 起点となるノード
    /// </summary>
    public sealed class RootNode : Node {
        [HideInInspector, Tooltip("接続先ノード")]
        public Node child;

        private class Logic : Logic<RootNode> {
            private bool _invoked;
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, RootNode node) : base(controller, node) {
            }

            /// <summary>
            /// 開始時処理
            /// </summary>
            protected override void OnStart() {
                _invoked = false;
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate() {
                if (_invoked) {
                    return State;
                }
                
                var child = Node.child;
                if (child != null) {
                    UpdateNode(child);
                }

                // 実行済フラグを立てる
                _invoked = true;

                return State;
            }

            /// <summary>
            /// 子要素の更新結果通知
            /// </summary>
            protected override State OnUpdatedChild(ILogic childNodeLogic) {
                // 常に同じStateとする
                return childNodeLogic.State;
            }
        }

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
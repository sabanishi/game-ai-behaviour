using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 起点となるノード
    /// </summary>
    public sealed class RootNode : Node {
        private class Logic : Logic<RootNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, RootNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                var child = Node.child;
                if (child != null) {
                    yield return ExecuteNodeRoutine(child, SetState);
                }
            }
        }

        [HideInInspector, Tooltip("接続先ノード")]
        public Node child;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
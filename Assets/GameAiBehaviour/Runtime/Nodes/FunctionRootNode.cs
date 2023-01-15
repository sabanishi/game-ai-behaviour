using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// Functionの起点となるノード
    /// </summary>
    public sealed class FunctionRootNode : Node {
        private class Logic : Logic<FunctionRootNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, FunctionRootNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                if (Node.child == null) {
                    SetState(State.Failure);
                    yield break;
                }
                
                yield return ExecuteNodeRoutine(Node.child, SetState);
            }
        }

        [HideInInspector, Tooltip("接続先ノード")]
        public Node child;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
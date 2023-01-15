using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 別のBehaviourTreeのRootを実行するためのノード
    /// </summary>
    public sealed class SubTreeNode : LinkNode {
        private class Logic : Logic<SubTreeNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, SubTreeNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 接続先がない場合は失敗
                if (Node.connectTree == null) {
                    SetState(State.Failure);
                    yield break;
                }
                
                // 接続先ノードの実行
                yield return ExecuteNodeRoutine(Node.connectTree.rootNode, SetState);
            }
        }

        [Tooltip("接続先のTree")]
        public BehaviourTree connectTree;

        public override string Description => connectTree == null ? "Empty" : $"to {connectTree.name}";

        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
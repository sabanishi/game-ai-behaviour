using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// ランタイムハンドリング可能なLinkNode
    /// </summary>
    public abstract class HandleableLinkNode : LinkNode {
        private class Logic : Logic<HandleableLinkNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, HandleableLinkNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                var handler = Controller.GetLinkNodeHandler(Node);

                // 何もなければ成功して終わる
                if (handler == null) {
                    SetState(State.Success);
                    yield break;
                }

                var connectNode = handler.GetConnectNode(Node); 
                // 接続先がない場合は失敗
                if (connectNode == null) {
                    SetState(State.Failure);
                    yield break;
                }

                // 接続先ノードの実行
                yield return ExecuteNodeRoutine(connectNode, SetState);
            }
        }

        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
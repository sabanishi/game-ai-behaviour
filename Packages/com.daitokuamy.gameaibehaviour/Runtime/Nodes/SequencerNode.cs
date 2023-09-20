using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// シーケンスノード
    /// </summary>
    public sealed class SequencerNode : CompositeNode {
        private class Logic : Logic<SequencerNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, SequencerNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                if (Node.children.Length <= 0) {
                    SetState(Node.FailureState);
                    yield break;
                }

                // 順番に実行
                for (var i = 0; i < Node.children.Length; i++) {
                    var node = Node.children[i];
                    yield return ExecuteNodeRoutine(node, SetState);

                    // 成功していたら待機
                    if (State == State.Success) {
                        // 最後だったら終わる
                        if (i >= Node.children.Length - 1) {
                            break;
                        }

                        yield return this;
                    }
                    // 失敗していた場合、そのまま失敗として終了
                    else if (State == State.Failure) {
                        SetState(Node.FailureState);
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
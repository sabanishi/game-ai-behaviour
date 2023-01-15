using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public sealed class SelectorNode : CompositeNode {
        private class Logic : Logic<SelectorNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, SelectorNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 順番に実行トライ
                for (var i = 0; i < Node.children.Length; i++) {
                    var node = Node.children[i];
                    yield return ExecuteNodeRoutine(node, SetState);

                    // 成功していた場合、完了とする
                    if (State == State.Success) {
                        yield break;
                    }
                }

                // 誰も実行できなかった場合、失敗とする
                SetState(State.Failure);
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
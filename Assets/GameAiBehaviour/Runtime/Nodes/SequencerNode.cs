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
            public Logic(IBehaviourTreeController controller, SequencerNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 順番に実行
                for (var i = 0; i < Node.children.Length; i++) {
                    var node = Node.children[i];
                    yield return ExecuteNodeRoutine(node, SetState);

                    // 成功していたら待機
                    if (State == State.Success) {
                        yield return this;
                    }
                    // 失敗していた場合、そのまま失敗として終了
                    else if (State == State.Failure) {
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
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
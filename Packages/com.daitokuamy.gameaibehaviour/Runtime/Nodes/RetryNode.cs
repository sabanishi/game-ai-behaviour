using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// リトライノード
    /// </summary>
    public sealed class RetryNode : DecoratorNode {
        private class Logic : Logic<RetryNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, RetryNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 子がいない場合は失敗
                if (Node.child == null) {
                    SetState(State.Failure);
                    yield break;
                }
                
                // 接続先ノードの実行
                yield return ExecuteNodeRoutine(Node.child, SetState);

                // 指定回数繰り返す
                if (State == State.Failure) {
                    for (var i = 0; i < Node.retryCount; i++) {
                        // 接続先ノードの実行
                        yield return ExecuteNodeRoutine(Node.child, SetState);

                        // 失敗していたらリトライ
                        if (State == State.Failure) {
                            // 最後だったら終わる
                            if (i >= Node.retryCount - 1) {
                                break;
                            }

                            yield return this;
                        }
                        // 成功していたら完了
                        else if (State == State.Success) {
                            yield break;
                        }
                    }
                }
            }
        }

        [Tooltip("リトライ上限")]
        public int retryCount = 3;

        public override string Description => $"RetryCount:{retryCount}";

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
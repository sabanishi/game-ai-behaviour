using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// ランダム選択用ノード
    /// </summary>
    public sealed class RandomSelectorNode : CompositeNode {
        private class Logic : Logic<RandomSelectorNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, RandomSelectorNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // ランダムに実行
                var orderedIndices = Enumerable.Range(0, Node.children.Length)
                    .Where(i => Node.weights[i] > float.Epsilon)
                    .OrderByDescending(i => {
                        var weight = Node.weights[i];
                        return Random.Range(0, weight);
                    })
                    .ToArray();
                
                for (var i = 0; i < orderedIndices.Length; i++) {
                    var index = orderedIndices[i];
                    var node = Node.children[index];
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

        [Tooltip("選択重み")]
        public FloatChildNodeValueGroup weights;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// ランダムシーケンスノード
    /// </summary>
    public sealed class RandomSequencerNode : CompositeNode {
        private class Logic : Logic<RandomSequencerNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeRunner runner, RandomSequencerNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                if (Node.children.Length <= 0) {
                    SetState(Node.FailureState);
                    yield break;
                }

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

        [Tooltip("実行順番抽選の重み")]
        public FloatChildNodeValueGroup weights;

        public override float NodeWidth => 185.0f;

#if UNITY_EDITOR
        public override string Description {
            get {
                if (children.Length != weights.Count) {
                    var weightCount = weights.Count;
                    
                    // 子の要素数にWeightの要素数を合わせる
                    for (var i = weightCount; i < children.Length; i++) {
                        weights.Set(i, 1.0f);
                    }

                    if (weightCount > children.Length) {
                        weights.SetCount(children.Length);
                    }
                }
                
                if (!weights.Any()) {
                    return "Empty Children";
                }
                
                var lines = weights.Select((x, i) => {
                    var nodeName = BehaviourTreeEditorUtility.GetNodeDisplayTitle(children[i]);
                    return $"[{x}]{nodeName}";
                });
                return string.Join('\n', lines);
            }
        }
#endif

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
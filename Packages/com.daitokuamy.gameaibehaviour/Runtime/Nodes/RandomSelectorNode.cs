using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// ランダム選択用ノード
    /// </summary>
    public sealed class RandomSelectorNode : CompositeNode {
        private class Logic : Logic<RandomSelectorNode> {
            private readonly List<int> _sourceIndices = new();
            private readonly List<int> _destIndices = new();
            
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
                _sourceIndices.Clear();
                _destIndices.Clear();
                _sourceIndices.AddRange(Enumerable.Range(0, Node.children.Length));

                while (_sourceIndices.Count > 0) {
                    var totalWeight = _sourceIndices.Sum(x => Node.weights[x]);
                    if (totalWeight <= 0.0f) {
                        break;
                    }
                    
                    var randomValue = RandomRange(0.0f, totalWeight);
                    for (var i = 0; i < _sourceIndices.Count; i++) {
                        var index = _sourceIndices[i];
                        var weight = Node.weights[index];
                        if (weight <= 0.0f) {
                            continue;
                        }
                    
                        randomValue -= weight;
                        if (randomValue <= 0.0f) {
                            _sourceIndices.RemoveAt(i--);
                            _destIndices.Add(index);
                        }
                    }
                }
                
                for (var i = 0; i < _destIndices.Count; i++) {
                    var index = _destIndices[i];
                    var node = Node.children[index];
                    yield return ExecuteNodeRoutine(node, SetState);

                    // 成功していた場合、完了とする
                    if (State == State.Success) {
                        yield break;
                    }
                }

                // 誰も実行できなかった場合、失敗とする
                SetState(Node.FailureState);
            }
        }

        [Tooltip("選択重み")]
        public FloatChildNodeValueGroup weights;

        public override float NodeWidth => 170.0f;

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
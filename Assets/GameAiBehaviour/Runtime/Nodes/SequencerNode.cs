using System.Collections;
using UnityEngine;

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
            /// 更新ルーチン
            /// </summary>
            protected override IEnumerator UpdateRoutineInternal() {
                // 順番に実行
                for (var i = 0; i < Node.children.Length; i++) {
                    var node = Node.children[i];
                    yield return UpdateNodeRoutine(node, SetState);
                    
                    // 成功していたらRunning扱い
                    if (State == State.Success) {
                        SetState(State.Running);
                        yield return null;
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
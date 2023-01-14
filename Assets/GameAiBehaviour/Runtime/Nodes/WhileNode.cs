using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 繰り返しノード
    /// </summary>
    public sealed class WhileNode : DecoratorNode {
        private class Logic : Logic<WhileNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, WhileNode node) : base(controller, node) {
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
                
                // 条件判定に失敗するまで繰り返す
                while (Node.conditions.Check(Controller.Blackboard)) {
                    // 接続先ノードの実行
                    yield return ExecuteNodeRoutine(Node.child, SetState);
                    
                    // 成功していたら処理継続
                    if (State == State.Success) {
                        yield return this;
                    }
                    // 失敗していたら失敗して抜け出す
                    else if (State == State.Failure) {
                        yield break;
                    }
                }
            }
        }
        
        [Tooltip("条件")]
        public ConditionGroup conditions;

        public override string Description => string.Join("\n", conditions.conditions.Select(x => x.ConditionTitle));

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
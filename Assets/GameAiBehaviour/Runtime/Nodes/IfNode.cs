using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 条件判定ノード
    /// </summary>
    public sealed class IfNode : DecoratorNode {
        private class Logic : Logic<IfNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, IfNode node) : base(controller, node) {
            }

            /// <summary>
            /// 更新ルーチン
            /// </summary>
            protected override IEnumerator UpdateRoutineInternal() {
                // 子がいない場合は失敗
                if (Node.child == null) {
                    SetState(State.Failure);
                    yield break;
                }
                
                // 条件判定に失敗したら失敗
                if (!Node.conditions.Check(Controller.Blackboard)) {
                    SetState(State.Failure);
                    yield break;
                }

                // 接続先ノードの実行
                yield return UpdateNodeRoutine(Node.child, SetState);
            }
        }
        
        [Tooltip("条件")]
        public ConditionGroup conditions = new ConditionGroup();

        public override string Description => string.Join("\n", conditions.conditions.Select(x => x.ConditionTitle));

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
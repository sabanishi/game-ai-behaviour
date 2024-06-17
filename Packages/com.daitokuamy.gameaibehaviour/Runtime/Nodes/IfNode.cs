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
            public Logic(IBehaviourTreeRunner runner, IfNode node) : base(runner, node) {
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

                // 条件判定に失敗したら失敗
                if (!Node.conditions.Check(Controller)) {
                    SetState(State.Failure);
                    yield break;
                }

                // 接続先ノードの実行
                yield return ExecuteNodeRoutine(Node.child, SetState);
            }
        }

        [Tooltip("条件")]
        public ConditionGroup conditions = new ConditionGroup();

        public override string Description => string.Join("\n", conditions.conditions.Select(x => x.ConditionTitle));
        public override float NodeWidth => 200.0f;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
using System;
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
            /// 実行処理
            /// </summary>
            protected override State OnUpdate() {
                if (Node.child == null) {
                    return State.Failure;
                }
                
                // 条件判定
                if (!Node.conditions.Check(Controller.Blackboard)) {
                    return State.Failure;
                }

                // 接続先ノードの実行
                UpdateNode(Node.child);

                return State;
            }

            /// <summary>
            /// 子要素の更新通知
            /// </summary>
            protected override State OnUpdatedChild(ILogic childNodeLogic) {
                // 失敗していたら失敗
                if (childNodeLogic.State == State.Failure) {
                    return State.Failure;
                }
                
                // それ以外は実行中
                return State.Running;
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
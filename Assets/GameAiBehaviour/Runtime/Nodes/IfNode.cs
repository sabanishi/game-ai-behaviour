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
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime) {
                if (Node.child == null) {
                    return State.Failure;
                }
                
                // 条件判定
                foreach (var condition in Node.conditions) {
                    if (!condition.Check()) {
                        return State.Failure;
                    }
                }

                // 接続先ノードの実行
                return UpdateNode(Node.child, deltaTime);
            }
        }
        
        [HideInInspector, Tooltip("条件")]
        public Condition[] conditions;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
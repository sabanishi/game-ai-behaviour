using System;
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
            protected override State OnUpdate(float deltaTime, bool back) {
                if (Node.child == null) {
                    return State.Failure;
                }

                // 戻り実行の際は実行中として終わる
                if (back) {
                    return State.Running;
                }
                
                // 条件判定
                foreach (var condition in Node.conditions) {
                    if (!condition.Check()) {
                        return State.Failure;
                    }
                }

                // 接続先ノードの実行
                var state = UpdateNode(Node.child, deltaTime);
                
                // 接続先が失敗していたらNodeを失敗とする
                if (state == State.Failure) {
                    return State.Failure;
                }

                return State.Running;
            }
        }
        
        [Tooltip("条件")]
        public Condition[] conditions = Array.Empty<Condition>();

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
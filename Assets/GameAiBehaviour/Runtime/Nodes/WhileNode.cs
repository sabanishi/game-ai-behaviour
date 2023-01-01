using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 繰り返しノード
    /// </summary>
    public sealed class WhileNode : DecoratorNode {
        private class Logic : Logic<WhileNode> {
            private bool _entered = false;
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, WhileNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime) {
                if (Node.child == null) {
                    return State.Failure;
                }
                
                // 条件判定
                if (!_entered) {
                    foreach (var condition in Node.conditions) {
                        if (!condition.Check()) {
                            return State.Failure;
                        }
                    }
                }
                else {
                    _entered = true;
                }

                // 接続先ノードの実行
                var state = UpdateNode(Node.child, deltaTime);
                if (state == State.Success) {
                    _entered = false;
                }
                // 接続先が失敗していたらNodeを失敗とする
                else if (state == State.Failure) {
                    _entered = false;
                    return State.Failure;
                }

                return State.Running;
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
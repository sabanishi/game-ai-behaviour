using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 指定回数繰り返しノード
    /// </summary>
    public sealed class RepeatNode : DecoratorNode {
        private class Logic : Logic<RepeatNode> {
            private int _index;
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, RepeatNode node) : base(controller, node) {
            }

            /// <summary>
            /// 開始処理
            /// </summary>
            protected override void OnStart() {
                _index = 0;
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime, bool back) {
                if (Node.child == null) {
                    return State.Failure;
                }
                
                // 条件判定
                if (_index >= Node.count) {
                    return State.Success;
                }

                // 戻り実行の際は実行中として終わる
                if (back) {
                    return State.Running;
                }

                // 接続先ノードの実行
                var state = UpdateNode(Node.child, deltaTime);
                _index++;
                
                if (state == State.Running) {
                    return State.Running;
                }
                
                // 終了していて指定回数を超えている場合、終了
                if (_index >= Node.count) {
                    return State.Success;
                }
                
                return State.Running;
            }
        }
        
        [Tooltip("繰り返し回数")]
        public int count = 1;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
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
            protected override State OnUpdate() {
                if (Node.child == null) {
                    return State.Failure;
                }
                
                // 条件判定
                if (_index >= Node.count) {
                    return State.Success;
                }

                // 接続先ノードの実行
                UpdateNode(Node.child);

                return State;
            }

            /// <summary>
            /// 子要素の更新結果通知
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
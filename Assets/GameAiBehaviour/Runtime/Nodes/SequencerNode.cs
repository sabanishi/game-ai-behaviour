using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// シーケンスノード
    /// </summary>
    public sealed class SequencerNode : CompositeNode {
        private class Logic : Logic<SequencerNode> {
            // 実行対象のNextNodeインデックス
            private int _index = 0;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, SequencerNode node) : base(controller, node) {
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
                var children = Node.children;

                // 先頭ノードを実行
                if (_index < children.Length) {
                    Debug.Log($"Update:{_index}");
                    UpdateNode(children[_index]);
                }
                else {
                    return State.Failure;
                }

                return State;
            }

            /// <summary>
            /// 子要素の更新結果通知
            /// </summary>
            protected override State OnUpdatedChild(ILogic childNodeLogic) {
                // 成功していたらRunningへ
                if (childNodeLogic.State == State.Success) {
                    _index++;
                    Debug.Log($"Increment:{childNodeLogic.TargetNode.GetType()}");
                    if (_index < Node.children.Length) {
                        return State.Running;
                    }

                    return State.Success;
                }

                return childNodeLogic.State;
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
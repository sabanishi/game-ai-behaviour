namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public sealed class SelectorNode : CompositeNode {
        private class Logic : Logic<SelectorNode> {
            private int _index;
            
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, SelectorNode node) : base(controller, node) {
            }

            /// <summary>
            /// 開始時処理
            /// </summary>
            protected override void OnStart() {
                _index = 0;
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate() {
                // 先頭を実行
                if (_index < Node.children.Length) {
                    var node = Node.children[_index];
                    UpdateNode(node);
                }
                else {
                    return State.Failure;
                }

                return State;
            }

            /// <summary>
            /// 子要素の更新通知
            /// </summary>
            protected override State OnUpdatedChild(ILogic childNodeLogic) {
                // 実行に失敗したら次を実行
                if (childNodeLogic.State == State.Failure) {
                    _index++;
                    if (_index < Node.children.Length) {
                        UpdateNode(Node.children[_index]);
                        return State;
                    }
                }
                
                // 子要素の状態と同じにする
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
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
            protected override State OnUpdate(float deltaTime, bool back) {
                var children = Node.children;
                
                // 実行ノードが全て終わった
                if (_index >= children.Length) {
                    return State.Success;
                }

                // 戻り実行の際は実行中として終わる
                if (back) {
                    return State.Running;
                }

                // 接続先ノード実行
                if (_index < children.Length) {
                    var state = UpdateNode(children[_index], deltaTime);
                    _index++;

                    // 失敗していたらSequenceNode自体を失敗にする
                    if (state == State.Failure) {
                        return State.Failure;
                    }
                }

                // 全てのNodeが実行終わっていたら完了
                if (_index >= children.Length) {
                    return State.Success;
                }

                // それ以外は継続中
                return State.Running;
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
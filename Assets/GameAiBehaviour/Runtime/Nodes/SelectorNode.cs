namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public sealed class SelectorNode : CompositeNode {
        private class Logic : Logic<SelectorNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, SelectorNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime, bool back) {
                // 戻り実行の際は完了扱い
                if (back) {
                    return State.Success;
                }
                
                foreach (var child in Node.children) {
                    var state = UpdateNode(child, deltaTime);
                    if (state != State.Failure) {
                        return state;
                    }
                }

                return State.Failure;
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
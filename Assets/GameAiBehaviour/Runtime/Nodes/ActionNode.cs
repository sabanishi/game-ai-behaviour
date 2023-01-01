namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノード
    /// </summary>
    public abstract class ActionNode : Node {
        private class Logic : Logic<ActionNode> {
            private IActionNodeHandler _actionNodeHandler;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, ActionNode node) : base(controller, node) {
            }

            /// <summary>
            /// 開始処理
            /// </summary>
            protected override void OnStart() {
                _actionNodeHandler = Controller.GetActionHandler(Node);
                _actionNodeHandler?.OnEnter(Node);
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime, bool back) {
                // 更新に続きがあればRunning
                if (_actionNodeHandler != null && _actionNodeHandler.OnUpdate(Node, deltaTime)) {
                    return State.Running;
                }
                
                return State.Success;
            }

            /// <summary>
            /// 停止時処理
            /// </summary>
            protected override void OnStop() {
                _actionNodeHandler?.OnExit(Node);
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
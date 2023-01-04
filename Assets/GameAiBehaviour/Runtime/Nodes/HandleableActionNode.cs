namespace GameAiBehaviour {
    /// <summary>
    /// ハンドリング拡張可能な実行ノード（アプリケーション依存度の高いActionNodeはこちらを使う）
    /// </summary>
    public abstract class HandleableActionNode : ActionNode {
        private class Logic : Logic<HandleableActionNode> {
            private IActionNodeHandler _actionNodeHandler;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, HandleableActionNode node) : base(controller, node) {
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
                // Handlerがあればそれを使用
                if (_actionNodeHandler != null) {
                    var result = _actionNodeHandler.OnUpdate(Node, deltaTime);
                    return result;
                }
                
                // 無ければそのまま終わる
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
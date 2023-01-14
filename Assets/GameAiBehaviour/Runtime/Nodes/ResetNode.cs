using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// 思考リセット用ノード
    /// </summary>
    public sealed class ResetNode : ActionNode {
        private class Logic : Logic<ResetNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, ResetNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 思考リセット
                Controller.ResetThink();
                // 完了
                SetState(State.Success);
                yield break;
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
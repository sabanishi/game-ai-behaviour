using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 待ちノード
    /// </summary>
    public sealed class WaitNode : ActionNode {
        private class Logic : Logic<WaitNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, WaitNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                var startTime = Controller.ThinkTime;

                // 時間経過するまで待ち合わせ
                while (Controller.ThinkTime - startTime < Node.duration) {
                    yield return this;
                }

                // 完了
                SetState(State.Success);

                // TickTimerのリセット
                if (Node.resetTickTimer) {
                    Controller.ResetTickTimer();
                }
            }
        }

        [Tooltip("待ち時間")]
        public float duration = 1.0f;
        [Tooltip("待ち完了時に次の思考をIntervalを無視して行うか")]
        public bool resetTickTimer = true;

        public override string Description => $"{duration} sec";

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
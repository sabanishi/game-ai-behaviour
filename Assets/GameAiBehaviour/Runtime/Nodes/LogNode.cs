using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// ログ出力ノード
    /// </summary>
    public sealed class LogNode : ActionNode {
        private class Logic : Logic<LogNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, LogNode node) : base(controller, node) {
            }

            /// <summary>
            /// 更新ルーチン
            /// </summary>
            protected override IEnumerator UpdateRoutineInternal() {
                Debug.unityLogger.Log(Node.logType, Node.text);
                SetState(State.Success);
                yield break;
            }
        }

        [Tooltip("ログモード")]
        public LogType logType = LogType.Log;
        [Tooltip("出力内容のログ")]
        public string text = "";

        public override string Description => $"[{logType}]{text}";

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
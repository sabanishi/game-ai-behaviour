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
            public Logic(IBehaviourTreeRunner runner, LogNode node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteActionRoutineInternal() {
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
        public override ILogic CreateLogic(IBehaviourTreeRunner runner) {
            return new Logic(runner, this);
        }
    }
}
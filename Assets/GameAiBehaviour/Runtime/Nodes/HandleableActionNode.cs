using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// ハンドリング拡張可能な実行ノード（アプリケーション依存度の高いActionNodeはこちらを使う）
    /// </summary>
    public abstract class HandleableActionNode : ActionNode {
        private class Logic : Logic<HandleableActionNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, HandleableActionNode node) : base(controller, node) {
            }

            /// <summary>
            /// 更新ルーチン
            /// </summary>
            protected sealed override IEnumerator UpdateRoutineInternal() {
                var handler = Controller.GetActionHandler(Node);

                // Handlerが無ければ、ログを出して終了
                if (handler == null) {
                    Debug.Log($"Invoke ActionNode[{GetType().Name}]");
                    SetState(State.Success);
                    yield break;
                }

                // 開始処理実行、Enterで失敗したらその時点でエラー
                if (!handler.OnEnter(Node)) {
                    SetState(State.Failure);
                    handler.OnEnter(Node);
                    yield break;
                }

                // 更新処理実行
                while (true) {
                    var state = handler.OnUpdate(Node);
                    if (state != IActionNodeHandler.State.Running) {
                        SetState(state == IActionNodeHandler.State.Success ? State.Success : State.Failure);
                        break;
                    }

                    yield return this;
                }

                // 終了処理実行
                handler.OnExit(Node);
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
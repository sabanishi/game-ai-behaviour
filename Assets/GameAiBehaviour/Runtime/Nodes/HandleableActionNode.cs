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
                
                // 開始処理実行
                handler.OnEnter(Node);
                
                // 更新処理実行
                while (true) {
                    SetState(handler.OnUpdate(Node));
                    
                    // 実行中なら繰り返す
                    if (State == State.Running) {
                        yield return null;
                        continue;
                    }

                    // 実行が終わった場合、終了させる
                    break;
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
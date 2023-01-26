using System.Collections;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノード
    /// </summary>
    public abstract class ActionNode : Node {
        protected new abstract class Logic<T> : Node.Logic<T>
            where T : ActionNode {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            protected Logic(IBehaviourTreeRunner runner, T node) : base(runner, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected sealed override IEnumerator ExecuteRoutineInternal() {
                // サブルーチン実行
                if (Node.subRoutine != null) {
                    Controller.SetSubRoutine(this, Node.subRoutine);
                }

                // 子の実行処理を呼び出し
                yield return ExecuteActionRoutineInternal();
                
                // サブルーチン停止
                Controller.ResetSubRoutine(this);
            }

            /// <summary>
            /// ActionNode用のLogic記述箇所
            /// </summary>
            protected abstract IEnumerator ExecuteActionRoutineInternal();
        }
        
        [Tooltip("実行中に動かすサブルーチン")]
        public FunctionRootNode subRoutine;

#if UNITY_EDITOR
        public sealed override string SubTitle =>
            subRoutine == null ? "" : $"SubRoutine:{BehaviourTreeEditorUtility.GetNodeDisplayTitle(subRoutine)}";
#endif
    }
}
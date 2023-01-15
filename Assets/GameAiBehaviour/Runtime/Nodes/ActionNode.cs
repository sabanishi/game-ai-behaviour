using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノード
    /// </summary>
    public abstract class ActionNode : Node {
        [Tooltip("実行中に動かすサブルーチン")]
        public FunctionRootNode subRoutine;
    }
}
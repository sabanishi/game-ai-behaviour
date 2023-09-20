using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public abstract class CompositeNode : Node {
        [HideInInspector, Tooltip("接続先ノード")]
        public Node[] children = Array.Empty<Node>();
        [Tooltip("強制的に成功にするか")]
        public bool forceSuccess;

        /// <summary>サブタイトル</summary>
        public override string SubTitle => forceSuccess ? "(Force Success)" : base.SubTitle;

        /// <summary>失敗した時のState</summary>
        protected State FailureState => forceSuccess ? State.Success : State.Failure;
    }
}
using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public abstract class CompositeNode : Node {
        [Tooltip("接続先ノード")]
        public Node[] children = Array.Empty<Node>();
    }
}
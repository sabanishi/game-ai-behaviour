using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public abstract class CompositeNode : Node {
        [HideInInspector, Tooltip("接続先ノード")]
        public Node[] children = new Node[0];
    }
}
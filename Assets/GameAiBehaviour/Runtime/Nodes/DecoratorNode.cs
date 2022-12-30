using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 接続情報
    /// </summary>
    public abstract class DecoratorNode : Node {
        [HideInInspector, Tooltip("接続先ノード")]
        public Node child;
    }
}
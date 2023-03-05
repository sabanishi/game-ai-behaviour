using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeアセット
    /// </summary>
    [CreateAssetMenu(fileName = "dat_behaviour_tree_hoge.asset", menuName = "GameAiBehaviour/Create Tree")]
    public class BehaviourTree : ScriptableObject {
        [Tooltip("メモ")]
        public string memo = "";
        [HideInInspector, Tooltip("ルートノードへの参照")]
        public RootNode rootNode;
        [HideInInspector, Tooltip("所持ノードリスト")]
        public Node[] nodes = Array.Empty<Node>();
        [HideInInspector, Tooltip("Blackboard定義用アセット")]
        public BlackboardAsset blackboardAsset;
    }
}
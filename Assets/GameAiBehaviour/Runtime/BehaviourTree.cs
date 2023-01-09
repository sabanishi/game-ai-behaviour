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
        [HideInInspector, Tooltip("所持ノードリスト")]
        public Node[] nodes = Array.Empty<Node>();
        [HideInInspector, Tooltip("Blackboardに列挙するプロパティ")]
        public Property[] properties = Array.Empty<Property>();
    }
}
using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// プロパティ格納管理用クラス
    /// </summary>
    [CreateAssetMenu(fileName = "dat_blackboard_hoge.asset", menuName = "GameAiBehaviour/Create Blackboard")]
    public class BlackboardAsset : ScriptableObject {
        [Tooltip("Blackboardに列挙するプロパティ")]
        public Property[] properties = Array.Empty<Property>();
        [Tooltip("Blackboardに列挙する定数プロパティ")]
        public Property[] constProperties = Array.Empty<Property>();
    }
}
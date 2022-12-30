using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 接続情報
    /// </summary>
    public abstract class Condition : ScriptableObject {
        /// <summary>
        /// 判定
        /// </summary>
        public abstract bool Check();
    }
}
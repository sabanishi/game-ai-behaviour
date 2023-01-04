using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 条件基底
    /// </summary>
    public abstract class Condition : ScriptableObject {
        /// <summary>
        /// 判定
        /// </summary>
        public abstract bool Check(Blackboard blackboard);
    }
}
using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 条件グループ
    /// </summary>
    [Serializable]
    public sealed class ConditionGroup {
        [Tooltip("判定条件リスト")]
        public Condition[] conditions = Array.Empty<Condition>();

        /// <summary>
        /// 条件の数
        /// </summary>
        public int Count => conditions.Length;
        
        /// <summary>
        /// 判定
        /// </summary>
        public bool Check(Blackboard blackboard) {
            // AND条件で判定を行う
            foreach (var condition in conditions) {
                if (condition == null) {
                    continue;
                }

                if (!condition.Check(blackboard)) {
                    return false;
                }
            }

            return true;
        }
    }
}
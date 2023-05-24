using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 条件グループ
    /// </summary>
    [Serializable]
    public sealed class ConditionGroup {
        // 判定条件リスト
        public Condition[] conditions = Array.Empty<Condition>();

        /// <summary>
        /// 判定
        /// </summary>
        public bool Check(IBehaviourTreeController controller) {
            // AND条件で判定を行う
            foreach (var condition in conditions) {
                if (condition == null) {
                    continue;
                }

                if (!condition.Check(controller)) {
                    return false;
                }
            }

            return true;
        }
    }
}
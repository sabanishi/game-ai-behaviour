namespace GameAiBehaviour {
    /// <summary>
    /// 条件ハンドリング用インターフェース
    /// </summary>
    public interface IConditionHandler {
        /// <summary>
        /// 条件判定
        /// </summary>
        bool Check(HandleableCondition condition, Blackboard blackboard);
    }

    /// <summary>
    /// 実行ノードハンドリング用基底クラス
    /// </summary>
    public abstract class ConditionHandler<TCondition> : IConditionHandler
        where TCondition : HandleableCondition {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConditionHandler() {
        }

        /// <summary>
        /// 条件判定
        /// </summary>
        bool IConditionHandler.Check(HandleableCondition condition, Blackboard blackboard) {
            return CheckInternal((TCondition)condition, blackboard);
        }

        /// <summary>
        /// 条件の判定
        /// </summary>
        /// <returns>判定結果</returns>
        protected abstract bool CheckInternal(TCondition condition, Blackboard blackboard);
    }
}
using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 条件ハンドリング用基底クラス
    /// </summary>
    public sealed class ObserveConditionHandler<TCondition> : ConditionHandler<TCondition>
        where TCondition : HandleableCondition {
        private Func<TCondition, Blackboard, bool> _checkFunc;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObserveConditionHandler() {
        }

        /// <summary>
        /// 更新時処理登録
        /// </summary>
        public void SetCheckFunc(Func<TCondition, Blackboard, bool> checkFunc) {
            _checkFunc = checkFunc;
        }

        /// <summary>
        /// 条件の判定
        /// </summary>
        /// <returns>判定結果</returns>
        protected override bool CheckInternal(TCondition condition, Blackboard blackboard) {
            return _checkFunc?.Invoke(condition, blackboard) ?? false;
        }
    }
}
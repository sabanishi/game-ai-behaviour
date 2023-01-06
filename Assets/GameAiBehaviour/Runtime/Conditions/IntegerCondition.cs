using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 整数型比較用条件
    /// </summary>
    public class IntegerCondition : NumericCondition<int, IntegerCondition.ValueObject> {
        /// <summary>
        /// 格納用の値型
        /// </summary>
        [Serializable]
        public class ValueObject : ValueObject<int> {
        }
        
        public override string ConditionTitle => $"[Int]{base.ConditionTitle}";

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override int GetPropertyValue(Blackboard blackboard, string propertyName, int defaultValue) {
            return blackboard.GetInteger(propertyName, defaultValue);
        }
    }
}
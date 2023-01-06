using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 小数型比較用条件
    /// </summary>
    public class FloatCondition : NumericCondition<float, FloatCondition.ValueObject> {
        /// <summary>
        /// 格納用の値型
        /// </summary>
        [Serializable]
        public class ValueObject : ValueObject<float> {
        }
        
        public override string ConditionTitle => $"[Float]{base.ConditionTitle}";

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override float GetPropertyValue(Blackboard blackboard, string propertyName, float defaultValue) {
            return blackboard.GetFloat(propertyName, defaultValue);
        }
    }
}
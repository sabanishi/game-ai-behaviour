using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 文字列比較用条件
    /// </summary>
    public class StringCondition : LogicalCondition<string, StringCondition.ValueObject> {
        /// <summary>
        /// 格納用の値型
        /// </summary>
        [Serializable]
        public class ValueObject : ValueObject<string> {
        }  

        public override string ConditionTitle => $"[String]{base.ConditionTitle}";

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override string GetPropertyValue(Blackboard blackboard, string propertyName, string defaultValue) {
            return blackboard.GetString(propertyName, defaultValue);
        }
    }
}
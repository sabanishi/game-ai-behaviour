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

        // 条件タイトル
        public override string ConditionTitle => $"[String]{base.ConditionTitle}";
        // GUI描画の際に使用するPropertyの使用可能型フィルタ
        protected override Property.Type[] PropertyTypeFilters => new[] { Property.Type.String };

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override string GetPropertyValue(Blackboard blackboard, string propertyName, string defaultValue) {
            return blackboard.GetString(propertyName, defaultValue);
        }
    }
}
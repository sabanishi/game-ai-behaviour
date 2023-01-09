using System;

namespace GameAiBehaviour {
    /// <summary>
    /// bool値比較用条件
    /// </summary>
    public class BooleanCondition : LogicalCondition<bool, BooleanCondition.ValueObject> {
        /// <summary>
        /// 格納用の値型
        /// </summary>
        [Serializable]
        public class ValueObject : ValueObject<bool> {
        }

        // 条件タイトル
        public override string ConditionTitle => $"[Bool]{base.ConditionTitle}";
        // GUI描画の際に使用するPropertyの使用可能型フィルタ
        protected override Property.Type[] PropertyTypeFilters => new[] { Property.Type.Boolean };

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override bool GetPropertyValue(Blackboard blackboard, string propertyName, bool defaultValue) {
            return blackboard.GetBoolean(propertyName, defaultValue);
        }
    }
}
using System;
using System.Linq;

namespace GameAiBehaviour {
    /// <summary>
    /// 数値比較用条件
    /// </summary>
    public abstract class NumericCondition<T, TValueObject> : ValueCondition<T, TValueObject>
        where T : IComparable
        where TValueObject : ValueObject<T> {
        // 演算子
        public NumericOperatorType operatorType = NumericOperatorType.Equal;

        // 条件タイトル
        public override string ConditionTitle => $"{leftValue} {operatorType.GetMark()} {rightValue}";
        // GUI描画の際に使用するPropertyの使用可能型フィルタ
        protected override Property.Type[] PropertyTypeFilters => new[] { Property.Type.Integer, Property.Type.Float };

        /// <summary>
        /// 値の判定
        /// </summary>
        protected sealed override bool CheckInternal(T left, T right) {
            return operatorType.Check(left, right);
        }

        /// <summary>
        /// オペレータタイプの表示名
        /// </summary>
        protected override string[] GetOperatorTypeLabels() {
            return ((NumericOperatorType[])Enum.GetValues(typeof(NumericOperatorType)))
                .Select(x => x.GetMark()).ToArray();
        }
    }
}
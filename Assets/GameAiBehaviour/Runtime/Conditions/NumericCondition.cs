using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 数値比較用条件
    /// </summary>
    public abstract class NumericCondition<T, TValueObject> : ValueCondition<T, TValueObject>
        where T : IComparable
        where TValueObject : ValueObject<T> {

        // 演算子
        public NumericOperatorType operatorType = NumericOperatorType.Equal;  

        public override string ConditionTitle => $"{leftValue} {operatorType.GetMark()} {rightValue}";

        /// <summary>
        /// 値の判定
        /// </summary>
        protected sealed override bool CheckInternal(T left, T right) {
            var result = left.CompareTo(right);
            switch (operatorType) {
                case NumericOperatorType.Equal:
                    return result == 0;
                case NumericOperatorType.NotEqual:
                    return result != 0;
                case NumericOperatorType.Less:
                    return result < 0;
                case NumericOperatorType.LessEqual:
                    return result <= 0;
                case NumericOperatorType.Greater:
                    return result > 0;
                case NumericOperatorType.GreaterEqual:
                    return result >= 0;
            }

            return false;
        }
    }
}
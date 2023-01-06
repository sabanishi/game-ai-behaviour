using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 文字列比較用条件
    /// </summary>
    public abstract class LogicalCondition<T, TValueObject> : ValueCondition<T, TValueObject>
        where T : IComparable
        where TValueObject : ValueObject<T> {
        
        // 演算子
        public OperatorType operatorType = OperatorType.Equal;  

        public override string ConditionTitle => $"{leftValue} {operatorType.GetMark()} {rightValue}";

        /// <summary>
        /// 値の判定
        /// </summary>
        protected sealed override bool CheckInternal(T left, T right) {
            switch (operatorType) {
                case OperatorType.Equal:
                    return left.Equals(right);
                case OperatorType.NotEqual:
                    return !left.Equals(right);
            }

            return false;
        }
    }
}
using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 単純比較用オペレータ
    /// </summary>
    public enum OperatorType {
        Equal,
        NotEqual,
    }

    /// <summary>
    /// 数値比較用オペレータ
    /// </summary>
    public enum NumericOperatorType {
        Equal,
        NotEqual,
        Less,
        LessEqual,
        Greater,
        GreaterEqual,
    }

    /// <summary>
    /// OperatorType用の拡張メソッド
    /// </summary>
    public static class OperatorTypeExtensions {
        /// <summary>
        /// 記号の取得
        /// </summary>
        public static string GetMark(this OperatorType source) {
            switch (source) {
                case OperatorType.Equal:
                    return "==";
                case OperatorType.NotEqual:
                    return "!=";
            }

            return source.ToString();
        }

        /// <summary>
        /// 記号の取得
        /// </summary>
        public static string GetMark(this NumericOperatorType source) {
            switch (source) {
                case NumericOperatorType.Equal:
                    return "==";
                case NumericOperatorType.NotEqual:
                    return "!=";
                case NumericOperatorType.Less:
                    return "<";
                case NumericOperatorType.LessEqual:
                    return "<=";
                case NumericOperatorType.Greater:
                    return ">";
                case NumericOperatorType.GreaterEqual:
                    return ">=";
            }

            return source.ToString();
        }

        /// <summary>
        /// 比較処理
        /// </summary>
        public static bool Check<T>(this OperatorType source, T left, T right) {
            switch (source) {
                case OperatorType.Equal:
                    return left.Equals(right);
                case OperatorType.NotEqual:
                    return !left.Equals(right);
            }

            return false;
        }

        /// <summary>
        /// 比較処理
        /// </summary>
        public static bool Check<T>(this NumericOperatorType source, T left, T right)
            where T : IComparable {
            var result = left.CompareTo(right);
            switch (source) {
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
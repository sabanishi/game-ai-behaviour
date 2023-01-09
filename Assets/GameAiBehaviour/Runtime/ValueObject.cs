using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 格納用の値型
    /// </summary>
    [Serializable]
    public abstract class ValueObject<T> {
        public bool useProperty;
        public T constValue = default;
        public string propertyName = "";

        /// <summary>
        /// 文字列化
        /// </summary>
        public override string ToString() {
            if (!useProperty) {
                return constValue?.ToString();
            }

            return $"<{propertyName}>";
        }
    }
}
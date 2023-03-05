using System;

namespace GameAiBehaviour {
    /// <summary>
    /// プロパティ情報
    /// </summary>
    [Serializable]
    public class Property {
        // プロパティのタイプ
        public enum Type {
            Integer,
            Float,
            String,
            Boolean,
        }

        // プロパティ名
        public string propertyName = "Property Name";
        // プロパティタイプ
        public Type propertyType = Type.Integer;

        // 整数型の値
        public int integerValue;
        // 小数型の値
        public float floatValue;
        // 文字列型の値
        public string stringValue;
        // 論理型の値
        public bool booleanValue;
    }
}
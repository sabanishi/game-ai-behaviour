using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
            Object,
        }
        
        [Tooltip("プロパティ名")]
        public string propertyName = "Property Name";
        [Tooltip("プロパティタイプ")]
        public Type propertyType = Type.Integer;

        [Tooltip("整数型の値")]
        public int integerValue;
        [Tooltip("小数型の値")]
        public float floatValue;
        [Tooltip("文字列型の値")]
        public string stringValue;
        [Tooltip("論理型の値")]
        public bool booleanValue;
        [Tooltip("オブジェクト型の値")]
        public Object objectValue;
    }
}
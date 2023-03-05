using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 子ノードに値を付与するための基底クラス
    /// </summary>
    public abstract class ChildNodeValueGroup<T> {
        [SerializeField]
        private T[] _values;
        
        public T this[int index] {
            get {
                if (index < 0 || index >= _values.Length) {
                    return default;
                }
                return _values[index];
            }
        }
    }

    [Serializable]
    public class FloatChildNodeValueGroup : ChildNodeValueGroup<float> {}
}
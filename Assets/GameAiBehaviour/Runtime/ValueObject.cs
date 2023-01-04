﻿using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 格納用の値型
    /// </summary>
    [Serializable]
    public abstract class ValueObject<T> {
        public T constValue;
        public string propertyName;
    }
}
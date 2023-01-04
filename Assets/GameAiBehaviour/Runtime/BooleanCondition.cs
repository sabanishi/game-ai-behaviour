using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// bool値比較用条件
    /// </summary>
    public class BooleanCondition : ValueCondition<bool, BooleanCondition.ValueObject> {
        /// <summary>
        /// 格納用の値型
        /// </summary>
        [Serializable]
        public class ValueObject : ValueObject<bool> {
        }

        /// <summary>
        /// 値の判定
        /// </summary>
        protected override bool CheckInternal(bool left, bool right) {
            return left == right;
        }

        /// <summary>
        /// プロパティの取得
        /// </summary>
        protected override bool GetPropertyValue(Blackboard blackboard, string propertyName, bool defaultValue) {
            return blackboard.GetBoolean(propertyName, defaultValue);
        }
    }
}
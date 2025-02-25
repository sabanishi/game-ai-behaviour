using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 乱数生成用インターフェース
    /// </summary>
    public sealed class UnityRandom : IRandom {
        /// <summary>
        /// 浮動小数範囲ランダム
        /// </summary>
        public float Range(float min, float max) {
            return Random.Range(min, max);
        }
    }
}
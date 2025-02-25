namespace GameAiBehaviour {
    /// <summary>
    /// 乱数生成用インターフェース
    /// </summary>
    public interface IRandom {
        /// <summary>
        /// 浮動小数範囲ランダム
        /// </summary>
        float Range(float min, float max);
    }
}
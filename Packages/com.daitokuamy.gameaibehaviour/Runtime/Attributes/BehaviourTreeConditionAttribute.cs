using System;
using System.Linq;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeConditionに付与するAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviourTreeConditionAttribute : Attribute {
        /// <summary>Node生成時のPath指定(nullならデフォルト値)</summary>
        public string Path { get; }
        /// <summary>表示名(Pathの一番最後の文字列を使用)</summary>
        public string DisplayName => string.IsNullOrEmpty(Path) ? null : Path.Split("/").Last();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">設定するPath</param>
        public BehaviourTreeConditionAttribute(string path) {
            Path = path;
        }
    }
}

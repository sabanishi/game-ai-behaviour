using System.Collections.Generic;

namespace GameAiBehaviour {
    /// <summary>
    /// プロパティ格納管理用クラス
    /// </summary>
    public class Blackboard {
        private readonly Dictionary<string, int> _intProperties = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _floatProperties = new Dictionary<string, float>();
        private readonly Dictionary<string, bool> _booleanProperties = new Dictionary<string, bool>();
        private readonly Dictionary<string, string> _stringProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, object> _objectProperties = new Dictionary<string, object>();

        /// <summary>
        /// プロパティのクリア
        /// </summary>
        public void Clear() {
            _intProperties.Clear();
            _floatProperties.Clear();
            _booleanProperties.Clear();
            _stringProperties.Clear();
            _objectProperties.Clear();
        }

        /// <summary>
        /// Int型プロパティの取得
        /// </summary>
        public int GetInt(string propertyName, int defaultValue = 0) {
            if (_intProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Int型プロパティ野設定
        /// </summary>
        public void SetInt(string propertyName, int val) {
            _intProperties[propertyName] = val;
        }

        /// <summary>
        /// Bool型プロパティの取得
        /// </summary>
        public bool GetBoolean(string propertyName, bool defaultValue = false) {
            if (_booleanProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Bool型プロパティ野設定
        /// </summary>
        public void SetBoolean(string propertyName, bool val) {
            _booleanProperties[propertyName] = val;
        }
    }
}
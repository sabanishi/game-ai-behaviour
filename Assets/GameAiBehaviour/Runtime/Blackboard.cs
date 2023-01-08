using System.Collections.Generic;
using System.Linq;

namespace GameAiBehaviour {
    /// <summary>
    /// プロパティ格納管理用クラス
    /// </summary>
    public class Blackboard {
        private readonly Dictionary<string, int> _integerProperties = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _floatProperties = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _stringProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _booleanProperties = new Dictionary<string, bool>();

        public string[] IntegerPropertyNames => _integerProperties.Keys.ToArray();
        public string[] FloatPropertyNames => _floatProperties.Keys.ToArray();
        public string[] StringPropertyNames => _stringProperties.Keys.ToArray();
        public string[] BooleanPropertyNames => _booleanProperties.Keys.ToArray();

        /// <summary>
        /// プロパティのクリア
        /// </summary>
        public void Clear() {
            _integerProperties.Clear();
            _floatProperties.Clear();
            _booleanProperties.Clear();
            _stringProperties.Clear();
        }

        /// <summary>
        /// Integer型プロパティの取得
        /// </summary>
        public int GetInteger(string propertyName, int defaultValue = 0) {
            if (_integerProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Integer型プロパティ野設定
        /// </summary>
        public void SetInteger(string propertyName, int val) {
            _integerProperties[propertyName] = val;
        }

        /// <summary>
        /// Float型プロパティの取得
        /// </summary>
        public float GetFloat(string propertyName, float defaultValue = 0) {
            if (_floatProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Float型プロパティ野設定
        /// </summary>
        public void SetFloat(string propertyName, float val) {
            _floatProperties[propertyName] = val;
        }

        /// <summary>
        /// String型プロパティの取得
        /// </summary>
        public string GetString(string propertyName, string defaultValue = "") {
            if (_stringProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// String型プロパティ野設定
        /// </summary>
        public void SetString(string propertyName, string val) {
            _stringProperties[propertyName] = val;
        }

        /// <summary>
        /// Boolean型プロパティの取得
        /// </summary>
        public bool GetBoolean(string propertyName, bool defaultValue = false) {
            if (_booleanProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Boolean型プロパティ野設定
        /// </summary>
        public void SetBoolean(string propertyName, bool val) {
            _booleanProperties[propertyName] = val;
        }
    }
}
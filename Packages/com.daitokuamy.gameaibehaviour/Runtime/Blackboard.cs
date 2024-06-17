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

        private readonly Dictionary<string, int> _constIntegerProperties = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _constFloatProperties = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _constStringProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _constBooleanProperties = new Dictionary<string, bool>();

        public string[] IntegerPropertyNames => _integerProperties.Keys.ToArray();
        public string[] FloatPropertyNames => _floatProperties.Keys.ToArray();
        public string[] StringPropertyNames => _stringProperties.Keys.ToArray();
        public string[] BooleanPropertyNames => _booleanProperties.Keys.ToArray();
        public string[] ConstIntegerPropertyNames => _constIntegerProperties.Keys.ToArray();
        public string[] ConstFloatPropertyNames => _constFloatProperties.Keys.ToArray();
        public string[] ConstStringPropertyNames => _constStringProperties.Keys.ToArray();
        public string[] ConstBooleanPropertyNames => _constBooleanProperties.Keys.ToArray();

        /// <summary>
        /// プロパティのクリア
        /// </summary>
        public void Clear() {
            _integerProperties.Clear();
            _floatProperties.Clear();
            _stringProperties.Clear();
            _booleanProperties.Clear();
            _constIntegerProperties.Clear();
            _constFloatProperties.Clear();
            _constStringProperties.Clear();
            _constBooleanProperties.Clear();
        }

        /// <summary>
        /// Integer型プロパティの取得
        /// </summary>
        public int GetInteger(string propertyName, int defaultValue = 0) {
            if (_integerProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            if (_constIntegerProperties.TryGetValue(propertyName, out val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Integer型プロパティ野設定
        /// </summary>
        public void SetInteger(string propertyName, int val) {
            if (!_integerProperties.ContainsKey(propertyName)) {
                return;
            }
            
            _integerProperties[propertyName] = val;
        }

        /// <summary>
        /// Float型プロパティの取得
        /// </summary>
        public float GetFloat(string propertyName, float defaultValue = 0) {
            if (_floatProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            if (_constFloatProperties.TryGetValue(propertyName, out val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Float型プロパティ野設定
        /// </summary>
        public void SetFloat(string propertyName, float val) {
            if (!_floatProperties.ContainsKey(propertyName)) {
                return;
            }
            
            _floatProperties[propertyName] = val;
        }

        /// <summary>
        /// String型プロパティの取得
        /// </summary>
        public string GetString(string propertyName, string defaultValue = "") {
            if (_stringProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            if (_constStringProperties.TryGetValue(propertyName, out val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// String型プロパティ野設定
        /// </summary>
        public void SetString(string propertyName, string val) {
            if (!_stringProperties.ContainsKey(propertyName)) {
                return;
            }
            
            _stringProperties[propertyName] = val;
        }

        /// <summary>
        /// Boolean型プロパティの取得
        /// </summary>
        public bool GetBoolean(string propertyName, bool defaultValue = false) {
            if (_booleanProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            if (_constBooleanProperties.TryGetValue(propertyName, out val)) {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Boolean型プロパティ野設定
        /// </summary>
        public void SetBoolean(string propertyName, bool val) {
            if (!_booleanProperties.ContainsKey(propertyName)) {
                return;
            }

            _booleanProperties[propertyName] = val;
        }
    }
}
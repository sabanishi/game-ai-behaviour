﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// プロパティ格納管理用クラス
    /// </summary>
    public class Blackboard {
        private readonly Dictionary<string, int> _integerProperties = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _floatProperties = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _stringProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _booleanProperties = new Dictionary<string, bool>();
        private readonly Dictionary<string,GameObject> _gameObjectProperties = new Dictionary<string, GameObject>();

        private readonly Dictionary<string, int> _constIntegerProperties = new Dictionary<string, int>();
        private readonly Dictionary<string, float> _constFloatProperties = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _constStringProperties = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _constBooleanProperties = new Dictionary<string, bool>();
        private readonly Dictionary<string, GameObject> _constGameObjectProperties = new Dictionary<string, GameObject>();

        public string[] IntegerPropertyNames => _integerProperties.Keys.ToArray();
        public string[] FloatPropertyNames => _floatProperties.Keys.ToArray();
        public string[] StringPropertyNames => _stringProperties.Keys.ToArray();
        public string[] BooleanPropertyNames => _booleanProperties.Keys.ToArray();
        public string[] GameObjectPropertyNames => _gameObjectProperties.Keys.ToArray();
        public string[] ConstIntegerPropertyNames => _constIntegerProperties.Keys.ToArray();
        public string[] ConstFloatPropertyNames => _constFloatProperties.Keys.ToArray();
        public string[] ConstStringPropertyNames => _constStringProperties.Keys.ToArray();
        public string[] ConstBooleanPropertyNames => _constBooleanProperties.Keys.ToArray();
        public string[] ConstGameObjectPropertyNames => _constGameObjectProperties.Keys.ToArray();

        /// <summary>
        /// プロパティのクリア
        /// </summary>
        public void Clear() {
            _integerProperties.Clear();
            _floatProperties.Clear();
            _stringProperties.Clear();
            _booleanProperties.Clear();
            _gameObjectProperties.Clear();
            _constIntegerProperties.Clear();
            _constFloatProperties.Clear();
            _constStringProperties.Clear();
            _constBooleanProperties.Clear();
            _constGameObjectProperties.Clear();
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
            if (_constIntegerProperties.ContainsKey(propertyName)) {
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
            if (_constFloatProperties.ContainsKey(propertyName)) {
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
            if (_constStringProperties.ContainsKey(propertyName)) {
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
            if (_constBooleanProperties.ContainsKey(propertyName)) {
                return;
            }

            _booleanProperties[propertyName] = val;
        }
        
        /// <summary>
        /// GameObject型プロパティの取得
        /// </summary>
        public GameObject GetGameObject(string propertyName, GameObject defaultValue = null) {
            if (_gameObjectProperties.TryGetValue(propertyName, out var val)) {
                return val;
            }

            if (_constGameObjectProperties.TryGetValue(propertyName, out val)) {
                return val;
            }

            return defaultValue;
        }
        
        /// <summary>
        /// GameObject型プロパティ野設定
        /// </summary>
        public void SetGameObject(string propertyName, GameObject val) {
            if (_constGameObjectProperties.ContainsKey(propertyName)) {
                return;
            }

            _gameObjectProperties[propertyName] = val;
        }

        /// <summary>
        /// ConstInteger型プロパティ野設定
        /// </summary>
        internal void SetConstInteger(string propertyName, int val) {
            _constIntegerProperties[propertyName] = val;
        }

        /// <summary>
        /// ConstFloat型プロパティ野設定
        /// </summary>
        internal void SetConstFloat(string propertyName, float val) {
            _constFloatProperties[propertyName] = val;
        }

        /// <summary>
        /// ConstString型プロパティ野設定
        /// </summary>
        internal void SetConstString(string propertyName, string val) {
            _constStringProperties[propertyName] = val;
        }

        /// <summary>
        /// ConstBoolean型プロパティ野設定
        /// </summary>
        internal void SetConstBoolean(string propertyName, bool val) {
            _constBooleanProperties[propertyName] = val;
        }
        
        /// <summary>
        /// ConstGameObject型プロパティ野設定
        /// </summary>
        internal void SetConstGameObject(string propertyName, GameObject val) {
            _constGameObjectProperties[propertyName] = val;
        }
    }
}
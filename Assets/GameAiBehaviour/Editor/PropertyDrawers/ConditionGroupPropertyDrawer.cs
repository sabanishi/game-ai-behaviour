using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// ConditionGroupのエディタ拡張
    /// </summary>
    [CustomPropertyDrawer(typeof(ConditionGroup))]
    public class ConditionGroupPropertyDrawer : PropertyDrawer {
        // プロパティ毎の情報
        private class PropertyInfo {
            public SerializedProperty listProperty;
            public ReorderableList reorderableList;
        }
        
        // Conditionの詳細表示用BoxのPadding
        private const float BoxPadding = 2;
        
        private Dictionary<string, PropertyInfo> _propertyInfos = new Dictionary<string, PropertyInfo>();
        private PropertyInfo _propertyInfo;


        /// <summary>
        /// GUI拡張
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Initialize(property);
            using (new EditorGUI.PropertyScope(position, label, property)) {
                var rect = position;
                rect.height = _propertyInfo.reorderableList.GetHeight();
                _propertyInfo.reorderableList.DoList(rect);
                rect.y += rect.height;
                
                // 選択中の物があった場合、その描画を行う
                if (_propertyInfo.reorderableList.index >= 0) {
                    var element = _propertyInfo.listProperty.GetArrayElementAtIndex(_propertyInfo.reorderableList.index);
                    var condition = element.objectReferenceValue as Condition;
                    if (condition != null) {
                        var serializedObj = new SerializedObject(condition);
                        rect.height = condition.GetInspectorGUIHeight(serializedObj) + BoxPadding * 2;
                        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
                        rect = AddPadding(rect, BoxPadding * 0.5f);
                        EditorGUI.DrawRect(rect, new Color(0.25f, 0.25f, 0.25f));
                        rect = AddPadding(rect, BoxPadding * 0.5f);
                        serializedObj.Update();
                        condition.OnInspectorGUI(rect, serializedObj);
                        serializedObj.ApplyModifiedProperties();
                        rect = AddPadding(rect, -BoxPadding);
                        rect.y += rect.height;
                    }
                }
            }
        }

        /// <summary>
        /// GUIの高さ計算
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            Initialize(property);
            var height = 0.0f;
            try {
                height = _propertyInfo.reorderableList.GetHeight();
            }
            catch {
                _propertyInfos.Remove(property.propertyPath);
                height = GetPropertyHeight(property, label);
            }

            // 選択中の物があった場合、その分の高さを加える
            if (_propertyInfo.reorderableList.index >= 0) {
                var element = _propertyInfo.listProperty.GetArrayElementAtIndex(_propertyInfo.reorderableList.index);
                var condition = element.objectReferenceValue as Condition;
                if (condition != null) {
                    height += condition.GetInspectorGUIHeight(new SerializedObject(condition)) + BoxPadding * 2;
                }
            }

            return height;
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize(SerializedProperty property)
        {
            if (_propertyInfos.TryGetValue(property.propertyPath, out _propertyInfo)){
                return;
            }

            _propertyInfo = new PropertyInfo();
            _propertyInfo.listProperty = property.FindPropertyRelative("conditions");

            // ReorderableListを初期化
            var reorderableList = new ReorderableList(property.serializedObject, _propertyInfo.listProperty);
            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, property.displayName);
            reorderableList.drawElementCallback = (rect, index, active, focused) => {
                var prop = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var condition = prop.objectReferenceValue as Condition;
                if (condition != null) {
                    // Labelを表示
                    var labelRect = rect;
                    labelRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(labelRect, $"{condition.ConditionTitle}", EditorStyles.boldLabel);
                }
                else {
                    EditorGUI.PropertyField(rect, prop, GUIContent.none);
                }
            };
            reorderableList.elementHeightCallback = index => {
                var prop = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                var condition = prop.objectReferenceValue as Condition;
                if (condition != null) {
                    // ラベルの高さ
                    return EditorGUIUtility.singleLineHeight;
                }
                return EditorGUI.GetPropertyHeight(prop);
            };
            reorderableList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                var conditionTypes = TypeCache.GetTypesDerivedFrom(typeof(Condition))
                    .Where(x => !x.IsAbstract && !x.IsGenericType)
                    .ToArray();
                foreach (var type in conditionTypes) {
                    var t = type;
                    menu.AddItem(new GUIContent(type.Name), false, () => {
                        // 条件の生成
                        CreateCondition(_propertyInfo.listProperty, t);
                        list.index = list.count - 1;
                    });
                }
                menu.ShowAsContext();
            };
            reorderableList.onRemoveCallback = list => {
                // 条件の削除
                DeleteCondition(_propertyInfo.listProperty, list.index);
                reorderableList.index--;
            };
            _propertyInfo.reorderableList = reorderableList;

            _propertyInfos.Add(property.propertyPath, _propertyInfo);
        }

        /// <summary>
        /// 条件の追加
        /// </summary>
        private Condition CreateCondition(SerializedProperty listProperty, Type type) {
            var serializedObject = listProperty.serializedObject;
            var targetObject = serializedObject.targetObject;
            Undo.RecordObject(targetObject, "CreateCondition");

            var condition = ScriptableObject.CreateInstance(type) as Condition;
            if (condition == null) {
                return null;
            }
            
            condition.name = type.Name;
            AssetDatabase.AddObjectToAsset(condition, targetObject);
            Undo.RegisterCreatedObjectUndo(condition, "CreateCondition");

            serializedObject.Update();
            listProperty.arraySize++;
            listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).objectReferenceValue = condition;
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
            return condition;
        }

        /// <summary>
        /// 条件の削除
        /// </summary>
        private void DeleteCondition(SerializedProperty listProperty, int index) {
            var serializedObject = listProperty.serializedObject;
            var targetObject = serializedObject.targetObject;
            Undo.RecordObject(targetObject, "DeleteCondition");

            serializedObject.Update();
            var condition = listProperty.GetArrayElementAtIndex(index).objectReferenceValue;
            listProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            if (condition != null) {
                AssetDatabase.RemoveObjectFromAsset(condition);
                Undo.DestroyObjectImmediate(condition);
            }

            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// RectにPaddingを加える
        /// </summary>
        private Rect AddPadding(Rect rect, float padding) {
            rect.xMin += padding;
            rect.xMax -= padding;
            rect.yMin += padding;
            rect.yMax -= padding;
            return rect;
        }
    }
}
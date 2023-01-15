using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.Experimental.GraphView;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// Node生成用のSearchWindowProvider
    /// </summary>
    public class CreateNodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider {
        private BehaviourTreeView _graphView;
        private EditorWindow _editorWindow;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(BehaviourTreeView graphView, EditorWindow editorWindow) {
            _graphView = graphView;
            _editorWindow = editorWindow;
        }

        /// <summary>
        /// 検索用のツリー構造構築
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {
            var entries = new List<SearchTreeEntry>();

            void AddTreeGroupEntry(string entriyName, int level = 0) {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(entriyName)) { level = level });
            }

            void AddTreeEntry(Type type, int level) {
                entries.Add(new SearchTreeEntry(new GUIContent(ObjectNames.NicifyVariableName(type.Name)))
                    { level = level, userData = type });
            }

            AddTreeGroupEntry("Create Node");

            var groupTypes = new List<Type> {
                typeof(CompositeNode),
                typeof(DecoratorNode),
                typeof(ActionNode),
                typeof(LinkNode),
            };

            foreach (var groupType in groupTypes) {
                var types = TypeCache.GetTypesDerivedFrom(groupType)
                    .Where(x => !x.IsAbstract && !x.IsGenericType)
                    .ToArray();
                if (types.Length <= 0) {
                    continue;
                }

                AddTreeGroupEntry(groupType.Name.Replace("Node", ""), 1);
                foreach (var type in types) {
                    AddTreeEntry(type, 2);
                }
            }

            return entries;
        }

        /// <summary>
        /// 要素選択時の処理
        /// </summary>
        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context) {
            var type = entry.userData as Type;
            var node = BehaviourTreeEditorUtility.CreateNode(_graphView.Data, type);

            var rootElement = _editorWindow.rootVisualElement;
            var worldMousePos = rootElement.ChangeCoordinatesTo(rootElement.parent,
                context.screenMousePosition - _editorWindow.position.position);
            var localMousePos = _graphView.contentViewContainer.WorldToLocal(worldMousePos);
            node.position = localMousePos;
            _graphView.CreateNodeView(node);

            AssetDatabase.SaveAssets();
            return true;
        }
    }
}
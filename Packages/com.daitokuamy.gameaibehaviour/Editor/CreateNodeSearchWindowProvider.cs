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

            void AddTreeGroupEntry(string entryName, int level = 0) {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(entryName)) { level = level });
            }

            void AddTreeEntry(Type type, int level) {
                var attr = type.GetCustomAttributes(typeof(BehaviourTreeNodeAttribute), false).FirstOrDefault() as BehaviourTreeNodeAttribute;
                var path = ObjectNames.NicifyVariableName(type.Name);
                if (attr != null) {
                    if (!string.IsNullOrEmpty(attr.Path)) {
                        path = attr.Path;
                    }
                }

                var splitPaths = path.Split("/");
                for (var i = 0; i < splitPaths.Length; i++) {
                    var splitPath = splitPaths[i];
                    var group = i < splitPaths.Length - 1;
                    var entry = default(SearchTreeEntry);
                    if (group) {
                        entry = new SearchTreeGroupEntry(new GUIContent(splitPath)) { level = level + i };
                    }
                    else {
                        entry = new SearchTreeEntry(new GUIContent(splitPath)) { level = level + i, userData = type };
                    }

                    entries.Add(entry);
                }
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

            AddTreeGroupEntry("Etc", 1);
            AddTreeEntry(typeof(FunctionRootNode), 2);

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
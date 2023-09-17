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
        private class TreeNode {
            public string name;
            public object userData;
            public List<TreeNode> children = new();

            public TreeNode(string name, object userData = null) {
                this.name = name;
                this.userData = userData;
            }
        }
        
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

            // Entryの追加
            void AddEntry(TreeNode node, int level = 0) {
                var group = node.children.Count > 0;
                if (group) {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(node.name)) { level = level, userData = node.userData});
                    foreach (var child in node.children) {
                        AddEntry(child, level + 1);
                    }
                }
                else {
                    entries.Add(new SearchTreeEntry(new GUIContent(node.name)) { level = level, userData = node.userData});
                }
            }
            
            AddEntry(CreateNodeTree());
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

        /// <summary>
        /// ノードツリーの作成
        /// </summary>
        private TreeNode CreateNodeTree() {
            // パスを解析した状態でのノード追加
            void AddNode(TreeNode parentNode, Type type) {
                var attr = type.GetCustomAttributes(typeof(BehaviourTreeNodeAttribute), false).FirstOrDefault() as BehaviourTreeNodeAttribute;
                var path = ObjectNames.NicifyVariableName(type.Name);
                if (attr != null) {
                    if (!string.IsNullOrEmpty(attr.Path)) {
                        path = attr.Path;
                    }
                }

                var splitPaths = path.Split("/");
                var currentNode = parentNode;
                for (var i = 0; i < splitPaths.Length; i++) {
                    var splitPath = splitPaths[i];
                    var group = i < splitPaths.Length - 1;
                    if (group) {
                        var foundNode = currentNode.children.Find(x => x.name == splitPath);
                        // 既に存在しているなら流用
                        if (foundNode != null) {
                            currentNode = foundNode;
                        }
                        // 無ければ生成して追加
                        else {
                            var nextNode = new TreeNode(splitPath);
                            currentNode.children.Add(nextNode);
                            currentNode = nextNode;
                        }
                    }
                    else {
                        var node = new TreeNode(splitPath, type);
                        currentNode.children.Add(node);
                    }
                }
            }
            
            var rootNode = new TreeNode("Create Node");
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

                var groupNode = new TreeNode(groupType.Name.Replace("Node", ""));
                rootNode.children.Add(groupNode);
                
                foreach (var type in types) {
                    AddNode(groupNode, type);
                }
            }

            var etcNode = new TreeNode("Etc");
            rootNode.children.Add(etcNode);
            AddNode(etcNode, typeof(FunctionNode));

            return rootNode;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// BehaviourTree用のエディタウィンドウ
    /// </summary>
    public partial class BehaviourTreeEditorWindow : EditorWindow {
        /// <summary>
        /// Node生成用のSearchWindowProvider
        /// </summary>
        private class CreateNodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider {
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
                };

                foreach (var groupType in groupTypes) {
                    var types = TypeCache.GetTypesDerivedFrom(groupType);
                    if (types.Count <= 0) {
                        continue;
                    }

                    AddTreeGroupEntry(groupType.Name.Replace("Node", ""), 1);
                    foreach (var type in types) {
                        AddTreeEntry(type, 2);
                    }
                }

                AddTreeGroupEntry("Etc", 1);
                AddTreeEntry(typeof(RootNode), 2);

                return entries;
            }

            /// <summary>
            /// 要素選択時の処理
            /// </summary>
            public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context) {
                var type = entry.userData as Type;
                var node = _graphView.Data.CreateNode(type);

                var rootElement = _editorWindow.rootVisualElement;
                var worldMousePos = rootElement.ChangeCoordinatesTo(rootElement.parent,
                    context.screenMousePosition - _editorWindow.position.position);
                var localMousePos = _graphView.contentViewContainer.WorldToLocal(worldMousePos);
                node.position = localMousePos;

                _graphView.CreateNodeView(node);
                return true;
            }
        }

        // UIElement
        private BehaviourTreeView _graphView;
        private InspectorView _inspectorView;

        // 制御対象のデータ
        private BehaviourTree _target;

        /// <summary>
        /// 開く処理
        /// </summary>
        public static void Open(BehaviourTree data) {
            if (data == null) {
                return;
            }

            var window =
                GetWindow<BehaviourTreeEditorWindow>(
                    ObjectNames.NicifyVariableName(nameof(BehaviourTreeEditorWindow)));
            window.Setup(data);
        }

        /// <summary>
        /// アセットを開いた際の処理
        /// </summary>
        [OnOpenAsset]
        private static bool OnOpen(int instanceId, int line) {
            var target = EditorUtility.InstanceIDToObject(instanceId);
            if (target is BehaviourTree data) {
                Open(data);
                return true;
            }

            return false;
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        private void CreateGUI() {
            // Xml, Style読み込み
            var root = rootVisualElement;
            var uxml = Resources.Load<VisualTreeAsset>("behaviour_tree_editor_window");
            uxml.CloneTree(root);
            var styleSheet = Resources.Load<StyleSheet>("behaviour_tree_editor_window");
            root.styleSheets.Add(styleSheet);

            _graphView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();

            _graphView.OnChangedSelectionNodeViews = nodeViews => {
                // 選択対象の更新
                _inspectorView.UpdateSelection(nodeViews.Select(x => x.Node).ToArray());
            };
            _inspectorView.OnChangedValue = targets => {
                // 編集時はNodeViewをリフレッシュ
                foreach (var target in targets) {
                    if (target is GameAiBehaviour.Node node) {
                        var nodeView = _graphView.GetNodeByGuid(node.guid) as NodeView;
                        nodeView.Refresh();
                    }
                }
            };

            // ノード生成処理
            var provider = CreateInstance<CreateNodeSearchWindowProvider>();
            provider.Initialize(_graphView, this);
            _graphView.nodeCreationRequest = context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };

            OnSelectionChange();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Setup(BehaviourTree data) {
            _target = data;
            _graphView.Load(_target);
        }

        /// <summary>
        /// 選択オブジェクトの変化通知
        /// </summary>
        private void OnSelectionChange() {
            var treeData = Selection.activeObject as BehaviourTree;

            if (treeData != null) {
                Setup(treeData);
            }
        }
    }

    /// <summary>
    /// GraphData用のエディタウィンドウ
    /// </summary>
    [CustomEditor(typeof(BehaviourTree))]
    public class GraphDataEditor : UnityEditor.Editor {
        /// <summary>
        /// インスペクタ拡張
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Window")) {
                BehaviourTreeEditorWindow.Open(target as BehaviourTree);
            }
        }
    }
}
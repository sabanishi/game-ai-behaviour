using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// BehaviourTree用のエディタウィンドウ
    /// </summary>
    public class BehaviourTreeEditorWindow : EditorWindow {
        // 制御対象のデータ
        [SerializeField]
        private BehaviourTree _target;

        // UIElement
        private BehaviourTreeView _graphView;
        private InspectorView _inspectorView;
        private BlackboardView _blackboardView;
        
        // BehaviourTreeControllerのオーナー
        private IBehaviourTreeControllerOwner _owner;

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
            _blackboardView = root.Q<BlackboardView>();

            _graphView.OnChangedSelectionNodeViews = nodeViews => {
                // 選択対象の更新
                _inspectorView.UpdateSelection(nodeViews.Select(x => x.Node).ToArray());
            };
            _inspectorView.OnChangedValue = targets => {
                // 編集時はNodeViewをリフレッシュ
                foreach (var target in targets) {
                    if (target is Node node) {
                        var nodeView = _graphView.GetNodeByGuid(node.guid) as NodeView;
                        nodeView?.Refresh();
                    }
                }
            };

            // ノード生成処理
            var provider = CreateInstance<CreateNodeSearchWindowProvider>();
            provider.Initialize(_graphView, this);
            _graphView.nodeCreationRequest = context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };

            Setup(_target);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Setup(BehaviourTree data) {
            var serializedObj = data != null ? new SerializedObject(data) : null;
            _target = data;
            _graphView.Load(_target);
            _blackboardView.SetBehaviourTreeObject(serializedObj);
            _inspectorView.SetBehaviourTree(_target);
        }

        /// <summary>
        /// 選択オブジェクトの変化通知
        /// </summary>
        private void OnSelectionChange() {
            var treeData = Selection.activeObject as BehaviourTree;

            if (treeData != null) {
                Setup(treeData);
            }

            var activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null) {
                var owner = activeGameObject.GetComponentInParent<IBehaviourTreeControllerOwner>();
                _owner = owner;
            }

            if (_owner != null) {
                _blackboardView.SetController(_owner.BehaviourTreeController);
            }
            else {
                _blackboardView.SetController(null);
            }
        }
    }
}
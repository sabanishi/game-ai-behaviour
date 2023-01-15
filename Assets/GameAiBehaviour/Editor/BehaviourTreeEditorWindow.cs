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
        private BehaviourTreeView _behaviourTreeView;
        private InspectorView _inspectorView;
        private BlackboardView _blackboardView;

        // BehaviourTreeControllerのオーナー
        private IBehaviourTreeControllerOwner _owner;
        // Playingモードの初期化フラグ
        private bool _playingInitFlag;

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

            _behaviourTreeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<BlackboardView>();

            _behaviourTreeView.OnChangedSelectionNodeViews = nodeViews => {
                // 選択対象の更新
                _inspectorView.UpdateSelection(nodeViews.Select(x => (Object)x.Node).ToArray());
            };
            _inspectorView.OnChangedValue = targets => {
                // 編集時はNodeViewをリフレッシュ
                foreach (var target in targets) {
                    if (target is Node node) {
                        var nodeView = _behaviourTreeView.GetNodeByGuid(node.guid) as NodeView;
                        nodeView?.Refresh();
                    }
                }
            };

            // ノード生成処理
            var provider = CreateInstance<CreateNodeSearchWindowProvider>();
            provider.Initialize(_behaviourTreeView, this);
            _behaviourTreeView.nodeCreationRequest = context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };

            Setup(_target);
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        private void OnEnable() {
            EditorApplication.playModeStateChanged += OnChangedPlayModeState;
            _playingInitFlag = false;
        }

        /// <summary>
        /// 非アクティブ時処理
        /// </summary>
        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnChangedPlayModeState;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update() {
            if (_behaviourTreeView == null) {
                return;
            }

            if (Application.isPlaying) {
                if (!_playingInitFlag) {
                    RefreshOwner();
                    _playingInitFlag = true;
                }
            }

            // Controllerの設定
            _blackboardView.SetController(_owner?.BehaviourTreeController);
            _behaviourTreeView.SetController(_owner?.BehaviourTreeController);

            // LogicStateの反映
            _behaviourTreeView.RefreshNodeLogicState();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Setup(BehaviourTree data) {
            var serializedObj = data != null ? new SerializedObject(data) : null;
            _target = data;
            _behaviourTreeView.Load(_target);
            _blackboardView.SetBehaviourTreeObject(serializedObj);
            _inspectorView.SetBehaviourTree(_target);
        }

        /// <summary>
        /// Ownerインスタンスのリフレッシュ
        /// </summary>
        private void RefreshOwner() {
            var activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null) {
                var owner = activeGameObject.GetComponentInParent<IBehaviourTreeControllerOwner>();
                _owner = owner;
            }
            else {
                _owner = null;
            }
        }

        /// <summary>
        /// 再生状態の変化通知
        /// </summary>
        private void OnChangedPlayModeState(PlayModeStateChange stateChange) {
            if (_behaviourTreeView == null) {
                return;
            }

            if (stateChange == PlayModeStateChange.ExitingPlayMode) {
                _owner = null;
                _blackboardView.SetController(null);
                _behaviourTreeView.SetController(null);
                _playingInitFlag = false;
            }
        }

        /// <summary>
        /// 選択オブジェクトの変化通知
        /// </summary>
        private void OnSelectionChange() {
            var treeData = Selection.activeObject as BehaviourTree;

            if (treeData != null) {
                Setup(treeData);
            }
            
            RefreshOwner();
        }
    }
}
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
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.daitokuamy.gameaibehaviour/Editor/Layouts/behaviour_tree_editor_window.uxml");
            uxml.CloneTree(root);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.daitokuamy.gameaibehaviour/Editor/Layouts/behaviour_tree_editor_window.uss");
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
            if (_target == null) {
                Setup(null);
            }

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
            CleanBehaviourTree(data);
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

            if (activeGameObject != null && Application.isPlaying) {
                _owner = activeGameObject.GetComponentInParent<IBehaviourTreeControllerOwner>();
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
            RefreshOwner();
        }

        /// <summary>
        /// BehaviourTreeの中身をクリーンする
        /// </summary>
        private void CleanBehaviourTree(BehaviourTree tree) {
            if (tree == null) {
                return;
            }

            var cleanAsset = false;
            var serializedObj = new SerializedObject(tree);
            serializedObj.Update();
            var nodesProp = serializedObj.FindProperty("nodes");
            for (var i = 0; i < nodesProp.arraySize; i++) {
                var trackProp = nodesProp.GetArrayElementAtIndex(i);
                if (trackProp.objectReferenceValue == null) {
                    nodesProp.DeleteArrayElementAtIndex(i);
                    i--;
                    cleanAsset = true;
                    continue;
                }

                var nodeProp = new SerializedObject(trackProp.objectReferenceValue);
                nodeProp.Update();
                var childrenProp = nodeProp.FindProperty("children");
                if (childrenProp != null) {
                    for (var j = 0; j < childrenProp.arraySize; j++) {
                        var prop = childrenProp.GetArrayElementAtIndex(j);
                        if (prop.objectReferenceValue == null) {
                            childrenProp.DeleteArrayElementAtIndex(j);
                            j--;
                            cleanAsset = true;
                        }
                    }
                }

                var childProp = nodeProp.FindProperty("child");
                if (childProp != null) {
                    if (childProp.objectReferenceValue == null) {
                        childProp.objectReferenceValue = null;
                    }
                }

                nodeProp.ApplyModifiedPropertiesWithoutUndo();
            }

            serializedObj.ApplyModifiedPropertiesWithoutUndo();
            
            // アセットに不備があった場合、SubAssetsをクリーンアップする
            if (cleanAsset) {
                BehaviourTreeEditorUtility.RemoveMissingSubAssets(tree);
            }
        }
    }
}
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// BehaviourTree用のエディタウィンドウ
    /// </summary>
    public class BehaviourTreeEditorWindow : EditorWindow {
        // 制御対象のデータ
        [SerializeField]
        private BehaviourTree _target;

        // UIElement
        private ObjectField _objectField;
        private BehaviourTreeView _behaviourTreeView;
        private InspectorView _inspectorView;
        private BlackboardView _blackboardView;

        // BehaviourTreeControllerのオーナー
        private IBehaviourTreeControllerProvider _provider;
        // Playingモードの初期化フラグ
        private bool _playingInitFlag;

        /// <summary>
        /// 開く処理
        /// </summary>
        public static void Open(BehaviourTree data) {
            if (data == null) {
                return;
            }

            var window = GetWindow<BehaviourTreeEditorWindow>(
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
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.daitokuamy.gameaibehaviour/Editor/Layouts/behaviour_tree_editor_window.uxml");
            uxml.CloneTree(root);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.daitokuamy.gameaibehaviour/Editor/Layouts/behaviour_tree_editor_window.uss");
            root.styleSheets.Add(styleSheet);

            _objectField = root.Q<ObjectField>();
            _behaviourTreeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<BlackboardView>();

            _objectField.value = _target;
            _objectField.RegisterValueChangedCallback(evt => {
                // 開いているTreeの変更
                if (_target != evt.newValue) {
                    Setup(evt.newValue as BehaviourTree);
                }
            });
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
            _behaviourTreeView.OnAlignmentSelectionNodeViews = (nodeViews, alignmentType) => {
                // 整列
                if (nodeViews.Length <= 0) {
                    return;
                }

                // 全体を取り込んだサイズの取得
                var mergedRect = nodeViews[0].GetPosition();
                for (var i = 1; i < nodeViews.Length; i++) {
                    var rect = nodeViews[i].GetPosition();
                    mergedRect.min = Vector2.Min(mergedRect.min, rect.min);
                    mergedRect.max = Vector2.Max(mergedRect.max, rect.max);
                }

                // 位置を補正
                for (var i = 0; i < nodeViews.Length; i++) {
                    var nodeView = nodeViews[i];
                    var rect = nodeView.GetPosition();
                    var offset = Vector2.zero;
                    switch (alignmentType) {
                        case BehaviourTreeView.AlignmentType.Top:
                            offset.y = mergedRect.yMin - rect.yMin;
                            break;
                        case BehaviourTreeView.AlignmentType.Bottom:
                            offset.y = mergedRect.yMax - rect.yMax;
                            break;
                        case BehaviourTreeView.AlignmentType.CenterH:
                            offset.y = mergedRect.center.y - rect.center.y;
                            break;
                        case BehaviourTreeView.AlignmentType.CenterV:
                            offset.x = mergedRect.center.x - rect.center.x;
                            break;
                        case BehaviourTreeView.AlignmentType.Left:
                            offset.x = mergedRect.xMin - rect.xMin;
                            break;
                        case BehaviourTreeView.AlignmentType.Right:
                            offset.x = mergedRect.xMax - rect.xMax;
                            break;
                    }

                    rect.position += offset;
                    nodeView.SetPosition(rect);
                }
            };
            _behaviourTreeView.OnConnectAllSelectionNodeViews = (connectNodeView, connectedNodeViews) => {
                // 一括接続
                foreach (var toNodeView in connectedNodeViews) {
                    _behaviourTreeView.ConnectNodeView(connectNodeView, toNodeView);
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
            _blackboardView.SetController(_provider?.BehaviourTreeController);
            _behaviourTreeView.SetController(_provider?.BehaviourTreeController);

            // LogicStateの反映
            _behaviourTreeView.RefreshNodeLogicState();
            
            // 描画の反映
            Repaint();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Setup(BehaviourTree data) {
            var serializedObj = data != null ? new SerializedObject(data) : null;
            CleanBehaviourTree(data);
            _target = data;
            _objectField.value = _target;
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
                _provider = activeGameObject.GetComponentInParent<IBehaviourTreeControllerProvider>();
            }
            else {
                _provider = null;
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
                _provider = null;
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
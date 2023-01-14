using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    using GraphNode = UnityEditor.Experimental.GraphView.Node;

    /// <summary>
    /// BehaviourTree表示用のGraphView
    /// </summary>
    public class BehaviourTreeView : GraphView {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> {
        }

        /// <summary>
        /// シリアライズ用のNode配列
        /// </summary>
        [Serializable]
        private class SerializableNodes {
            public Node[] nodes;
        }

        private BehaviourTreeController _behaviourTreeController;
        private List<NodeView> _tempNodeViews = new List<NodeView>();
        private List<NodeEdge> _tempNodeEdges = new List<NodeEdge>();

        public Action<NodeView[]> OnChangedSelectionNodeViews;
        public BehaviourTree Data { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeView() {
            // 背景設定
            var gridBackground = new GridBackground();
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();
            gridBackground.AddToClassList("grid-background");

            // 基本設定
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // 変更通知監視
            graphViewChanged += OnGraphViewChanged;

            // コピー
            serializeGraphElements += OnSerializeGraphElements;
            // ペースト
            unserializeAndPaste += OnUnserializeAndPaste;

            // Undo通知
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        /// <summary>
        /// 互換性ポート取得
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            var compatiblePorts = new List<Port>();

            compatiblePorts.AddRange(ports.ToList().Where(port => {
                if (port.node == startPort.node) {
                    return false;
                }

                if (port.direction == startPort.direction) {
                    return false;
                }

                if (port.portType != startPort.portType) {
                    return false;
                }

                return true;
            }));

            return compatiblePorts;
        }

        /// <summary>
        /// データの読み込み
        /// </summary>
        public void Load(BehaviourTree data) {
            // 既存のデータをクリア
            Data = null;
            DeleteElements(graphElements.ToList());

            Data = data;

            if (Data == null) {
                return;
            }

            // ノード生成
            foreach (var node in data.nodes) {
                CreateNodeView(node);
            }

            // エッジの追加
            foreach (var node in data.nodes) {
                if (node is CompositeNode composite) {
                    foreach (var child in composite.children) {
                        CreateEdge(node, child);
                    }
                }
                else if (node is DecoratorNode decorator) {
                    CreateEdge(node, decorator.child);
                }
                else if (node is RootNode root) {
                    CreateEdge(node, root.child);
                }
            }
        }

        /// <summary>
        /// NodeViewの生成
        /// </summary>
        public void CreateNodeView(Node node) {
            if (node == null) {
                return;
            }

            var nodeView = new NodeView(node);
            AddElement(nodeView);
        }

        /// <summary>
        /// BehaviourTreeControllerの参照を設定
        /// </summary>
        public void SetController(BehaviourTreeController behaviourTreeController) {
            _behaviourTreeController = behaviourTreeController;

            // Nodeの状態をリセット
            foreach (var node in nodes) {
                if (!(node is NodeView nodeView)) {
                    continue;
                }

                nodeView.SetNodeState(NodeView.State.Default);
            }
        }

        /// <summary>
        /// NodeLogicの状態を更新
        /// </summary>
        public void RefreshNodeLogicState() {
            if (_behaviourTreeController == null) {
                return;
            }
            
            // Node/Edgeの取得
            _tempNodeViews.Clear();
            _tempNodeViews.AddRange(nodes.OfType<NodeView>());
            _tempNodeEdges.Clear();
            _tempNodeEdges.AddRange(_tempNodeViews
                .SelectMany(x => x.Output?.connections ?? Array.Empty<Edge>())
                .OfType<NodeEdge>());
            
            // 各種状態のリセット
            foreach (var nodeView in _tempNodeViews) {
                nodeView.SetNodeState(NodeView.State.Default);
            }

            foreach (var nodeEdge in _tempNodeEdges) {
                nodeEdge.IsRan = false;
            }
            
            // ノードの状態を設定
            void SetNodeState(NodeView view, Node.ILogic nodeLogic) {
                if (nodeLogic.IsRunning) {
                    view.SetNodeState(NodeView.State.Running);
                }
                else {
                    switch (nodeLogic.State) {
                        case Node.State.Success:
                            view.SetNodeState(NodeView.State.Success);
                            break;
                        case Node.State.Failure:
                            view.SetNodeState(NodeView.State.Failure);
                            break;
                        default:
                            view.SetNodeState(NodeView.State.Default);
                            break;
                    }
                }
            }
            
            // 実行履歴の反映
            foreach (var path in _behaviourTreeController.ExecutedPaths) {
                var prevView = _tempNodeViews.FirstOrDefault(x => x.Node == path.prev.TargetNode);
                var nextView = _tempNodeViews.FirstOrDefault(x => x.Node == path.next.TargetNode);
                var edge = _tempNodeEdges.FirstOrDefault(x =>
                    ((NodeView)x.input.node).Node == path.next.TargetNode &&
                    ((NodeView)x.output.node).Node == path.prev.TargetNode);
                
                if (prevView != null) {
                    SetNodeState(prevView, path.prev);
                }
                
                if (nextView != null) {
                    SetNodeState(nextView, path.next);
                }

                if (edge != null) {
                    edge.IsRan = true;
                }
            }
        }

        /// <summary>
        /// 選択状態の追加
        /// </summary>
        public override void AddToSelection(ISelectable selectable) {
            base.AddToSelection(selectable);
            OnChangedSelectionNodeViews?.Invoke(selection.OfType<NodeView>().ToArray());
        }

        /// <summary>
        /// 選択状態の削除
        /// </summary>
        public override void RemoveFromSelection(ISelectable selectable) {
            base.RemoveFromSelection(selectable);
            OnChangedSelectionNodeViews?.Invoke(selection.OfType<NodeView>().ToArray());
        }

        /// <summary>
        /// 選択状態のクリア
        /// </summary>
        public override void ClearSelection() {
            base.ClearSelection();
            OnChangedSelectionNodeViews?.Invoke(new NodeView[0]);
        }

        /// <summary>
        /// Edgeの生成
        /// </summary>
        private void CreateEdge(Node parent, Node child) {
            if (parent == null || child == null) {
                return;
            }

            var parentElement = GetElementByGuid(parent.guid) as NodeView;
            var childElement = GetElementByGuid(child.guid) as NodeView;
            if (parentElement != null && childElement != null) {
                var edge = parentElement.Output.ConnectTo<NodeEdge>(childElement.Input);
                AddElement(edge);
            }
        }

        /// <summary>
        /// グラフ変更通知
        /// </summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            if (Data == null) {
                return graphViewChange;
            }

            if (graphViewChange.elementsToRemove != null) {
                graphViewChange.elementsToRemove.ForEach(element => {
                    if (element is NodeView nodeView) {
                        BehaviourTreeEditorUtility.DeleteNode(Data, nodeView.Node);
                    }
                    else if (element is Edge edge) {
                        var parentView = edge.output.node as NodeView;
                        var childView = edge.input.node as NodeView;
                        BehaviourTreeEditorUtility.DisconnectNode(parentView?.Node, childView?.Node);
                    }
                });
                AssetDatabase.SaveAssets();
            }

            if (graphViewChange.edgesToCreate != null) {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    BehaviourTreeEditorUtility.ConnectNode(parentView?.Node, childView?.Node);
                    BehaviourTreeEditorUtility.SortChildren(parentView?.Node);
                });
                AssetDatabase.SaveAssets();
            }

            if (graphViewChange.movedElements != null) {
                graphViewChange.movedElements.ForEach(element => {
                    if (element is NodeView nodeView) {
                        var input = nodeView.Input;
                        if (input != null) {
                            // 接続されている元のNodeに対してソートを行う
                            foreach (var edge in input.connections) {
                                var parentView = edge.output.node as NodeView;
                                BehaviourTreeEditorUtility.SortChildren(parentView?.Node);
                            }
                        }
                    }
                });
                AssetDatabase.SaveAssets();
            }

            return graphViewChange;
        }

        /// <summary>
        /// コピー処理
        /// </summary>
        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements) {
            var nodeViews = elements.OfType<NodeView>().ToArray();
            if (nodeViews.Length <= 0 || Data == null) {
                return "";
            }

            // Node情報をシリアライズ化
            var serializableNodes = new SerializableNodes();
            serializableNodes.nodes = nodeViews.Select(x => x.Node).ToArray();
            return JsonUtility.ToJson(serializableNodes);
        }

        /// <summary>
        /// ペースト処理
        /// </summary>
        private void OnUnserializeAndPaste(string operationName, string data) {
            if (string.IsNullOrEmpty(data) || Data == null) {
                return;
            }

            // Node情報をデシリアライズ
            var serializableNodes = JsonUtility.FromJson<SerializableNodes>(data);
            var newNodes = BehaviourTreeEditorUtility.DuplicateNodes(Data, serializableNodes.nodes)
                .ToList();

            // ツリー情報を再構築
            Load(Data);
            AssetDatabase.SaveAssets();

            // 新しいNodeを選択状態にする
            var newNodeViews = nodes.OfType<NodeView>()
                .Where(x => newNodes.Contains(x.Node))
                .ToArray();
            selection.Clear();
            foreach (var view in newNodeViews) {
                view.Select(this, true);
            }
        }

        /// <summary>
        /// Undo/Redo通知
        /// </summary>
        private void OnUndoRedo() {
            Load(Data);
            AssetDatabase.SaveAssets();
        }
    }
}
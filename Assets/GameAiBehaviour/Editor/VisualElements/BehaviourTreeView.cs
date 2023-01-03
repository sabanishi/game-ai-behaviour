using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    using GraphNode = UnityEditor.Experimental.GraphView.Node;

    /// <summary>
    /// BehaviourTree表示用のGraphView
    /// </summary>
    public class BehaviourTreeView : GraphView {
        public Action<NodeView[]> OnChangedSelectionNodeViews;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> {
        }

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

            // サイズ調整
            this.StretchToParentSize();
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
            var edge = parentElement.Output.ConnectTo(childElement.Input);
            AddElement(edge);
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
                        Data.DeleteNode(nodeView.Node);
                    }
                    else if (element is Edge edge) {
                        var parent = edge.output.node as NodeView;
                        var child = edge.input.node as NodeView;
                        Data.DisconnectNode(parent.Node, child.Node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null) {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    var parent = edge.output.node as NodeView;
                    var child = edge.input.node as NodeView;
                    Data.ConnectNode(parent.Node, child.Node);
                    Data.SortChildren(parent.Node);
                });
            }

            if (graphViewChange.movedElements != null) {
                graphViewChange.movedElements.ForEach(element => {
                    if (element is NodeView nodeView) {
                        var input = nodeView.Input;
                        if (input != null) {
                            // 接続されている元のNodeに対してソートを行う
                            foreach (var edge in input.connections) {
                                var parent = edge.output.node as NodeView;
                                if (parent != null) {
                                    Data.SortChildren(parent.Node);
                                }
                            }
                        }
                    }
                });
            }

            return graphViewChange;
        }
    }
}
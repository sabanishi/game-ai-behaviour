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
            foreach (var node in serializableNodes.nodes) {
                BehaviourTreeEditorUtility.DuplicateNode(Data, node);
            }
            
            // ツリー情報を再構築
            Load(Data);
            AssetDatabase.SaveAssets();
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
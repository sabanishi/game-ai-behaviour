using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAiBehaviour;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace GameAiBehaviour.Editor {
    using GraphNode = UnityEditor.Experimental.GraphView.Node;
    using Node = GameAiBehaviour.Node;

    /// <summary>
    /// エディタ用のノード
    /// </summary>
    public class NodeView : GraphNode {
        public Node Node { get; private set; }
        public Port Input { get; private set; }
        public Port Output { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NodeView(Node node)
//            : base(BehaviourTreeDataEditorWindow.GetEditorFilePath($"{nameof(NodeView)}.uxml"))
        {
            title = node.DisplayName;
            Node = node;
            viewDataKey = node.guid;

            // ポート作成
            CreateInputPort(node);
            CreateOutputPort(node);

            // 座標設定
            style.left = Node.position.x;
            style.top = Node.position.y;
        }

        /// <summary>
        /// 値更新時などのリフレッシュ
        /// </summary>
        public void Refresh() {
            title = Node.DisplayName;
        }

        /// <summary>
        /// 座標の設定
        /// </summary>
        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "SetPosition");
            Node.position = newPos.position;
            EditorUtility.SetDirty(Node);
        }

        /// <summary>
        /// InputPort生成
        /// </summary>
        private void CreateInputPort(Node node) {
            var port = default(Port);

            if (node is CompositeNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is DecoratorNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is ActionNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (port != null) {
                port.portName = "";
                inputContainer.Add(port);
            }

            Input = port;
        }

        /// <summary>
        /// OutputPort生成
        /// </summary>
        private void CreateOutputPort(Node node) {
            var port = default(Port);

            if (node is CompositeNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is DecoratorNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (node is RootNode) {
                port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (port != null) {
                port.portName = "";
                outputContainer.Add(port);
            }

            Output = port;
        }
    }
}
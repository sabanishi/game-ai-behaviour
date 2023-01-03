using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace GameAiBehaviour.Editor {
    using GraphNode = UnityEditor.Experimental.GraphView.Node;

    /// <summary>
    /// エディタ用のノード
    /// </summary>
    public class NodeView : GraphNode {
        public new class UxmlFactory : UxmlFactory<NodeView, UxmlTraits> {
        }
        
        public Node Node { get; private set; }
        public Port Input { get; private set; }
        public Port Output { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NodeView(Node node) 
            : base("Assets/GameAiBehaviour/Editor/Resources/node_view.uxml") {
            var styleSheet = Resources.Load<StyleSheet>("node_view");
            styleSheets.Add(styleSheet);
            
            title = node.DisplayName;
            Node = node;
            viewDataKey = node.guid;

            // 座標設定
            style.left = Node.position.x;
            style.top = Node.position.y;

            // ポート作成
            CreateInputPort(node);
            CreateOutputPort(node);
            
            // Style適用
            SetupClasses(node);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NodeView() {
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
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is DecoratorNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is ActionNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (port != null) {
                port.portName = "";
                port.style.flexDirection = FlexDirection.Column;
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
                port = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is DecoratorNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (node is RootNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (port != null) {
                port.portName = "";
                port.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(port);
            }

            Output = port;
        }

        /// <summary>
        /// スタイルクラスの初期化
        /// </summary>
        private void SetupClasses(Node node) {
            if (node is CompositeNode) {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode) {
                AddToClassList("decorator");
            }
            else if (node is ActionNode) {
                AddToClassList("action");
            }
            else if (node is RootNode) {
                AddToClassList("root");
            }
        }
    }
}
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
        private Label _descriptionLabel;
        
        public sealed override string title {
            get => base.title;
            set {
                if (string.IsNullOrEmpty(value)) {
                    base.title = ObjectNames.NicifyVariableName(Node.GetType().Name);
                }
                else {
                    base.title = value;
                }
            }
        }
        
        public string Description {
            get {
                if (_descriptionLabel.style.display == DisplayStyle.None) {
                    return "";
                }

                return _descriptionLabel.text;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    _descriptionLabel.text = "";
                    _descriptionLabel.style.display = DisplayStyle.None;
                }
                else {
                    _descriptionLabel.style.display = DisplayStyle.Flex;
                    _descriptionLabel.text = value;
                }
            }
        }
        public Node Node { get; private set; }
        public Port Input { get; private set; }
        public Port Output { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NodeView(Node node) 
            : base("Assets/GameAiBehaviour/Editor/Resources/node_view.uxml") {
            // パーツ取得
            _descriptionLabel = this.Q<Label>("description");
            
            Node = node;
            viewDataKey = node.guid;
            title = node.title;
            Description = node.Description;

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
            title = Node.title;
            Description = Node.Description;
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
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is DecoratorNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is ActionNode) {
                port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }

            if (port != null) {
                port.portName = "";
                port.style.flexDirection = FlexDirection.Column;
                port.AddToClassList("node_port");
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
                port.AddToClassList("node_port");
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
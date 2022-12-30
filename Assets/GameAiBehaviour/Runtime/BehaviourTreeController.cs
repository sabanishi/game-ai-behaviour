using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御クラス
    /// </summary>
    public class BehaviourTreeController {
        // オーナー
        private object _owner;
        // 実行データ
        private BehaviourTree _data;
        // 実行状態
        private Node.State _state = Node.State.Success;
        // Rootノード
        private RootNode _rootNode;
        // ノードロジック
        private Dictionary<Node, Node.ILogic> _logics = new Dictionary<Node, Node.ILogic>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeController(object owner) {
            _owner = owner;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Setup(BehaviourTree data) {
            Cleanup();

            _data = data;

            if (_data == null) {
                return;
            }

            _rootNode = _data.nodes.OfType<RootNode>().FirstOrDefault();
            _logics = _data.nodes.ToDictionary(x => x, x => x.CreateLogic(this));
            foreach (var pair in _logics) {
                pair.Value.Initialize();
            }
        }

        /// <summary>
        /// 思考リセット
        /// </summary>
        public void ResetThink() {
            foreach (var logic in _logics) {
                logic.Value.Cancel();
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Cleanup() {
            _data = null;
            _rootNode = null;
            foreach (var pair in _logics) {
                pair.Value.Dispose();
            }

            _logics.Clear();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update(float deltaTime) {
            if (_data == null || _rootNode == null) {
                return;
            }

            // ノードの実行
            _state = UpdateNode(_rootNode, deltaTime);
        }

        /// <summary>
        /// ノード用のロジック取得
        /// </summary>
        public Node.ILogic GetLogic(Node node) {
            if (_logics.TryGetValue(node, out var logic)) {
                return logic;
            }

            Debug.LogError($"Not found node logic. {node.GetType()}");
            return null;
        }

        /// <summary>
        /// ノードの実行
        /// </summary>
        public Node.State UpdateNode(Node node, float deltaTime) {
            var logic = GetLogic(node);
            if (logic == null) {
                return Node.State.Failure;
            }

            return logic.Update(deltaTime);
        }
    }
}
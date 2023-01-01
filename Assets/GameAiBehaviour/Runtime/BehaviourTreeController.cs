using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御クラス
    /// </summary>
    public class BehaviourTreeController : IBehaviourTreeController {
        // 実行データ
        private BehaviourTree _data;
        // 実行状態
        private Node.State _state = Node.State.Success;
        // Rootノード
        private RootNode _rootNode;
        // 実行中ノードスタック
        private List<Node> _runningNodes = new List<Node>();
        // ノードロジック
        private Dictionary<Node, Node.ILogic> _logics = new Dictionary<Node, Node.ILogic>();
        // アクションノードハンドラ
        private readonly Dictionary<Type, IActionNodeHandler> _actionNodeHandlers = new Dictionary<Type, IActionNodeHandler>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeController() {
        }

        /// <summary>
        /// ActionNodeHandlerのBind
        /// </summary>
        /// <param name="onInit">Handlerの初期化関数</param>
        public void BindActionNodeHandler<TNode, TNodeHandler>(Action<TNodeHandler> onInit)
            where TNode : ActionNode
            where TNodeHandler : ActionNodeHandler<TNode>, new() {
            var handler = new TNodeHandler();
            onInit?.Invoke(handler);
            _actionNodeHandlers[typeof(TNode)] = handler;
        }

        /// <summary>
        /// ActionNodeHandlerのBindを解除
        /// </summary>
        public void ResetActionNodeHandler<TNode>()
            where TNode : ActionNode {
            _actionNodeHandlers.Remove(typeof(TNode));
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
            _runningNodes.Clear();
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

            _runningNodes.Clear();
            _logics.Clear();
            _actionNodeHandlers.Clear();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update(float deltaTime) {
            if (_data == null || _rootNode == null) {
                return;
            }

            // スタックの更新
            void UpdateStack(bool back) {
                if (_runningNodes.Count <= 0) {
                    return;
                }
                
                // ノードの実行
                var lastIndex = _runningNodes.Count - 1;
                var lastNode = _runningNodes[lastIndex];
                _runningNodes.RemoveAt(lastIndex);
                _state = ((IBehaviourTreeController)this).UpdateNode(lastNode, deltaTime, back);

                if (_state != Node.State.Running) {
                    // 再起的実行
                    UpdateStack(true);
                }
            }
            
            // 実行中ノードがあれば実行
            if (_runningNodes.Count > 0) {
                UpdateStack(false);
            }
            else {
                // ノードの実行
                _state = ((IBehaviourTreeController)this).UpdateNode(_rootNode, deltaTime, false);
            }
            
            // ステータスが実行中でなければ、実行中スタックをクリア
            if (_state != Node.State.Running) {
                _runningNodes.Clear();
            }
        }

        /// <summary>
        /// ノードの実行
        /// </summary>
        Node.State IBehaviourTreeController.UpdateNode(Node node, float deltaTime, bool back) {
            var logic = GetLogic(node);
            if (logic == null) {
                return Node.State.Failure;
            }

            _runningNodes.Add(node);
            var state = logic.Update(deltaTime, back);
            if (state != Node.State.Running) {
                _runningNodes.Remove(node);
            }
            
            return state;
        }

        /// <summary>
        /// ActionNodeのハンドリング用インスタンスを取得
        /// </summary>
        IActionNodeHandler IBehaviourTreeController.GetActionHandler(ActionNode node) {
            if (_actionNodeHandlers.TryGetValue(node.GetType(), out var handler)) {
                return handler;
            }

            return null;
        }

        /// <summary>
        /// ノード用のロジック取得
        /// </summary>
        private Node.ILogic GetLogic(Node node) {
            if (_logics.TryGetValue(node, out var logic)) {
                return logic;
            }

            Debug.LogError($"Not found node logic. {node.GetType()}");
            return null;
        }
    }
}
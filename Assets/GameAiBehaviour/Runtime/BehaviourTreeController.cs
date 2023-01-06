using System;
using System.Collections.Generic;
using System.Linq;

//using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御クラス
    /// </summary>
    public class BehaviourTreeController : IBehaviourTreeController {
        /// <summary>
        /// ActionHandler情報
        /// </summary>
        private class ActionHandlerInfo {
            public Type Type;
            public Action<object> InitAction;
        }

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
        // アクションハンドラ情報
        private Dictionary<Type, ActionHandlerInfo> _actionHandlerInfos = new Dictionary<Type, ActionHandlerInfo>();
        // アクションノードハンドラ
        private readonly Dictionary<Node, IActionNodeHandler> _actionNodeHandlers =
            new Dictionary<Node, IActionNodeHandler>();

        /// <summary>
        /// Blackboard
        /// </summary>
        public Blackboard Blackboard { get; private set; } = new Blackboard();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeController() {
        }

        /// <summary>
        /// ActionNodeHandlerのBind
        /// </summary>
        /// <param name="onInit">Handlerの初期化関数</param>
        public void BindActionNodeHandler<TNode, THandler>(Action<THandler> onInit)
            where TNode : HandleableActionNode
            where THandler : ActionNodeHandler<TNode>, new() {
            ResetActionNodeHandler<TNode>();

            _actionHandlerInfos[typeof(TNode)] = new ActionHandlerInfo {
                Type = typeof(THandler),
                InitAction = onInit != null ? obj => { onInit.Invoke(obj as THandler); } : null
            };
        }

        /// <summary>
        /// ActionNodeHandlerのBind
        /// </summary>
        /// <param name="updateFunc">更新関数</param>
        /// <param name="enterAction">開始関数</param>
        /// <param name="exitAction">終了関数</param>
        public void BindActionNodeHandler<TNode>(Func<TNode, float, Node.State> updateFunc,
            Action<TNode> enterAction = null, Action<TNode> exitAction = null)
            where TNode : HandleableActionNode {
            BindActionNodeHandler<TNode, ObserveActionNodeHandler<TNode>>(handler => {
                handler.SetEnterAction(enterAction);
                handler.SetUpdateFunc(updateFunc);
                handler.SetExitAction(exitAction);
            });
        }

        /// <summary>
        /// ActionNodeHandlerのBindを解除
        /// </summary>
        public void ResetActionNodeHandler<TNode>()
            where TNode : HandleableActionNode {
            _actionHandlerInfos.Remove(typeof(TNode));

            // 既に登録済のHandlerがあった場合は削除する
            var removeKeys = _actionNodeHandlers.Keys
                .Where(x => x.GetType() == typeof(TNode))
                .ToArray();
            foreach (var removeKey in removeKeys) {
                _actionNodeHandlers.Remove(removeKey);
            }
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

            // Blackboard初期化
            foreach (var property in data.properties) {
                switch (property.propertyType) {
                    case Property.Type.Integer:
                        Blackboard.SetInteger(property.propertyName, property.integerValue);
                        break;
                    case Property.Type.Float:
                        Blackboard.SetFloat(property.propertyName, property.floatValue);
                        break;
                    case Property.Type.String:
                        Blackboard.SetString(property.propertyName, property.stringValue);
                        break;
                    case Property.Type.Boolean:
                        Blackboard.SetBoolean(property.propertyName, property.booleanValue);
                        break;
                }
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
            Blackboard.Clear();

            _data = null;
            _rootNode = null;
            foreach (var pair in _logics) {
                pair.Value.Dispose();
            }

            _runningNodes.Clear();
            _logics.Clear();
            _actionHandlerInfos.Clear();
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
        IActionNodeHandler IBehaviourTreeController.GetActionHandler(HandleableActionNode node) {
            if (_actionNodeHandlers.TryGetValue(node, out var handler)) {
                return handler;
            }

            // 無ければ生成
            if (_actionHandlerInfos.TryGetValue(node.GetType(), out var handlerInfo)) {
                var constructorInfo = handlerInfo.Type.GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null) {
                    handler = (IActionNodeHandler)constructorInfo.Invoke(Array.Empty<object>());
                    _actionNodeHandlers[node] = handler;
                    handlerInfo.InitAction?.Invoke(handler);
                }
            }

            return handler;
        }

        /// <summary>
        /// ノード用のロジック取得
        /// </summary>
        private Node.ILogic GetLogic(Node node) {
            if (_logics.TryGetValue(node, out var logic)) {
                return logic;
            }

            return null;
        }
    }
}
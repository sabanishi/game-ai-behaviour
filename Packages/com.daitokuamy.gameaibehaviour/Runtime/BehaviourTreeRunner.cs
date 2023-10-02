using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree実行用クラス
    /// </summary>
    public class BehaviourTreeRunner : IBehaviourTreeRunner {
        /// <summary>
        /// 実行パス
        /// </summary>
        public struct Path {
            public Node.ILogic PrevNodeLogic;
            public Node.ILogic NextNodeLogic;
        }

        // 開始ノード
        private Node _startNode;
        // ノードロジックのプールリスト
        private readonly Dictionary<Node, List<Node.ILogic>> _logicPools = new Dictionary<Node, List<Node.ILogic>>();
        // 実行用ルーチン
        private NodeLogicRoutine _nodeLogicRoutine;
        // 実行中StartNodeLogic
        private Node.ILogic _startNodeLogic;
        // 実行履歴パス
        private readonly List<Path> _executedPaths = new List<Path>();
        // 思考時間
        private float _thinkTime;
        // アクションノードハンドラ
        private readonly Dictionary<Node, IActionNodeHandler> _actionNodeHandlers = new();

        // 現在の実行状態
        public Node.State CurrentState => _startNodeLogic?.State ?? Node.State.Inactive;
        // 実行中か
        public bool IsRunning => _nodeLogicRoutine != null;
        // 実行履歴のパス
        public IReadOnlyList<Path> ExecutedPaths => _executedPaths;
        // 制御用コントローラ
        public IBehaviourTreeController Controller { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeRunner(IBehaviourTreeController controller, Node startNode) {
            Controller = controller;
            _startNode = startNode;
        }

        /// <summary>
        /// 思考リセット
        /// </summary>
        public void ResetThink() {
            foreach (var logics in _logicPools.Values) {
                foreach (var logic in logics) {
                    logic.Cancel();
                }
            }

            _nodeLogicRoutine = null;
            _startNodeLogic = null;
            _executedPaths.Clear();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Cleanup() {
            ResetThink();

            // Handlerを解放
            RemoveActionNodeHandlers();

            _startNode = null;
            foreach (var logics in _logicPools.Values) {
                foreach (var logic in logics) {
                    logic.Dispose();
                }
            }

            _logicPools.Clear();
        }

        /// <summary>
        /// Tick
        /// </summary>
        public Node.State Tick(Action onReset = null) {
            if (_startNode == null) {
                return Node.State.Failure;
            }

            void UpdateRoutine() {
                if (!_nodeLogicRoutine.MoveNext()) {
                    _nodeLogicRoutine = null;
                }
            }

            if (_nodeLogicRoutine != null) {
                // ルーチン実行
                UpdateRoutine();
            }
            else {
                // 実行状態をリセット
                ResetThink();

                // リセット処理通知
                onReset?.Invoke();

                // 起点NodeのRoutineを生成
                var startNodeLogic = ((IBehaviourTreeRunner)this).GetLogic(_startNode);
                _nodeLogicRoutine = new NodeLogicRoutine(startNodeLogic.ExecuteRoutine());
                _startNodeLogic = startNodeLogic;

                // ルーチン実行
                UpdateRoutine();
            }

            return CurrentState;
        }

        /// <summary>
        /// ノード用ロジックを取得(無ければ生成)
        /// </summary>
        Node.ILogic IBehaviourTreeRunner.GetLogic(Node node) {
            if (node == null) {
                return null;
            }

            // Poolを探す
            if (!_logicPools.TryGetValue(node, out var logics)) {
                // 無ければ追加
                logics = new List<Node.ILogic>();
                _logicPools[node] = logics;
            }

            // 未使用のLogicを探す
            foreach (var logic in logics) {
                if (logic.State != Node.State.Inactive) {
                    continue;
                }

                return logic;
            }

            // なかった場合は追加して作成
            var newLogic = node.CreateLogic(this);
            newLogic.Initialize();
            logics.Add(newLogic);
            return newLogic;
        }

        /// <summary>
        /// ActionNodeHandlerの取得
        /// </summary>
        IActionNodeHandler IBehaviourTreeRunner.GetActionNodeHandler(HandleableActionNode node) {
            if (node == null) {
                return null;
            }

            if (_actionNodeHandlers.TryGetValue(node, out var handler)) {
                return handler;
            }

            // 無ければ生成
            handler = Controller.CreateActionNodeHandler(node);
            if (handler != null) {
                _actionNodeHandlers[node] = handler;
            }

            return handler;
        }

        /// <summary>
        /// LinkNodeHandlerの取得
        /// </summary>
        ILinkNodeHandler IBehaviourTreeRunner.GetLinkNodeHandler(HandleableLinkNode node) {
            return Controller.GetLinkNodeHandler(node);
        }

        /// <summary>
        /// 実行パスの追加
        /// </summary>
        void IBehaviourTreeRunner.AddExecutedPath(Node.ILogic prev, Node.ILogic next) {
            _executedPaths.Add(new Path { PrevNodeLogic = prev, NextNodeLogic = next });
        }

        /// <summary>
        /// ActionNodeHandlerを削除する
        /// </summary>
        internal void RemoveActionNodeHandlers(Type nodeType) {
            var removeKeys = _actionNodeHandlers.Keys
                .Where(x => x.GetType() == nodeType)
                .ToArray();
            var runner = (IBehaviourTreeRunner)this;

            foreach (var removeKey in removeKeys) {
                // 登録解除時に事前にキャンセルを呼び出し
                if (removeKey is HandleableActionNode actionNode) {
                    var logic = runner.GetLogic(actionNode);
                    if (logic != null) {
                        if (logic.State != Node.State.Inactive) {
                            _actionNodeHandlers[removeKey].OnCancel(actionNode);
                        }
                    }
                }

                _actionNodeHandlers.Remove(removeKey);
            }
        }

        /// <summary>
        /// ActionNodeHandlerを削除する
        /// </summary>
        internal void RemoveActionNodeHandlers() {
            var runner = (IBehaviourTreeRunner)this;
            foreach (var pair in _actionNodeHandlers) {
                // 登録解除時に事前にキャンセルを呼び出し
                if (pair.Key is HandleableActionNode actionNode) {
                    var logic = runner.GetLogic(actionNode);
                    if (logic != null) {
                        if (logic.State != Node.State.Inactive) {
                            _actionNodeHandlers[pair.Key].OnCancel(actionNode);
                        }
                    }
                }
            }

            _actionNodeHandlers.Clear();
        }
    }
}
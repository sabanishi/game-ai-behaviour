using System;
using System.Collections.Generic;

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
        // 実行履歴パス
        private readonly List<Path> _executedPaths = new List<Path>();
        // 思考時間
        private float _thinkTime;

        // 現在の実行状態
        public Node.State CurrentState => _nodeLogicRoutine?.Current?.State ?? Node.State.Inactive;
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
            _executedPaths.Clear();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Cleanup() {
            ResetThink();

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
        public void Tick(Action onReset = null) {
            if (_startNode == null) {
                return;
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
                var startLogic = ((IBehaviourTreeRunner)this).FindLogic(_startNode);
                _nodeLogicRoutine = new NodeLogicRoutine(startLogic.ExecuteRoutine());

                // ルーチン実行
                UpdateRoutine();
            }
        }

        /// <summary>
        /// ノード用ロジックを検索
        /// </summary>
        Node.ILogic IBehaviourTreeRunner.FindLogic(Node node) {
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
        /// 実行パスの追加
        /// </summary>
        void IBehaviourTreeRunner.AddExecutedPath(Node.ILogic prev, Node.ILogic next) {
            _executedPaths.Add(new Path { PrevNodeLogic = prev, NextNodeLogic = next });
        }
    }
}
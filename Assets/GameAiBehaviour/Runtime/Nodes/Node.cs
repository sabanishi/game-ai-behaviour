using System;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// Graphデータ本体
    /// </summary>
    public abstract class Node : ScriptableObject {
        // 状態
        public enum State {
            Inactive,
            Failure,
            Running,
            Success,
        }

        /// <summary>
        /// ノード処理実行用インターフェース
        /// </summary>
        public interface ILogic : IDisposable {
            /// <summary>
            /// 制御対象のNode
            /// </summary>
            Node TargetNode { get; }
            
            /// <summary>
            /// 現在の状態
            /// </summary>
            State State { get; }
            
            /// <summary>
            /// 初期化処理
            /// </summary>
            void Initialize();

            /// <summary>
            /// 実行処理
            /// </summary>
            void Update(ILogic parentNodeLogic);

            /// <summary>
            /// Running状態の実行処理
            /// </summary>
            void UpdateRunning(ILogic childNodeLogic);

            /// <summary>
            /// 子のNode実行結果を送る
            /// </summary>
            void SendChildResult(ILogic childNodeLogic);

            /// <summary>
            /// キャンセル処理
            /// </summary>
            void Reset();
        }

        /// <summary>
        /// ノード処理実行用クラス
        /// </summary>
        protected abstract class Logic<TNode> : ILogic
            where TNode : Node {
            // 開始済みか
            private bool _started;
            // 実行主のLogic
            private ILogic _parentNodeLogic;
            // 現在の状態
            private State _state;

            // 現在の状態
            public State State => _state;
            // 実行中か
            public bool IsRunning => _state == State.Running;
            // 制御対象のNode
            Node ILogic.TargetNode => Node;
            
            // 制御用コントローラ
            protected IBehaviourTreeController Controller { get; private set; }
            // 参照元のノード
            protected TNode Node { get; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, TNode node) {
                Controller = controller;
                Node = node;
                _state = State.Inactive;
            }

            /// <summary>
            /// 初期化処理
            /// </summary>
            public void Initialize() {
                OnInitialize();
            }

            /// <summary>
            /// 廃棄時処理
            /// </summary>
            public void Dispose() {
                Reset();
                OnDispose();
            }

            /// <summary>
            /// 更新処理
            /// </summary>
            /// <param name="deltaTime">更新にかける時間</param>
            /// <param name="parentNodeLogic">呼び出し元のLogic</param>
            public void Update(ILogic parentNodeLogic) {
                _parentNodeLogic = parentNodeLogic;
                
                Start();

                // 自身の更新
                _state = OnUpdate();

                // 実行完了している場合
                if (!IsRunning) {
                    Stop();
                }
                
                // 親NodeLogicに更新結果を送る
                if (_parentNodeLogic != null) {
                    _parentNodeLogic.SendChildResult(this);
                }
            }

            /// <summary>
            /// Running状態の更新処理
            /// </summary>
            public void UpdateRunning(ILogic childNodeLogic) {
                if (childNodeLogic != null) {
                    SendChildResult(childNodeLogic);
                }
                
                if (IsRunning) {
                    // 自身の更新
                    _state = OnUpdate();
                }

                // 実行完了している場合
                if (!IsRunning) {
                    Stop();
                }
            }

            /// <summary>
            /// 子要素の結果を送る
            /// </summary>
            public void SendChildResult(ILogic childNodeLogic) {
                if (childNodeLogic == null) {
                    return;
                }
                
                _state = OnUpdatedChild(childNodeLogic);

                if (_state != State.Running) {
                    Stop();
                }
            }

            /// <summary>
            /// リセット処理
            /// </summary>
            public void Reset() {
                Stop();
                _parentNodeLogic = null;
                _state = State.Inactive;
            }

            protected virtual void OnInitialize() {
            }

            protected virtual void OnDispose() {
            }

            protected virtual void OnStart() {
            }

            protected abstract State OnUpdate();

            protected abstract State OnUpdatedChild(ILogic childNodeLogic);

            protected virtual void OnStop() {
            }

            /// <summary>
            /// ノードの実行
            /// </summary>
            protected void UpdateNode(Node node) {
                Controller.UpdateNode(this, node);
            }

            /// <summary>
            /// 開始処理
            /// </summary>
            private void Start() {
                if (!_started) {
                    OnStart();
                    _started = true;
                }
            }

            /// <summary>
            /// 停止処理
            /// </summary>
            private void Stop() {
                if (_started) {
                    OnStop();
                    _started = false;
                }
            }
        }

        [HideInInspector, Tooltip("GUID")]
        public string guid;
        [HideInInspector, Tooltip("GraphView上の座標")]
        public Vector2 position;
        [Tooltip("タイトル")]
        public string title;

        // 説明文
        public virtual string Description => "";

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public abstract ILogic CreateLogic(IBehaviourTreeController controller);

        /// <summary>
        /// 生成時処理
        /// </summary>
        private void Awake() {
            OnValidate();
        }

        /// <summary>
        /// 値変化通知
        /// </summary>
        private void OnValidate() {
            //hideFlags |= HideFlags.HideInHierarchy;
            hideFlags &= ~HideFlags.HideInHierarchy;
        }
    }
}
using System;
using UnityEditor;
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
            /// 初期化処理
            /// </summary>
            void Initialize();

            /// <summary>
            /// 実行処理
            /// </summary>
            State Update(float deltaTime, bool back);

            /// <summary>
            /// キャンセル処理
            /// </summary>
            void Cancel();
        }

        /// <summary>
        /// ノード処理実行用クラス
        /// </summary>
        protected abstract class Logic<TNode> : ILogic
            where TNode : Node {
            // 開始済みか
            private bool _started;
            
            // 現在の状態
            public State State { get; private set; }
            // 制御用コントローラ
            protected IBehaviourTreeController Controller { get; private set; }
            // 参照元のノード
            protected TNode Node { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, TNode node) {
                Controller = controller;
                Node = node;
                State = State.Inactive;
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
                Cancel();
                OnDispose();
            }

            /// <summary>
            /// 更新処理
            /// </summary>
            public State Update(float deltaTime, bool back) {
                Start();

                var state = OnUpdate(deltaTime, back);
                State = state;

                if (state != State.Running) {
                    Stop();
                }

                return state;
            }

            /// <summary>
            /// キャンセル処理
            /// </summary>
            public void Cancel() {
                Stop();
            }

            protected virtual void OnInitialize() {
            }

            protected virtual void OnDispose() {
            }

            protected virtual void OnStart() {
            }

            protected abstract State OnUpdate(float deltaTime, bool back);

            protected virtual void OnStop() {
            }

            /// <summary>
            /// ノードの実行
            /// </summary>
            protected State UpdateNode(Node node, float deltaTime) {
                return Controller.UpdateNode(node, deltaTime, false);
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
                    State = State.Inactive;
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
            hideFlags |= HideFlags.HideInHierarchy;
        }
    }
}
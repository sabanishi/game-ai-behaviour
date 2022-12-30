using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// Graphデータ本体
    /// </summary>
    public abstract class Node : ScriptableObject {
        // 状態
        public enum State {
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
            State Update(float deltaTime);

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
            protected BehaviourTreeController Controller { get; private set; }
            // 参照元のノード
            protected TNode Node { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(BehaviourTreeController controller, TNode node) {
                Controller = controller;
                Node = node;
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
            public State Update(float deltaTime) {
                if (!_started) {
                    OnStart();
                    _started = true;
                }

                State = OnUpdate(deltaTime);

                if (State != State.Running) {
                    OnStop();
                    _started = false;
                }

                return State;
            }

            /// <summary>
            /// キャンセル処理
            /// </summary>
            public void Cancel() {
                if (_started) {
                    OnStop();
                    _started = false;
                }
            }

            protected virtual void OnInitialize() {
            }

            protected virtual void OnDispose() {
            }

            protected virtual void OnStart() {
            }

            protected abstract State OnUpdate(float deltaTime);

            protected virtual void OnStop() {
            }

            /// <summary>
            /// ノードの実行
            /// </summary>
            protected State UpdateNode(Node node, float deltaTime) {
                return Controller.UpdateNode(node, deltaTime);
            }
        }

        [HideInInspector, Tooltip("GUID")]
        public string guid;
        [HideInInspector, Tooltip("GraphView上の座標")]
        public Vector2 position;
        [Tooltip("タイトル")]
        public string title;

        // 表示名
        public virtual string DisplayName => string.IsNullOrEmpty(title) ? GetType().Name : title;

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public abstract ILogic CreateLogic(BehaviourTreeController controller);
    }
}
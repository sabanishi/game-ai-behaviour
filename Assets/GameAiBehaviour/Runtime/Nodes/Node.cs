using System;
using System.Collections;
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
            /// 実行ルーチン
            /// </summary>
            IEnumerator UpdateRoutine();

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
            // 現在の状態
            public State State { get; private set; }
            // 実行中か
            public bool IsRunning => State == State.Running;
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
                State = State.Inactive;
            }

            /// <summary>
            /// 初期化処理
            /// </summary>
            public void Initialize() {
                InitializeInternal();
            }

            /// <summary>
            /// 廃棄時処理
            /// </summary>
            public void Dispose() {
                ((ILogic)this).Reset();
                DisposeInternal();
            }

            /// <summary>
            /// 更新ルーチン
            /// </summary>
            IEnumerator ILogic.UpdateRoutine() {
                // 初期値はSuccessにしておく
                State = State.Success;
                
                // Override用コード実行
                yield return UpdateRoutineInternal();
                
                // 現在の状態をルーチン実行元に通知
                yield return State;
                Debug.LogWarning($"{Node.GetType().Name}:{State}");
            }

            /// <summary>
            /// リセット処理
            /// </summary>
            void ILogic.Reset() {
                if (State != State.Inactive) {
                    ResetInternal();
                    State = State.Inactive;
                }
            }

            /// <summary>
            /// 初期化処理(Override用)
            /// </summary>
            protected virtual void InitializeInternal() {
            }

            /// <summary>
            /// 削除処理(Override用)
            /// </summary>
            protected virtual void DisposeInternal() {
            }

            /// <summary>
            /// 更新ルーチン(Override用)
            /// </summary>
            protected virtual IEnumerator UpdateRoutineInternal() {
                State = State.Success;
                yield break;
            }

            /// <summary>
            /// 思考リセット時の処理(Override用)
            /// </summary>
            protected virtual void ResetInternal() {
            }

            /// <summary>
            /// 状態の変更
            /// </summary>
            protected void SetState(State state) {
                State = state;
            }

            /// <summary>
            /// ノードの実行
            /// </summary>
            protected IEnumerator UpdateNodeRoutine(Node node, Action<State> onResult) {
                var logic = Controller.FindLogic(node);
                
                // Logicが存在しない場合、完了扱い
                if (logic == null) {
                    Debug.Log($"Skip node logic. {node.GetType().Name}");
                    yield return State.Success;
                    onResult?.Invoke(State.Success);
                    yield break;
                }

                // Logicが存在する場合、さらにRoutineを実行
                yield return logic.UpdateRoutine();
                onResult?.Invoke(logic.State);
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
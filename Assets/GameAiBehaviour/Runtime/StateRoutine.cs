using System;
using System.Collections;
using System.Collections.Generic;

namespace GameAiBehaviour {
    /// <summary>
    /// State実行用コルーチン
    /// </summary>
    public class StateRoutine : IEnumerator {
        private IEnumerator _enumerator;
        private Stack<object> _stack;
        private Node.State _current;

        // 完了しているか
        public bool IsDone { get; private set; }
        // 現在のState
        public Node.State Current => _current;
        // Interface実装用
        object IEnumerator.Current => _current;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="enumerator">実行対象のEnumerator</param>
        public StateRoutine(IEnumerator enumerator) {
            _enumerator = enumerator;
            _stack = new Stack<object>();
            ((IEnumerator)this).Reset();
        }
        
        /// <summary>
        /// Routineを進める
        /// </summary>
        public bool MoveNext() {
            Update();
            return !IsDone;
        }

        /// <summary>
        /// Routineをリセット
        /// </summary>
        public void Reset() {
            _stack.Clear();
            _stack.Push(_enumerator);
            _current = Node.State.Inactive;
            IsDone = false;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update() {
            // スタックがなければ、完了
            if (_stack.Count == 0) {
                IsDone = true;
                return;
            }

            // スタックを取り出して、処理を進める
            var peek = _stack.Peek();
            
            // nullはRunning扱い
            if (peek == null) {
                _current = Node.State.Running;
                _stack.Pop();
            }
            // State返却
            else if (peek is Node.State state) {
                _current = state;
                _stack.Pop();

                if (state != Node.State.Running) {
                    Update();
                }
            }
            // Stack追加呼び出し
            else if (peek is IEnumerator enumerator) {
                if (enumerator.MoveNext()) {
                    _stack.Push(enumerator.Current);
                }
                else {
                    _stack.Pop();
                }

                Update();
            }
            // それ以外は非対応
            else {
                throw new NotSupportedException($"{peek.GetType()} is not support");
            }
        }
    }
}
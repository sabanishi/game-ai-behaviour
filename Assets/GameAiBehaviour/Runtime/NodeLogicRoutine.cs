using System;
using System.Collections;
using System.Collections.Generic;

namespace GameAiBehaviour {
    /// <summary>
    /// NodeLogic実行用コルーチン
    /// </summary>
    public class NodeLogicRoutine : IEnumerator {
        private IEnumerator _enumerator;
        private Stack<object> _stack;
        private Node.ILogic _current;

        // 完了しているか
        public bool IsDone { get; private set; }
        // 現在のNodeLogic
        public Node.ILogic Current => _current;
        // Interface実装用
        object IEnumerator.Current => _current;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="enumerator">実行対象のEnumerator</param>
        public NodeLogicRoutine(IEnumerator enumerator) {
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
            _current = null;
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

            // NodeLogic返却はCurrentを更新して待つ(Running扱い)
            if (peek is Node.ILogic logic) {
                _current = logic;
                _stack.Pop();
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
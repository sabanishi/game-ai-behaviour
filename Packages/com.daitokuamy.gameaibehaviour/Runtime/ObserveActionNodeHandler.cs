using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノードハンドリング用基底クラス
    /// </summary>
    public sealed class ObserveActionNodeHandler<TNode> : ActionNodeHandler<TNode>
        where TNode : HandleableActionNode {
        private Func<TNode, bool> _enterFunc;
        private Func<TNode, IActionNodeHandler.State> _updateFunc;
        private Action<TNode> _exitAction;
        private Action<TNode> _cancelAction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObserveActionNodeHandler() {
        }

        /// <summary>
        /// 開始時処理登録
        /// </summary>
        public void SetEnterAction(Func<TNode, bool> enterFunc) {
            _enterFunc = enterFunc;
        }

        /// <summary>
        /// 更新時処理登録
        /// </summary>
        public void SetUpdateFunc(Func<TNode, IActionNodeHandler.State> updateFunc) {
            _updateFunc = updateFunc;
        }

        /// <summary>
        /// 終了時処理登録
        /// </summary>
        public void SetExitAction(Action<TNode> exitAction) {
            _exitAction = exitAction;
        }

        /// <summary>
        /// キャンセル時処理登録
        /// </summary>
        public void SetCancelAction(Action<TNode> cancelAction) {
            _cancelAction = cancelAction;
        }

        /// <summary>
        /// 実行ノードの開始処理
        /// </summary>
        protected override bool OnEnterInternal(TNode node) {
            return _enterFunc?.Invoke(node) ?? true;
        }

        /// <summary>
        /// 実行ノードの更新処理
        /// </summary>
        protected override IActionNodeHandler.State OnUpdateInternal(TNode node) {
            return _updateFunc?.Invoke(node) ?? IActionNodeHandler.State.Success;
        }

        /// <summary>
        /// 実行ノードの終了処理
        /// </summary>
        protected override void OnExitInternal(TNode node) {
            _exitAction?.Invoke(node);
        }

        /// <summary>
        /// 実行ノードのキャンセル処理
        /// </summary>
        protected override void OnCancelInternal(TNode node) {
            _cancelAction?.Invoke(node);
        }
    }
}
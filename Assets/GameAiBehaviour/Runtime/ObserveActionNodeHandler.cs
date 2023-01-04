using System;

namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノードハンドリング用基底クラス
    /// </summary>
    public sealed class ObserveActionNodeHandler<TNode> : ActionNodeHandler<TNode>
        where TNode : HandleableActionNode {

        private Action<TNode> _enterAction;
        private Func<TNode, float, Node.State> _updateFunc;
        private Action<TNode> _exitAction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObserveActionNodeHandler() {
        }

        /// <summary>
        /// 開始時処理登録
        /// </summary>
        public void SetEnterAction(Action<TNode> enterAction) {
            _enterAction = enterAction;
        }
        
        /// <summary>
        /// 更新時処理登録
        /// </summary>
        public void SetUpdateFunc(Func<TNode, float, Node.State> updateFunc) {
            _updateFunc = updateFunc;
        }

        /// <summary>
        /// 終了時処理登録
        /// </summary>
        public void SetExitAction(Action<TNode> exitAction) {
            _exitAction = exitAction;
        }

        /// <summary>
        /// 実行ノードの開始処理
        /// </summary>
        protected override void OnEnterInternal(TNode node) {
            _enterAction?.Invoke(node);
        }

        /// <summary>
        /// 実行ノードの更新処理
        /// </summary>
        protected override Node.State OnUpdateInternal(TNode node, float deltaTime) {
            return _updateFunc?.Invoke(node, deltaTime) ?? Node.State.Success;
        }

        /// <summary>
        /// 実行ノードの終了処理
        /// </summary>
        protected override void OnExitInternal(TNode node) {
            _exitAction?.Invoke(node);
        }
    }
}
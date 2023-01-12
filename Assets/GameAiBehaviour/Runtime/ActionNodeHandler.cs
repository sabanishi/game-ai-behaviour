namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノードハンドリング用インターフェース
    /// </summary>
    public interface IActionNodeHandler {
        void OnEnter(HandleableActionNode node);
        Node.State OnUpdate(HandleableActionNode node);
        void OnExit(HandleableActionNode node);
    }
    
    /// <summary>
    /// 実行ノードハンドリング用基底クラス
    /// </summary>
    public abstract class ActionNodeHandler<TNode> : IActionNodeHandler
        where TNode : HandleableActionNode {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ActionNodeHandler() {
        }

        void IActionNodeHandler.OnEnter(HandleableActionNode node) {
            OnEnterInternal((TNode)node);
        }

        Node.State IActionNodeHandler.OnUpdate(HandleableActionNode node) {
            return OnUpdateInternal((TNode)node);
        }

        void IActionNodeHandler.OnExit(HandleableActionNode node) {
            OnExitInternal((TNode)node);
        }

        /// <summary>
        /// 実行ノードの開始処理
        /// </summary>
        protected virtual void OnEnterInternal(TNode node) {}

        /// <summary>
        /// 実行ノードの更新処理
        /// </summary>
        protected virtual Node.State OnUpdateInternal(TNode node) {
            return Node.State.Success;
        }

        /// <summary>
        /// 実行ノードの終了処理
        /// </summary>
        protected virtual void OnExitInternal(TNode node) {}
    }
}
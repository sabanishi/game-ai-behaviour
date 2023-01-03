namespace GameAiBehaviour {
    /// <summary>
    /// 実行ノードハンドリング用インターフェース
    /// </summary>
    public interface IActionNodeHandler {
        void OnEnter(HandleableActionNode node);
        bool OnUpdate(HandleableActionNode node, float deltaTime);
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

        bool IActionNodeHandler.OnUpdate(HandleableActionNode node, float deltaTime) {
            return OnUpdateInternal((TNode)node, deltaTime);
        }

        void IActionNodeHandler.OnExit(HandleableActionNode node) {
            OnExitInternal((TNode)node);
        }

        /// <summary>
        /// 実行ノードの開始処理
        /// </summary>
        protected abstract void OnEnterInternal(TNode node);

        /// <summary>
        /// 実行ノードの更新処理
        /// </summary>
        protected virtual bool OnUpdateInternal(TNode node, float deltaTime) {
            return false;
        }

        /// <summary>
        /// 実行ノードの終了処理
        /// </summary>
        protected virtual void OnExitInternal(TNode node) {}
    }
}
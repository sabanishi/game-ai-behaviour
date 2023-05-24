namespace GameAiBehaviour {
    /// <summary>
    /// 接続ノードハンドリング用インターフェース
    /// </summary>
    public interface ILinkNodeHandler {
        /// <summary>
        /// 接続先のNodeを取得
        /// </summary>
        Node GetConnectNode(HandleableLinkNode node);
    }

    /// <summary>
    /// 接続ノードハンドリング用基底クラス
    /// </summary>
    public abstract class LinkNodeHandler<TNode> : ILinkNodeHandler
        where TNode : HandleableLinkNode {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinkNodeHandler() {
        }

        /// <summary>
        /// 接続先のNodeを取得
        /// </summary>
        Node ILinkNodeHandler.GetConnectNode(HandleableLinkNode node) {
            return GetConnectNodeInternal((TNode)node);
        }

        /// <summary>
        /// 接続先のNodeを取得
        /// </summary>
        protected abstract Node GetConnectNodeInternal(TNode node);
    }
}
using System;

namespace GameAiBehaviour {
    /// <summary>
    /// リンクノードハンドリング用基底クラス
    /// </summary>
    public sealed class ObserveLinkNodeHandler<TNode> : LinkNodeHandler<TNode>
        where TNode : HandleableLinkNode {
        private Func<TNode, Node> _getConnectNodeFunc;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObserveLinkNodeHandler() {
        }
        
        /// <summary>
        /// 接続先ノード取得用処理登録
        /// </summary>
        public void SetGetConnectNodeFunc(Func<TNode, Node> getConnectNodeFunc) {
            _getConnectNodeFunc = getConnectNodeFunc;
        }

        /// <summary>
        /// 接続先のNodeを取得
        /// </summary>
        protected override Node GetConnectNodeInternal(TNode node) {
            return _getConnectNodeFunc?.Invoke(node);
        }
    }
}
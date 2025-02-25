namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeのノード実行用インターフェース
    /// </summary>
    public interface IBehaviourTreeRunner {
        /// <summary>
        /// Runnerを制御するためのコントローラ
        /// </summary>
        IBehaviourTreeController Controller { get; }
        
        /// <summary>
        /// 乱数生成用のインスタンス
        /// </summary>
        IRandom Random { get; }

        /// <summary>
        /// ノード用ロジックを検索
        /// </summary>
        Node.ILogic GetLogic(Node node);

        /// <summary>
        /// ノードにBindされているActionNodeHandlerの取得
        /// </summary>
        IActionNodeHandler GetActionNodeHandler(HandleableActionNode node);

        /// <summary>
        /// ノードにBindされているLinkNodeHandlerの取得
        /// </summary>
        ILinkNodeHandler GetLinkNodeHandler(HandleableLinkNode node);

        /// <summary>
        /// 実行パスの追加
        /// </summary>
        void AddExecutedPath(Node.ILogic prev, Node.ILogic next);
    }
}
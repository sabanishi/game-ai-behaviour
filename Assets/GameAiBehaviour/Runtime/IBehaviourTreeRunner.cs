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
        /// ノード用ロジックを検索
        /// </summary>
        Node.ILogic FindLogic(Node node);

        /// <summary>
        /// 実行パスの追加
        /// </summary>
        void AddExecutedPath(Node.ILogic prev, Node.ILogic next);
    }
}
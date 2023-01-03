namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御用のインターフェース
    /// </summary>
    public interface IBehaviourTreeController {
        /// <summary>
        /// ノードの実行
        /// </summary>
        Node.State UpdateNode(Node node, float deltaTime, bool back);

        /// <summary>
        /// ノードにBindされているHandlerの取得
        /// </summary>
        IActionNodeHandler GetActionHandler(HandleableActionNode node);
    }
}
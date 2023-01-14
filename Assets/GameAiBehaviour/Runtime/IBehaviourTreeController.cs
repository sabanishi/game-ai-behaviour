using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御用のインターフェース
    /// </summary>
    public interface IBehaviourTreeController {
        /// <summary>
        /// Blackboard
        /// </summary>
        Blackboard Blackboard { get; }

        /// <summary>
        /// ノード用ロジックを検索
        /// </summary>
        Node.ILogic FindLogic(Node node);

        /// <summary>
        /// ノードにBindされているHandlerの取得
        /// </summary>
        IActionNodeHandler GetActionHandler(HandleableActionNode node);
    }
}
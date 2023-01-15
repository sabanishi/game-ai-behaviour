using System.Collections;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御用のインターフェース
    /// </summary>
    public interface IBehaviourTreeController {
        /// <summary>
        /// 思考実行してからの経過時間
        /// </summary>
        float ThinkTime { get; }

        /// <summary>
        /// Blackboard
        /// </summary>
        Blackboard Blackboard { get; }

        /// <summary>
        /// ノードにBindされているHandlerの取得
        /// </summary>
        IActionNodeHandler GetActionHandler(HandleableActionNode node);

        /// <summary>
        /// 思考リセット
        /// </summary>
        void ResetThink();

        /// <summary>
        /// TickTimerのリセット
        /// </summary>
        void ResetTickTimer();
    }
}
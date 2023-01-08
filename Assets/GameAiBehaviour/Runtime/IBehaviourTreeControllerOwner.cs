namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeControllerの持ち主用インターフェース
    /// </summary>
    public interface IBehaviourTreeControllerOwner {
        /// <summary>
        /// 制御用BehaviourTreeController
        /// </summary>
        BehaviourTreeController BehaviourTreeController { get; }
    }
}
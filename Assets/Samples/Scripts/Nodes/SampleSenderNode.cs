using GameAiBehaviour;
using UnityEngine;

[BehaviourTreeNode("Test/サンプル送信者")]
public class SampleSenderNode:HandleableActionNode {
}

public class SampleSenderNodeHandler : ActionNodeHandler<SampleSenderNode> {
    private Blackboard _blackboard;
    private GameObject _sendTarget;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Setup(Blackboard blackboard,GameObject sendTarget) {
        _blackboard = blackboard;
        _sendTarget = sendTarget;
    }

    /// <summary>
    /// ノード開始時処理
    /// </summary>
    protected override bool OnEnterInternal(SampleSenderNode node) {
        return true;
    }

    protected override IActionNodeHandler.State OnUpdateInternal(SampleSenderNode node) {
        //Blackboardに値をセット
        _blackboard.SetGameObject("SampleGameObject", _sendTarget);
        base.OnUpdateInternal(node);
        return IActionNodeHandler.State.Success;
    }
}
using GameAiBehaviour;
using UnityEngine;

[BehaviourTreeNode("Test/サンプル受信者")]
public class SampleReceiverNode :HandleableActionNode{
}

public class SampleReceiverNodeHandler : ActionNodeHandler<SampleReceiverNode> {
    private Blackboard _blackboard;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Setup(Blackboard blackboard) {
        _blackboard = blackboard;
    }
    
    /// <summary>
    /// ノード開始時処理
    /// </summary>
    protected override bool OnEnterInternal(SampleReceiverNode node) {
        return true;
    }

    protected override IActionNodeHandler.State OnUpdateInternal(SampleReceiverNode node) {
        //Blackboardから値を取得
        var target = _blackboard.GetGameObject("SampleGameObject");
        Debug.Log($"{target.name}:{target.transform.position}");
        
        return IActionNodeHandler.State.Success;
    }
}
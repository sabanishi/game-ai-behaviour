using GameAiBehaviour;
using UnityEngine;

/// <summary>
/// サンプル用のログ出力ノード
/// </summary>
[BehaviourTreeNode("Test/サンプルログ")]
public class SampleLogNode : HandleableActionNode {
    [Tooltip("メッセージ内容")]
    public string message;
}

/// <summary>
/// ハンドラ
/// </summary>
public class SampleLogNodeHandler : ActionNodeHandler<SampleLogNode> {
    /// <summary>
    /// ノード開始時処理
    /// </summary>
    protected override bool OnEnterInternal(SampleLogNode node) {
        Debug.Log(node.message);
        return true;
    }

    /// <summary>
    /// ノード更新時処理
    /// </summary>
    protected override IActionNodeHandler.State OnUpdateInternal(SampleLogNode node) {
        return IActionNodeHandler.State.Success;
    }

    protected override void OnCancelInternal(SampleLogNode node) {
        Debug.Log("Canceled action node handler");
    }
}
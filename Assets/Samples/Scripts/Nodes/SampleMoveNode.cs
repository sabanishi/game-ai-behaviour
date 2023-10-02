using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameAiBehaviour;
using UnityEngine;

/// <summary>
/// 一定距離移動するノード
/// </summary>
[BehaviourTreeNode("Test/サンプル移動")]
public class SampleMoveNode : HandleableActionNode {
    [Tooltip("相対移動量")]
    public Vector3 offset;
    [Tooltip("移動時間")]
    public float duration;
}

/// <summary>
/// 一定距離移動するノード用ハンドラ
/// </summary>
public class SampleMoveNodeHandler : ActionNodeHandler<SampleMoveNode> {
    private Agent _owner;
    private CancellationTokenSource _cancellationTokenSource;
    private UniTask _uniTask;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="owner">制御対象のTransform</param>
    public void Setup(Agent owner) {
        _owner = owner;
    }

    /// <summary>
    /// ノード開始時処理
    /// </summary>
    protected override bool OnEnterInternal(SampleMoveNode node) {
        _cancellationTokenSource = new CancellationTokenSource();
        _uniTask = _owner.MoveAsync(node.offset, node.duration, _cancellationTokenSource.Token);
        return true;
    }

    /// <summary>
    /// ノード更新時処理
    /// </summary>
    protected override IActionNodeHandler.State OnUpdateInternal(SampleMoveNode node) {
        if (_uniTask.Status == UniTaskStatus.Pending) {
            return IActionNodeHandler.State.Running;
        }

        return IActionNodeHandler.State.Success;
    }

    /// <summary>
    /// ノード終了時処理
    /// </summary>
    protected override void OnExitInternal(SampleMoveNode node) {
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    protected override void OnCancelInternal(SampleMoveNode node) {
        Debug.Log("Canceled action node handler");
    }
}
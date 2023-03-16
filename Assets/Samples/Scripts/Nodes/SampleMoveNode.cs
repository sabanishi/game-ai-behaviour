using GameAiBehaviour;
using UnityEngine;

/// <summary>
/// 一定距離移動するノード
/// </summary>
public class SampleMoveNode : HandleableActionNode {
    [Tooltip("移動速度")]
    public Vector3 velocity;
    [Tooltip("移動時間")]
    public float duration;
}

/// <summary>
/// 一定距離移動するノード用ハンドラ
/// </summary>
public class SampleMoveNodeHandler : ActionNodeHandler<SampleMoveNode> {
    private Transform _owner;
    private float _timer;
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="owner">制御対象のTransform</param>
    public void Setup(Transform owner) {
        _owner = owner;
    }

    /// <summary>
    /// ノード開始時処理
    /// </summary>
    protected override bool OnEnterInternal(SampleMoveNode node) {
        _timer = node.duration;
        return true;
    }

    /// <summary>
    /// ノード更新時処理
    /// </summary>
    protected override IActionNodeHandler.State OnUpdateInternal(SampleMoveNode node) {
        var deltaTime = Time.deltaTime;
        
        // 秒数を減らす
        _timer -= deltaTime;
        
        // 移動する
        _owner.position += node.velocity * deltaTime;

        if (_timer > 0) {
            return IActionNodeHandler.State.Running;
        }

        return IActionNodeHandler.State.Success;
    }
}

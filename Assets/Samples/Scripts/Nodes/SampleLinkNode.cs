using System.Linq;
using GameAiBehaviour;
using UnityEngine;

/// <summary>
/// サンプル用のリンクノード
/// </summary>
public class SampleLinkNode : HandleableLinkNode {
    public BehaviourTree[] trees;
}

/// <summary>
/// サンプル用のリンクノード用ハンドラ
/// </summary>
public class SampleLinkNodeHandler : LinkNodeHandler<SampleLinkNode> {
    /// <summary>
    /// 接続先のノードを取得
    /// </summary>
    protected override Node GetConnectNodeInternal(SampleLinkNode node) {
        return node.trees.OrderBy(_ => Random.Range(0, 100)).FirstOrDefault()?.rootNode;
    }
}

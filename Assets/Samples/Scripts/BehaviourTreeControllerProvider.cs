using GameAiBehaviour;
using UnityEngine;

/// <summary>
/// BehaviourTreeの可視化用コンポーネント
/// </summary>
public class BehaviourTreeControllerProvider : MonoBehaviour, IBehaviourTreeControllerProvider
{
    public BehaviourTreeController BehaviourTreeController { get; private set; }

    public void Set(BehaviourTreeController controller)
    {
        BehaviourTreeController = controller;
    }
}
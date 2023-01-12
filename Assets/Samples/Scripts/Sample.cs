
using GameAiBehaviour;
using UnityEngine;

public class Sample : MonoBehaviour, IBehaviourTreeControllerOwner {
    [SerializeField, Tooltip("実行対象のツリー")]
    private BehaviourTree _behaviourTree;

    private BehaviourTreeController _controller;

    BehaviourTreeController IBehaviourTreeControllerOwner.BehaviourTreeController => _controller;

    private void Awake() {
        _controller = new BehaviourTreeController();
        _controller.Setup(_behaviourTree);
    }

    private void Update() {
        _controller.Update(Time.deltaTime);
    }

    private void OnDestroy() {
        _controller.Cleanup();
    }
}

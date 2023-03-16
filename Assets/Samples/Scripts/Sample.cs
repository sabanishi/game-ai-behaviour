using GameAiBehaviour;
using UnityEngine;

public class Sample : MonoBehaviour, IBehaviourTreeControllerOwner {
    [SerializeField, Tooltip("実行対象のツリー")]
    private BehaviourTree _behaviourTree;
    [SerializeField, Tooltip("思考頻度")]
    private float _tickInterval;
    [SerializeField, Tooltip("移動制御ターゲット")]
    private GameObject _moveTarget;

    private BehaviourTreeController _controller;

    BehaviourTreeController IBehaviourTreeControllerOwner.BehaviourTreeController => _controller;

    private void Awake() {
        _controller = new BehaviourTreeController();
        _controller.TickInterval = _tickInterval;
        _controller.Setup(_behaviourTree);
    }

    private void Start() {
        _controller.BindActionNodeHandler<SampleMoveNode, SampleMoveNodeHandler>(handler => {
            handler.Setup(_moveTarget.transform);
        });
    }

    private void Update() {
        _controller.Update(Time.deltaTime);
    }

    private void OnDestroy() {
        _controller.ResetActionNodeHandlers();
        _controller.Cleanup();
    }
}

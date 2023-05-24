using GameAiBehaviour;
using UnityEngine;

public class Sample : MonoBehaviour, IBehaviourTreeControllerProvider {
    [SerializeField, Tooltip("実行対象のツリー")]
    private BehaviourTree _behaviourTree;
    [SerializeField, Tooltip("思考頻度")]
    private float _tickInterval;
    [SerializeField, Tooltip("移動制御ターゲット")]
    private Agent _moveTarget;

    private BehaviourTreeController _controller;

    BehaviourTreeController IBehaviourTreeControllerProvider.BehaviourTreeController => _controller;

    private void Awake() {
        _controller = new BehaviourTreeController();
        _controller.TickInterval = _tickInterval;
        _controller.Setup(_behaviourTree);
    }

    private void Start() {
        _controller.BindActionNodeHandler<SampleMoveNode, SampleMoveNodeHandler>(handler => {
            handler.Setup(_moveTarget);
        });
        _controller.BindConditionHandler<SampleCondition>((condition, blackboard) => condition.test == 1);
    }

    private void Update() {
        _controller.Update(Time.deltaTime);
    }

    private void OnDestroy() {
        _controller.ResetActionNodeHandlers();
        _controller.Cleanup();
    }
}

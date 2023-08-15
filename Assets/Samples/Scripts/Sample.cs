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
    private Node.State _prevResultState = Node.State.Inactive;

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
        _controller.BindLinkNodeHandler<SampleLinkNode, SampleLinkNodeHandler>(_ => {});
    }

    private void Update() {
        var result = _controller.Update(Time.deltaTime);
        if (result != _prevResultState) {
            Debug.Log($"ResultState:{result}");
            _prevResultState = result;
        }
    }

    private void OnDestroy() {
        _controller.ResetActionNodeHandlers();
        _controller.Cleanup();
    }
}

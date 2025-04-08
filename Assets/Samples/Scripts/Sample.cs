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
    private BehaviourTreeControllerProvider _provider;
    private Blackboard _blackboard;
    private Node.State _prevResultState = Node.State.Inactive;

    BehaviourTreeController IBehaviourTreeControllerProvider.BehaviourTreeController => _controller;

    private void Awake() {
        _controller = new BehaviourTreeController();
        _controller.TickInterval = _tickInterval;
        _controller.Setup(_behaviourTree);
        _blackboard = _controller.Blackboard;
        _provider = gameObject.AddComponent<BehaviourTreeControllerProvider>();
    }

    private void Start() {
        _controller.BindActionNodeHandler<SampleMoveNode, SampleMoveNodeHandler>(handler => {
            handler.Setup(_moveTarget);
        });
        _controller.BindActionNodeHandler<SampleLogNode, SampleLogNodeHandler>(null);
        _controller.BindConditionHandler<SampleCondition>((condition, blackboard) => condition.test == 1);
        _controller.BindLinkNodeHandler<SampleLinkNode, SampleLinkNodeHandler>(_ => {});
        _controller.BindActionNodeHandler<SampleSenderNode, SampleSenderNodeHandler>(handler => {
            handler.Setup(_blackboard, _moveTarget.Target);
        });
        _controller.BindActionNodeHandler<SampleReceiverNode, SampleReceiverNodeHandler>(handler => {
            handler.Setup(_blackboard);
        });
        _provider.Set(_controller);
    }

    private void Update() {
        var result = _controller.Update(Time.deltaTime);
        if (result != _prevResultState) {
            Debug.Log($"ResultState:{result}");
            _prevResultState = result;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            _controller.Dispose();
        }
    }

    private void OnDestroy() {
        _controller.ResetActionNodeHandlers();
        _controller.Cleanup();
        _provider.Set(null);
    }
}

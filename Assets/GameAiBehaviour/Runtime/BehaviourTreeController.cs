using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTree制御クラス
    /// </summary>
    public class BehaviourTreeController : IBehaviourTreeController {
        /// <summary>
        /// ActionHandler情報
        /// </summary>
        private class ActionHandlerInfo {
            public Type Type;
            public Action<object> InitAction;
        }

        // 実行データ
        private BehaviourTree _data;
        // 思考リセットフラグ
        private bool _thinkResetFlag;
        // 思考タイミング用タイマー
        private float _tickTimer;
        // アクションハンドラ情報
        private readonly Dictionary<Type, ActionHandlerInfo> _actionHandlerInfos =
            new Dictionary<Type, ActionHandlerInfo>();
        // アクションノードハンドラ
        private readonly Dictionary<Node, IActionNodeHandler> _actionNodeHandlers =
            new Dictionary<Node, IActionNodeHandler>();

        // ベースとなるTreeのRunner
        private BehaviourTreeRunner _baseRunner;

        // 思考開始からの経過時間
        public float ThinkTime { get; private set; }
        // プロパティ管理用Blackboard
        public Blackboard Blackboard { get; private set; } = new Blackboard();
        // 思考頻度
        public float TickInterval { get; set; }
        // 実行履歴パス
        public IReadOnlyList<BehaviourTreeRunner.Path> ExecutedPaths => _baseRunner.ExecutedPaths;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BehaviourTreeController() {
        }

        /// <summary>
        /// ActionNodeHandlerのBind
        /// </summary>
        /// <param name="onInit">Handlerの初期化関数</param>
        public void BindActionNodeHandler<TNode, THandler>(Action<THandler> onInit)
            where TNode : HandleableActionNode
            where THandler : ActionNodeHandler<TNode>, new() {
            ResetActionNodeHandler<TNode>();

            _actionHandlerInfos[typeof(TNode)] = new ActionHandlerInfo {
                Type = typeof(THandler),
                InitAction = onInit != null ? obj => { onInit.Invoke(obj as THandler); } : null
            };
        }

        /// <summary>
        /// ActionNodeHandlerのBind
        /// </summary>
        /// <param name="updateFunc">更新関数</param>
        /// <param name="enterAction">開始関数</param>
        /// <param name="exitAction">終了関数</param>
        public void BindActionNodeHandler<TNode>(Func<TNode, IActionNodeHandler.State> updateFunc,
            Func<TNode, bool> enterAction = null, Action<TNode> exitAction = null)
            where TNode : HandleableActionNode {
            BindActionNodeHandler<TNode, ObserveActionNodeHandler<TNode>>(handler => {
                handler.SetEnterAction(enterAction);
                handler.SetUpdateFunc(updateFunc);
                handler.SetExitAction(exitAction);
            });
        }

        /// <summary>
        /// ActionNodeHandlerのBindを解除
        /// </summary>
        public void ResetActionNodeHandler<TNode>()
            where TNode : HandleableActionNode {
            _actionHandlerInfos.Remove(typeof(TNode));

            // 既に登録済のHandlerがあった場合は削除する
            var removeKeys = _actionNodeHandlers.Keys
                .Where(x => x.GetType() == typeof(TNode))
                .ToArray();
            foreach (var removeKey in removeKeys) {
                _actionNodeHandlers.Remove(removeKey);
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Setup(BehaviourTree data) {
            Cleanup();

            _data = data;

            if (_data == null) {
                return;
            }

            _baseRunner = new BehaviourTreeRunner(this, _data.rootNode);

            // Blackboard初期化
            if (data.blackboardAsset != null) {
                foreach (var property in data.blackboardAsset.properties) {
                    switch (property.propertyType) {
                        case Property.Type.Integer:
                            Blackboard.SetInteger(property.propertyName, property.integerValue);
                            break;
                        case Property.Type.Float:
                            Blackboard.SetFloat(property.propertyName, property.floatValue);
                            break;
                        case Property.Type.String:
                            Blackboard.SetString(property.propertyName, property.stringValue);
                            break;
                        case Property.Type.Boolean:
                            Blackboard.SetBoolean(property.propertyName, property.booleanValue);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 思考リセット
        /// </summary>
        public void ResetThink() {
            _baseRunner?.ResetThink();
            _tickTimer = 0.0f;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Cleanup() {
            ResetThink();

            Blackboard.Clear();

            _baseRunner?.Cleanup();
            _baseRunner = null;

            _data = null;
            _actionHandlerInfos.Clear();
            _actionNodeHandlers.Clear();
        }

        /// <summary>
        /// Tree更新
        /// </summary>
        public void Update(float deltaTime) {
            if (_data == null || _baseRunner == null) {
                return;
            }

            // 思考時間更新
            ThinkTime += deltaTime;

            // Tickタイマー更新
            if (_tickTimer > 0.0f) {
                _tickTimer -= deltaTime;
                return;
            }

            _tickTimer = TickInterval;

            // 基本思考の実行
            _baseRunner.Tick(() => ThinkTime = 0.0f);

            // 思考リセットフラグが立っていたらリセットする
            if (_thinkResetFlag) {
                ResetThink();
                _thinkResetFlag = false;
            }
        }

        /// <summary>
        /// ActionNodeのハンドリング用インスタンスを取得
        /// </summary>
        IActionNodeHandler IBehaviourTreeController.GetActionHandler(HandleableActionNode node) {
            if (node == null) {
                return null;
            }

            if (_actionNodeHandlers.TryGetValue(node, out var handler)) {
                return handler;
            }

            // 無ければ生成
            if (_actionHandlerInfos.TryGetValue(node.GetType(), out var handlerInfo)) {
                var constructorInfo = handlerInfo.Type.GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null) {
                    handler = (IActionNodeHandler)constructorInfo.Invoke(Array.Empty<object>());
                    _actionNodeHandlers[node] = handler;
                    handlerInfo.InitAction?.Invoke(handler);
                }
            }

            return handler;
        }

        /// <summary>
        /// 思考リセット
        /// </summary>
        void IBehaviourTreeController.ResetThink() {
            _thinkResetFlag = true;
        }

        /// <summary>
        /// ThickTimerのリセット(次回の思考Intervalがなくなる)
        /// </summary>
        void IBehaviourTreeController.ResetTickTimer() {
            _tickTimer = 0.0f;
        }
    }
}
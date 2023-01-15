using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameAiBehaviour {
    /// <summary>
    /// FunctionRootノードを実行するためのノード
    /// </summary>
    public sealed class FunctionNode : LinkNode {
        private class Logic : Logic<FunctionNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(IBehaviourTreeController controller, FunctionNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行ルーチン
            /// </summary>
            protected override IEnumerator ExecuteRoutineInternal() {
                // 接続先がない場合は失敗
                if (Node.functionRoot == null) {
                    SetState(State.Failure);
                    yield break;
                }

                // 接続先ノードの実行
                yield return ExecuteNodeRoutine(Node.functionRoot, SetState);
            }
        }

        [Tooltip("接続先のFunctionNode")]
        public FunctionRootNode functionRoot;

        public override string Description {
            get {
                if (functionRoot == null) {
                    return "Empty";
                }

                var nodeTitle = functionRoot.title;
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(nodeTitle)) {
                    nodeTitle = ObjectNames.NicifyVariableName(functionRoot.GetType().Name);
                }
#endif

                return $"to {nodeTitle}";
            }
        }

        public override ILogic CreateLogic(IBehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
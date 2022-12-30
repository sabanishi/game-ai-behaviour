using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 起点となるノード
    /// </summary>
    public sealed class RootNode : Node {
        [HideInInspector, Tooltip("接続先ノード")]
        public Node child;

        private class Logic : Logic<RootNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(BehaviourTreeController controller, RootNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            /// <returns></returns>
            protected override State OnUpdate(float deltaTime) {
                var child = Node.child;
                if (child != null) {
                    return UpdateNode(child, deltaTime);
                }

                return State.Failure;
            }
        }

        /// <summary>
        /// ロジックの生成
        /// </summary>
        public override ILogic CreateLogic(BehaviourTreeController controller) {
            return new Logic(controller, this);
        }
    }
}
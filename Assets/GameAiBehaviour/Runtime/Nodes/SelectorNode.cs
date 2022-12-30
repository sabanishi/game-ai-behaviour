using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAiBehaviour {
    /// <summary>
    /// 選択用ノード
    /// </summary>
    public class SelectorNode : CompositeNode {
        private class Logic : Logic<SelectorNode> {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Logic(BehaviourTreeController controller, SelectorNode node) : base(controller, node) {
            }

            /// <summary>
            /// 実行処理
            /// </summary>
            protected override State OnUpdate(float deltaTime) {
                foreach (var child in Node.children) {
                    var state = UpdateNode(child, deltaTime);
                    if (state != State.Failure) {
                        return state;
                    }
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
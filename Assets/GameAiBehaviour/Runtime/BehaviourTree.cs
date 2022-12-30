using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameAiBehaviour {
    /// <summary>
    /// BehaviourTreeアセット
    /// </summary>
    [CreateAssetMenu(fileName = "dat_behaviour_tree_hoge.asset", menuName = "GameAiBehaviour/Create Tree")]
    public class BehaviourTree : ScriptableObject {
        [Tooltip("メモ")]
        public string memo = "";
        [HideInInspector, Tooltip("所持ノードリスト")]
        public Node[] nodes = new Node[0];

#if UNITY_EDITOR
        /// <summary>
        /// ノードの生成
        /// </summary>
        public Node CreateNode(System.Type type) {
            Undo.RecordObject(this, "CreateNode");

            var node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "CreateNode");

            var list = nodes.ToList();
            list.Add(node);
            nodes = list.ToArray();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            return node;
        }

        /// <summary>
        /// ノードの削除
        /// </summary>
        public void DeleteNode(Node node) {
            Undo.RecordObject(this, "DeleteNode");

            var serializedObj = new SerializedObject(this);
            serializedObj.Update();

            var list = nodes.ToList();
            if (list.Remove(node)) {
                nodes = list.ToArray();
            }

            AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 子ノードのソート
        /// </summary>
        public void SortChildren(Node parent) {
            Undo.RecordObject(parent, "Sort Children");

            if (parent is CompositeNode composite) {
                var sortedChildren = composite.children.OrderBy(x => x.position.y).ToList();
                sortedChildren.Sort((a, b) => a.position.y.CompareTo(b.position.y));
                composite.children = sortedChildren.ToArray();
            }

            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// ノードの接続
        /// </summary>
        public void ConnectNode(Node parent, Node child) {
            Undo.RecordObject(parent, "ConnectNode");

            if (parent is DecoratorNode decorator) {
                decorator.child = child;
            }
            else if (parent is CompositeNode composite) {
                if (!composite.children.Contains(child)) {
                    var list = composite.children.ToList();
                    list.Add(child);
                    composite.children = list.ToArray();
                }
            }
            else if (parent is RootNode root) {
                root.child = child;
            }

            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// ノードの接続解除
        /// </summary>
        public void DisconnectNode(Node parent, Node child) {
            Undo.RecordObject(parent, "DisconnectNode");

            if (parent is DecoratorNode decorator) {
                if (decorator.child == child) {
                    decorator.child = null;
                }
            }
            else if (parent is CompositeNode composite) {
                if (composite.children.Contains(child)) {
                    var list = composite.children.ToList();
                    list.Remove(child);
                    composite.children = list.ToArray();
                }
            }
            else if (parent is RootNode root) {
                if (root.child == child) {
                    root.child = null;
                }
            }

            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// 接続先ノードの取得
        /// </summary>
        public Node[] GetChildren(Node parent) {
            if (parent is DecoratorNode decorator) {
                return new[] { decorator.child };
            }
            else if (parent is CompositeNode composite) {
                return composite.children;
            }
            else if (parent is RootNode root) {
                return new[] { root.child };
            }

            return new Node[0];
        }
#endif
    }
}
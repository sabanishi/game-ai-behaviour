using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// Nodeのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(IfNode), true)]
    public class NodeEditor : UnityEditor.Editor
    {
        /// <summary>
        /// UIToolKit対応
        /// </summary>
        public override VisualElement CreateInspectorGUI() {
            var container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            return container;
        }
    }
}
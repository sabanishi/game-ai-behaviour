using UnityEngine.UIElements;

namespace GameAiBehaviour.Editor {
    /// <summary>
    /// 画面分割用のビュー
    /// </summary>
    public class SplitView : TwoPaneSplitView {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> {
        }
    }
}
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameAiBehaviour {
    /// <summary>
    /// エディタで使用する想定のRootAsset
    /// </summary>
    [CreateAssetMenu(fileName = "dat_game_ai_behaviour_root_asset.asset", menuName = "GameAiBehaviour/Create Root Asset")]
    public class GameAiBehaviourRootAsset : ScriptableObject {
        [Tooltip("NodeViewに使うUxml")]
        public Object nodeUxml;
    }
}
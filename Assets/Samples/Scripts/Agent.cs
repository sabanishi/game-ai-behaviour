using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Agent : MonoBehaviour {
    [SerializeField] private GameObject target;
    public GameObject Target => target;
    
    /// <summary>
    /// 相対移動
    /// </summary>
    public async UniTask MoveAsync(Vector3 offsetPosition, float duration, CancellationToken ct) {
        var timer = duration;
        var startPos = transform.position;
        var endPos = transform.TransformPoint(offsetPosition);
        while (timer > 0.0f) {
            var ratio = Mathf.Clamp01(1 - timer / duration);
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            timer -= Time.deltaTime;
            if (timer <= 0.0f) {
                break;
            }
            await UniTask.Yield(ct);
        }

        transform.position = endPos;
    }
}

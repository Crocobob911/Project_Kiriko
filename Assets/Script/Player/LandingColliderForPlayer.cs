using System;
using UnityEngine;

namespace Script {
    /// <summary>
/// 플레이어의 '착지'를 감지하는 콜라이더입니다.
/// 땅에 닿았을 때 Player.cs에 착지했음을 알려 점프 상태를 끝냅니다.
/// </summary>
    public class LandingColliderForPlayer : MonoBehaviour {
        [SerializeField] private Player player;

        private void OnTriggerEnter(Collider other) {
            Debug.Log("[LandingCollider] : " + other.transform.tag);
            if (!other.transform.CompareTag("Ground")) return;

            player.Jump_End();
        }
    }
}

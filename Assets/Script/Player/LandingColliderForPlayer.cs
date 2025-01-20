using UnityEngine;

namespace Script {
    public class LandingColliderForPlayer : MonoBehaviour {
        [SerializeField] private Player player;

        private void OnTriggerEnter(Collider other) {
            Debug.Log("[LandingCollider] : " + other.transform.tag);
            if (!other.transform.CompareTag("Ground")) return;
             
            player.Jump_End();
        }
    }
}

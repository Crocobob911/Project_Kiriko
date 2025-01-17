using UnityEngine;

namespace Script {
    public class LandingColliderForPlayer : MonoBehaviour {
        [SerializeField] private PlayerMoveController player;

        private void OnTriggerEnter(Collider other){
            if (!other.transform.CompareTag("Ground")) {
                player.Jump_end();
            }
        }
    }
}

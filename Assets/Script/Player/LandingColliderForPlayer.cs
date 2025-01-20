using UnityEngine;

namespace Script {
    public class LandingColliderForPlayer : MonoBehaviour {
        [SerializeField] private Player player;

        private void OnTriggerEnter(Collider other){
            if (!other.transform.CompareTag("Ground")) {
                player.Jump_End();
            }
        }
    }
}

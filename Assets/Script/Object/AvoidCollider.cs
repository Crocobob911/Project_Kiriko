using UnityEngine;

namespace Script {
    public class AvoidCollider : MonoBehaviour, IAttackableObject {
        [SerializeField] private Player player;

        private float time = 0;
        [SerializeField] private float successTime = 0.6f;
        
        void Start()
        {
            gameObject.SetActive(false);
        }

        void OnEnable() {
            // Debug.Log("[AvoidCollider]  OnEnable.");
            time = 0f;
        }

        void Update()
        {
            time += Time.deltaTime;
            if(time >= successTime) AvoidFail();
        }

        public void Attacked(int damage) {
            player.AvoidSuccess(time);
            time = 0f;
            gameObject.SetActive(false);
        }

        private void AvoidFail() {
            player.AvoidFail();
            time = 0f;
            gameObject.SetActive(false);
        }
    }
}

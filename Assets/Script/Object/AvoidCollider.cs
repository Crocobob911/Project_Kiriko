using UnityEngine;

namespace Script {
    public class AvoidCollider : MonoBehaviour, IAttackableObject {
        [SerializeField] private Player player;

        private float time = 0;
        [SerializeField] private float successTime = 0.39f;
        // value Modifier
        
        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable() {
            time = 0f;
        }

        private void Update()
        {
            time += Time.deltaTime;
            if(time >= successTime) AvoidFail();
        }

        public void Attacked(int damage) {
            player.Avoid_Success(time);
            time = 0f;
            gameObject.SetActive(false);
        }

        private void AvoidFail() {
            // Debug.Log("Avoid Collider");
            player.Avoid_Fail();
            time = 0f;
            gameObject.SetActive(false);
        }
    }
}

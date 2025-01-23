using UnityEngine;

namespace Script {
    public class TempWeapon : MonoBehaviour {
        public int speed;
        
        private void Update() {
            gameObject.transform.Rotate(new Vector3(0, speed, 0));
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;

            other.gameObject.GetComponent<IAttackableObject>().Attacked(10);
        }
    }
}
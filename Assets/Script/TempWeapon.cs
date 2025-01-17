using UnityEngine;

namespace Script {
    public class TempWeapon : MonoBehaviour
    {
        private void Update() {
            gameObject.transform.Rotate(new Vector3(0, 4, 0));
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;
        
            other.gameObject.GetComponent<IAttackableObject>().Attacked(10);
        }
    }
}

using UnityEngine;

namespace Script {
    /// <summary>
/// 플레이어에게 닿으면 데미지를 주는 간단한 테스트용 임시 무기입니다.
/// </summary>
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
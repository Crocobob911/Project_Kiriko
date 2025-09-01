using UnityEngine;

namespace Script {
    /// <summary>
/// '성공적인 회피(패링)' 판정을 위한 특수 콜라이더입니다.
/// 회피 시도 시 짧은 시간 동안 활성화되며, 이 때 피격되면 '회피 성공'으로 처리됩니다.
/// </summary>
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

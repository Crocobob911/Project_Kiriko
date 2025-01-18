using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    public class Player : MonoBehaviour, IAttackableObject {

        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        [SerializeField] private GameObject avoidCollider;

        private bool isAvoiding;
        
        
        private int maxHp = 300;
        public int MaxHp {
            get => maxHp;
            set {
                if(value < 0) maxHp = 0;
                else maxHp = value;
            }
        }

        private int hp;
        public int Hp {
            get => hp;
            set {
                if (value > maxHp) hp = maxHp;
                else if (value < 0) hp = 0;
                else hp = value;
            }
        }

        private void Start() {
            hp = maxHp;
            // isAvoiding = false;
        }

        public void Attacked(int damage) { 
            Hp -= damage;
            Debug.Log("[Player] | Player Attacked. Hp : " + Hp);
            moveController.KnockBack_Start();
            animController.KnockBackAnim_Start();
        }

        #region Avoid
        //==============================================================
        public void ActiveAvoid(InputAction.CallbackContext context) {
            if (!context.started || isAvoiding) return;
            
            // isAvoiding = true;
            
            // Debug.Log("[Player] Avoid Activated.");
            AvoidColliderOnReady();
            moveController.Avoid_Start();
            animController.Avoid_Start();
        }

        private void AvoidColliderOnReady() {
            avoidCollider.transform.position = transform.position + new Vector3(0, 0, 0);
            avoidCollider.SetActive(true);
        }

        public void AvoidSuccess(float time) {
            // 회피 성공
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void AvoidFail() {
            // 회피 실패
            Debug.Log("[Player] Avoid Failed.");
        }
        //==============================================================
        #endregion

    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    public class Player : MonoBehaviour, IAttackableObject {

        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        [SerializeField] private GameObject avoidCollider;

        private bool isAvoiding;
        private bool isJumping;
        
        [SerializeField] private int avoidStamina = 10;
        // value Modifier
        [SerializeField] private int jumpStamina = 10;
        // value Modifier

        private int stamina;
        private int Stamina {
            get => stamina;
            set {
                if(value > maxStamina) stamina = maxStamina;
                else if(value < 0) stamina = 0;
                stamina = value;
                Debug.Log("[Player] Stamina : " + stamina);
            }
        }
        
        private int maxStamina = 100;
        public int MaxStamina {
            get => maxStamina;
            set => maxStamina = (value < 0) ? 0 : value; 
        }
        
        
        private int maxHp = 300;
        public int MaxHp {
            get => maxHp;
            set => maxHp = (value <= 0) ? 0 : value; 
        }

        private int hp;

        private int Hp {
            get => hp;
            set {
                if (value >= maxHp) hp = maxHp;
                else if (value < 0) hp = 0;
                else hp = value;
                
                Debug.Log("[Player] HP : " + hp);
            }
        }

        
        private void Start() {
            Init();
        }
        

        private void Init() {
            Hp = MaxHp;
            Stamina = MaxStamina;
            isAvoiding = false;
            isJumping = false;
        }
        

        public void Attacked(int damage) { 
            Hp -= damage;
            moveController.KnockBack_Start();
            animController.KnockBackAnim_Start();
        }

        #region Avoid
        //==============================================================
        public void ActiveAvoid(InputAction.CallbackContext context) {
            if (!context.started || isAvoiding) return;
            
            isAvoiding = true;
            Stamina -= avoidStamina;
            
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
            isAvoiding = false;
            moveController.Avoid_End();
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void AvoidFail() {
            // 회피 실패
            isAvoiding = false;
            moveController.Avoid_End();
            Debug.Log("[Player] Avoid Failed.");
        }
        //==============================================================
        #endregion


        #region Jump
        //==============================================================
        public void Jump(InputAction.CallbackContext context) {
            if (!context.started || isJumping) return;
            
            isJumping = true;
            Stamina -= jumpStamina;
            moveController.Jump();
            animController.JumpAnim_Start();
        }

        public void Jump_End() {
            isJumping = false;
            moveController.Jump_End();
            animController.JumpAnim_End();
        }
        //==============================================================
        #endregion

    }
}

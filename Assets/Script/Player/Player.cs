using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    public class Player : CombatObject, IAttackableObject {

        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        [SerializeField] private GameObject avoidCollider;

        private bool isAvoiding;
        private bool isJumping;

        [SerializeField] private int playerMaxHP;
        [SerializeField] private int playerMaxStamina;
         
        [SerializeField] private int avoidStamina = 10;
        // value Modifier
        [SerializeField] private int jumpStamina = 10;
        // value Modifier

        
        private void Start() {
            Init();
        }
        

        private void Init() {
            MaxHp = playerMaxHP;
            MaxStamina = playerMaxStamina;
            
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
            // moveController.Avoid_End();
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void AvoidFail() {
            // 회피 실패
            isAvoiding = false;
            // moveController.Avoid_End();
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

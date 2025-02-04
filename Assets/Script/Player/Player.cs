using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    public class Player : CombatObject, IAttackableObject {
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        [SerializeField] private PlayerAttackManager attackManager; 
        [SerializeField] private GameObject avoidCollider;
        [SerializeField] private GameObject weaponCollider;

        private bool duringBehavior;
        private bool isAvoiding;
        private bool isJumping;

        [SerializeField] private int playerMaxHP;
        [SerializeField] private int playerMaxStamina;

        private int ultiGauge = 100;
        [SerializeField] private int attackDamage;
        
        [SerializeField] private int avoidStamina = 10;

        // value Modifier
        [SerializeField] private int jumpStamina = 10;
        // value Modifier


        private void Start() {
            Init();
        }

        private void Init() {
            InitHp(playerMaxHP);
            InitStamina(playerMaxStamina);
            isAvoiding = false;
            isJumping = false;
            duringBehavior = false;
        }

        private void InitHp(int _playerMaxHp) {
            MaxHp = _playerMaxHp;
            Hp = MaxHp;
        }

        private void InitStamina(int _playerMaxStamina) {
            MaxStamina = _playerMaxStamina;
            Stamina = MaxStamina;
        }

        #region Attack

        public void Attack_Start(InputAction.CallbackContext context) {
            if(!context.started || duringBehavior) return;
            
            weaponCollider.SetActive(true);
            moveController.Attack_Start();
            animController.Attack_Start();
            
            Invoke(nameof(Attack_End), 0.51f);
        }

        public void Attack(Enemy target) {
            target.Attacked(attackManager.
                CalculateDamage(PlayerAttackManager.AttackType.Normal, false));
        }

        public void Attack_End() {
            weaponCollider.SetActive(false);
            moveController.Attack_End();
        }


        public void Attacked(int damage) {
            Hp -= damage;
            moveController.KnockBack_Start();
            animController.KnockBackAnim_Start();
        }
        
        #endregion
        
        #region Avoid

        //==============================================================
        public void Avoid_Start(InputAction.CallbackContext context) {
            if (!context.started || duringBehavior) return;

            isAvoiding = true;
            duringBehavior = true;
            Stamina -= avoidStamina;

            AvoidColliderOnReady();
            moveController.Avoid_Start();
            animController.AvoidAnim_Start();
        }

        private void AvoidColliderOnReady() {
            avoidCollider.transform.position = transform.position + new Vector3(0, 0, 0);
            avoidCollider.SetActive(true);
        }

        public void Avoid_Success(float time) {
            isAvoiding = false;
            duringBehavior = false;
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void Avoid_Fail() {
            isAvoiding = false;
            duringBehavior = false;
            Debug.Log("[Player] Avoid Failed.");
        }

        //==============================================================

        #endregion

        #region Jump

        //==============================================================
        public void Jump(InputAction.CallbackContext context) {
            if (!context.started || duringBehavior) return;

            isJumping = true;
            duringBehavior = true;
            Stamina -= jumpStamina;
            moveController.Jump();
            animController.JumpAnim_Start();
        }

        public void Jump_End() {
            isJumping = false;
            duringBehavior = false;
            moveController.Jump_End();
            animController.JumpAnim_End();
        }

        //==============================================================
        
        #endregion
    }
}
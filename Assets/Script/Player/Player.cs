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


        public void Attack_Start(InputAction.CallbackContext context) {
            if(!context.started || duringBehavior) return;
            
            weaponCollider.SetActive(true);
            moveController.Attack();
            animController.Attack();
        }

        public void Attack(Enemy target) {
            Enemy.Attacked(attackManager.CalculateDamage(PlayerAttackManager.AttackType.Normal, false));
        }

        public void Attack_End() {
            moveController.Jump_End();
            animController.JumpAnim_End();
        }


        public void Attacked(int damage) {
            Hp -= damage;
            moveController.KnockBack_Start();
            animController.KnockBackAnim_Start();
        }

        #region Avoid

        //==============================================================
        public void ActiveAvoid(InputAction.CallbackContext context) {
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

        public void AvoidSuccess(float time) {
            isAvoiding = false;
            duringBehavior = false;
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void AvoidFail() {
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
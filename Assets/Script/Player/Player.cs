using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    /// <summary>
    /// 플레이어의 전반적인 상태와 행동을 총괄하는 메인 클래스입니다.
    /// 각종 컨트롤러들을 조율하고 플레이어의 핵심 상태(HP, 스태미나, 행동 가능 여부)를 관리합니다.
    /// </summary>
    public class Player : CombatObject, IAttackableObject {
        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        [SerializeField] private PlayerAttackManager attackManager; 
        [SerializeField] private GameObject avoidCollider;
        [SerializeField] private GameObject weaponCollider;

        private bool isDuringBehavior;
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
            isDuringBehavior = false;
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

        public void NormalAttack_Start() {
            if(isDuringBehavior) return;
            
            isDuringBehavior = true;
            weaponCollider.SetActive(true);
            moveController.NormalAttack_Start();
            animController.NormalAttack_Start();
            
            Invoke(nameof(Attack_End), 0.51f);
        }

        public void StrongAttack_Start() {
            if(isDuringBehavior) return;
            
            isDuringBehavior = true;
            weaponCollider.SetActive(true);
            moveController.StrongAttack_Start();
            animController.StrongAttack_Start();
            
            Invoke(nameof(Attack_End), 1.74f);
        }

        public void Attack_Normal(Enemy target) {
            target.Attacked(attackManager.
                CalculateDamage(AttackType.Normal, false));
        }

        public void Attack_Strong(Enemy target) {
            
        }

        public void Attack_End() {
            isDuringBehavior = false;
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
            if (!context.started || isDuringBehavior) return;

            isAvoiding = true;
            isDuringBehavior = true;
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
            isDuringBehavior = false;
            Debug.Log("[Player] Success Avoid. Time : " + time + "s.");
        }

        public void Avoid_Fail() {
            isAvoiding = false;
            isDuringBehavior = false;
            Debug.Log("[Player] Avoid Failed.");
        }

        //==============================================================

        #endregion

        #region Jump

        //==============================================================
        public void Jump(InputAction.CallbackContext context) {
            if (!context.started || isDuringBehavior) return;

            isJumping = true;
            isDuringBehavior = true;
            Stamina -= jumpStamina;
            moveController.Jump();
            animController.JumpAnim_Start();
        }

        public void Jump_End() {
            isJumping = false;
            isDuringBehavior = false;
            moveController.Jump_End();
            animController.JumpAnim_End();
        }

        //==============================================================
        
        #endregion
    }
}
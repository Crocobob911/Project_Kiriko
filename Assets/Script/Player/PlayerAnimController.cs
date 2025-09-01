using UnityEngine;

namespace Script {
    /// <summary>
/// 플레이어의 '애니메이션'을 모두 책임집니다.
/// 다른 스크립트로부터 명령을 받아 이동, 공격, 회피 등 상황에 맞는 애니메이션을 재생합니다.
/// </summary>
public class PlayerAnimController : MonoBehaviour, ICameraLockObserver
    {
        // 애니메이터 매개변수들
        // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
        private readonly int animIsMove = Animator.StringToHash("isMove");
        private readonly int animMoveForward = Animator.StringToHash("moveForward");
        private readonly int animMoveRight = Animator.StringToHash("moveRight");
        private readonly int animIsCamLocked = Animator.StringToHash("isCamLocked");
        private readonly int animIsOnAir = Animator.StringToHash("isOnAir");
        private readonly int animAvoid = Animator.StringToHash("avoid");
        private readonly int animKnockBack = Animator.StringToHash("knockBack");
        private readonly int animNormalAttack = Animator.StringToHash("normalAttack");
        private readonly int animStrongAttack = Animator.StringToHash("strongAttack");

        private Animator animator;
    
        private Vector2 currentDirection;
        private Vector2 newDirection;
    
        private void Start() {
            animator = transform.GetChild(1).GetComponent<Animator>();
            Init();
            
            CameraRotateController.Instance.AddMeLockObserver(this);
        }
    
        private void Init() {
            currentDirection = Vector2.zero;
            newDirection = Vector2.zero;
        }
        
        private void Update() {
            ChangeMoveAction();
        }
    
        private void ChangeMoveAction() {
            if(newDirection == currentDirection) return;
            
            SetMoveVectorOfAnimator(
                LerpVector(currentDirection, newDirection));
        }

        private void SetMoveVectorOfAnimator(Vector2 dir) {
            animator.SetFloat(animMoveForward, dir.y);
            animator.SetFloat(animMoveRight, dir.x);
        }

        private Vector2 LerpVector(Vector2 current, Vector2 target) {
            return (current - target).magnitude >= 0.01f 
                ? Vector2.Lerp(current, target, 5f * Time.deltaTime) : target;
        }
        
        /// <summary>
        /// get moveDirection as a parameter to change the moving animation of player
        /// </summary>
        /// <param name="moveDir"></param>
        public void SetMoveDirection(Vector2 moveDir) {
            newDirection = moveDir;
            animator.SetBool(animIsMove, moveDir != Vector2.zero);
        }

        public void JumpAnim_Start() {
            animator.SetBool(animIsOnAir, true);
            SetMoveDirection(Vector2.zero);
        
            // 도약 모션 재생 들어가야함.
            // 그 뒤에 체공 모션으로 자연스레 넘어가야함.
            
            // 카메라 락온, 락온 아님에 따라 다른 애니 출력해줘야함.
        }

        public void JumpAnim_End() {
            animator.SetBool(animIsOnAir, false);
            // 착지 모션 재생 들어가야함.
        }
        
        public void KnockBackAnim_Start() {
            animator.SetTrigger(animKnockBack);
        }
        
        
        public void AvoidAnim_Start() {
            animator.SetTrigger(animAvoid);
            // 카메라 락온 상태에 따라서 다른 애니메이션 출력해줘야함.
        }

        public void NormalAttack_Start() {
            animator.SetTrigger(animNormalAttack);
        }
        
        public void StrongAttack_Start() {
            animator.SetTrigger(animStrongAttack);
        }

        public void CamLockUpdate(bool locked) {
            animator.SetBool(animIsCamLocked, locked);
        }

    }
}

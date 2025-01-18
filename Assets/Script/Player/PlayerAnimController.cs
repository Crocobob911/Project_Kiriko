using UnityEngine;

namespace Script {
    public class PlayerAnimController : MonoBehaviour
    {
        // 애니메이터 매개변수들
        // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
        private readonly int animIsMove = Animator.StringToHash("isMove");
        private readonly int animMoveForward = Animator.StringToHash("moveForward");
        private readonly int animMoveRight = Animator.StringToHash("moveRight");
        private readonly int animIsCamLocked = Animator.StringToHash("isCamLocked");
        private readonly int animIsOnAir = Animator.StringToHash("isOnAir");
        private readonly int animAvoid = Animator.StringToHash("avoid");
        private readonly int animIsJump = Animator.StringToHash("isJump");
        private readonly int animKnockBack = Animator.StringToHash("knockBack");

        [SerializeField]private Animator animator;
    
        [SerializeField]private Vector2 currentDirection;
        [SerializeField]private Vector2 newDirection;

        private void Init() {
            currentDirection = Vector2.zero;
            newDirection = Vector2.zero;
        }
    
        void Start() {
            animator = transform.GetChild(1).GetComponent<Animator>();
        
            Init();
        }
    
        void Update() {
            SetPlayerAnim();
        }
    
        private void SetPlayerAnim() {
            if(newDirection == currentDirection) return;
        
            LerpMoveAnimDirection();
        
            animator.SetFloat(animMoveForward, currentDirection.y);
            animator.SetFloat(animMoveRight, currentDirection.x);
        }
    
        public void SetMoveAnimDirection(Vector2 moveDir) {
            newDirection = moveDir;
            animator.SetBool(animIsMove, moveDir != Vector2.zero);
        }

        private void LerpMoveAnimDirection() {
            if ((currentDirection - newDirection).magnitude >= 0.001f) {
                currentDirection = Vector2.Lerp(currentDirection, newDirection, 5f * Time.deltaTime);
            }
            else currentDirection = newDirection;
        }

        public void JumpAnim_Start() {
            animator.SetBool(animIsOnAir, true);
            SetMoveAnimDirection(Vector2.zero);
        
            // 도약 모션 재생 들어가야함.
        }

        public void JumpAnim_End() {
            animator.SetBool(animIsOnAir, false);
        
            // 착지 모션 재생 들어가야함.
        }
        

        public void KnockBackAnim_Start() {
            animator.SetTrigger(animKnockBack);
        }

        private void KnockBackAnim_End() {
            // animator.SetBool(animKnockBack, false);
        }


        public void Avoid_Start() {
            animator.SetTrigger(animAvoid);
        }

        private void Avoid_End() {
            // animator.SetBool(animAvoid, false);
        }
        
                
        public void SetIsCamLocked(bool isCamLocked) {
            animator.SetBool(animIsCamLocked, isCamLocked);
        }
    }
}
